using System;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	internal sealed class EditorPrefObject : EditorPrefType
	{
		public override bool	CanHandle(Type type)
		{
			return typeof(Object).IsAssignableFrom(type);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			Object	obj = instance as Object;
			string	assetPath = string.Empty;

			if (obj != null)
				assetPath = AssetDatabase.GetAssetPath(obj);

			NGEditorPrefs.SetString(path, assetPath);
		}

		public override void	Save(object instance, Type type, string path)
		{
			if (instance == null)
				return;

			foreach (var field in Utility.EachFieldHierarchyOrdered(type, typeof(object), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (field.IsDefined(typeof(NonSerializedAttribute), true) == true ||
					(field.IsPublic == false && field.IsDefined(typeof(SerializeField), true) == false))
				{
					continue;
				}

				object	value = field.GetValue(instance);

				if (value == null || object.Equals(value, 0) == true)
					value = this.GetDefaultValue(field) ?? value;

				Utility.DirectSaveEditorPref(value, field.FieldType, path + type.FullName + '.' + field.Name);
			}
		}

		public override void	Load(object instance, Type type, string path)
		{
			foreach (var field in Utility.EachFieldHierarchyOrdered(type, typeof(object), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (field.IsDefined(typeof(NonSerializedAttribute), true) == true ||
					(field.IsPublic == false && field.IsDefined(typeof(SerializeField), true) == false))
				{
					continue;
				}

				field.SetValue(instance, Utility.LoadEditorPref(field.GetValue(instance), field.FieldType, path + type.FullName + '.' + field.Name));
			}
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			if (NGEditorPrefs.HasKey(path) == true)
			{
				string	assetPath = NGEditorPrefs.GetString(path, string.Empty);

				if (assetPath != string.Empty)
				{
					if (assetPath == "Library/unity editor resources")
						return instance;
					return AssetDatabase.LoadAssetAtPath(assetPath, type) ?? instance;
				}

				return null;
			}

			return instance;
		}
	}
}