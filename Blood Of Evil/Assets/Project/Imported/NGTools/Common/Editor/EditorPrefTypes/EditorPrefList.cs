using NGTools;
using System;
using System.Collections;
using System.Runtime.Serialization;

namespace NGToolsEditor
{
	/// <summary>
	/// Saves IList with any class, but the class must be public and implement a default constructor.
	/// </summary>
	internal sealed class EditorPrefList : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return typeof(IList).IsAssignableFrom(type);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			IList	list = instance as IList;
			if (instance == null)
			{
				NGEditorPrefs.DeleteKey(path);
				return;
			}

			NGEditorPrefs.SetInt(path, list.Count);

			try
			{
				if (list.Count > 0)
				{
					Type	subType = Utility.GetArraySubType(type);

					if (subType.IsValueType == true || subType == typeof(string) || subType.IsInterface == true || subType.IsAbstract == true)
					{
						NGEditorPrefs.SetString(path + ".serialized", Convert.ToBase64String(Utility.SerializeField(list)));
						return;
					}
				}

				for (int i = 0; i < list.Count; i++)
				{
					object	value = list[i];

					if (value != null)
						Utility.DirectSaveEditorPref(value, value.GetType(), path + '.' + i);
					else
						NGEditorPrefs.DeleteKey(path + '.' + i);
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("EditorPrefList failed saving at \"" + path + "\".", ex);
			}
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			int	count = NGEditorPrefs.GetInt(path, -1);
			if (count == -1)
				return instance;

			Type	subType = Utility.GetArraySubType(type);

			if (count > 0 && (subType.IsValueType == true || subType == typeof(string) || subType.IsInterface == true || subType.IsAbstract == true))
			{
				string	v = NGEditorPrefs.GetString(path + ".serialized", null);

				if (string.IsNullOrEmpty(v) == false)
					return Utility.DeserializeField(Convert.FromBase64String(v));
				return instance;
			}

			IList	list = Activator.CreateInstance(type) as IList;

			try
			{
				for (int i = 0; i < count; i++)
				{
					object	element = null;

					try
					{
						if (subType.IsValueType == true)
							element = Activator.CreateInstance(subType);
						else if (subType != typeof(string))
						{
							try
							{
								element = Activator.CreateInstance(subType);
							}
							catch (MissingMethodException)
							{
								element = FormatterServices.GetUninitializedObject(subType);
							}
						}
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException(ex);
						break;
					}

					element = Utility.LoadEditorPref(element, subType, path + '.' + i);

					list.Add(element);
				}

				return list;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("EditorPrefList failed fetching at \"" + path + "\".", ex);
			}

			return instance;
		}
	}
}