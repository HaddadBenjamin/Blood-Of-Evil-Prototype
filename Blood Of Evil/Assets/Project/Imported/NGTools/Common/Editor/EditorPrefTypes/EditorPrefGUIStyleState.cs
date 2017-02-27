using NGTools;
using System;
using System.Reflection;
using UnityEngine;

namespace NGToolsEditor
{
	internal sealed class EditorPrefGUIStyleState : EditorPrefType
	{
		public PropertyModifier[]	fields;

		public	EditorPrefGUIStyleState()
		{
			Type	type = typeof(GUIStyleState);

			this.fields = new PropertyModifier[] {
				new PropertyModifier(type.GetProperty("background", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
				new PropertyModifier(type.GetProperty("textColor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)),
			};
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(GUIStyleState);
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