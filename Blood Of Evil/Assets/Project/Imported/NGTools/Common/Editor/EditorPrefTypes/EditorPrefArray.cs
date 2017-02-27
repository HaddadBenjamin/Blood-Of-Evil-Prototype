using NGTools;
using System;
using System.Runtime.Serialization;

namespace NGToolsEditor
{
	internal sealed class EditorPrefArray : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return type.IsArray == true;
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Array	array = instance as Array;
			if (instance == null)
			{
				NGEditorPrefs.DeleteKey(path);
				return;
			}

			NGEditorPrefs.SetInt(path, array.Length);

			try
			{
				if (array.Length > 0)
				{
					Type	subType = Utility.GetArraySubType(type);

					if (subType.IsValueType == true || subType == typeof(string) || subType.IsInterface == true || subType.IsAbstract == true)
					{
						NGEditorPrefs.SetString(path + ".serialized", Convert.ToBase64String(Utility.SerializeField(array)));
						return;
					}
				}

				for (int i = 0; i < array.Length; i++)
				{
					object	value = array.GetValue(i);

					if (value != null)
						Utility.DirectSaveEditorPref(value, value.GetType(), path + '.' + i);
					else
						NGEditorPrefs.DeleteKey(path + '.' + i);
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("EditorPrefArray failed saving at \"" + path + "\".", ex);
			}
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			int	length = NGEditorPrefs.GetInt(path, -1);
			if (length == -1)
				return instance;

			Type	subType = type.GetElementType();

			if (length > 0 && (subType.IsValueType == true || subType == typeof(string) || subType.IsInterface == true || subType.IsAbstract == true))
			{
				string	v = NGEditorPrefs.GetString(path + ".serialized", null);

				if (v != null)
					return Utility.DeserializeField(Convert.FromBase64String(v));
				return instance;
			}

			Array	array = Array.CreateInstance(subType, length) as Array;

			try
			{
				for (int i = 0; i < array.Length; i++)
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

					array.SetValue(element, i);
				}

				return array;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("EditorPrefArray failed fetching at \"" + path + "\".", ex);
			}

			return instance;
		}
	}
}