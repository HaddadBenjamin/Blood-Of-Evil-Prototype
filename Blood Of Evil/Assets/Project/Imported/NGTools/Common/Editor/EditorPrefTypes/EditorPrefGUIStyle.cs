using NGTools;
using System;
using System.Reflection;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefGUIStyle : EditorPrefType
	{
		public PropertyModifier[]	fields;

		public	EditorPrefGUIStyle()
		{
			Type	type = typeof(GUIStyle);

			this.fields = new PropertyModifier[] {
				new PropertyModifier(type.GetProperty("name", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("normal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("hover", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("active", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("focused", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("onNormal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("onHover", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("onActive", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("onFocused", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("border", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("margin", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("padding", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("overflow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("font", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("fontSize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("fontStyle", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("alignment", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("wordWrap", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("richText", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("clipping", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("imagePosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("contentOffset", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("fixedWidth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("fixedHeight", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("stretchWidth", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("stretchHeight", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
			};
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(GUIStyle);
		}

		public override void	DirectSave(object instance, Type type, string path)
		{
			if (instance == null)
				return;

			for (int i = 0; i < this.fields.Length; i++)
			{
				object	value = this.fields[i].GetValue(instance);

				Utility.DirectSaveEditorPref(value, this.fields[i].Type, path + '.' + this.fields[i].Name);
			}
		}

		public override void	Load(object instance, Type type, string path)
		{
			for (int i = 0; i < this.fields.Length; i++)
				this.fields[i].SetValue(instance, Utility.LoadEditorPref(this.fields[i].GetValue(instance), this.fields[i].Type, path + '.' + this.fields[i].Name));
		}

		public override object	Fetch(object instance, Type type, string path)
		{
			if (instance == null)
				return null;

			this.Load(instance, type, path);

			return instance;
		}
	}
}