using NGTools;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public sealed class SectionDrawer
	{
		public readonly string	sectionName;
		public readonly Type	typeSetting;

		private SerializedObject	so;
		private NGSettings.Settings	settings;
		private FieldInfo			fieldInfo;

		/// <summary>
		/// Initializes a Section.
		/// </summary>
		/// <param name="typeSetting">The Type of your class inside NGSettings.</param>
		/// <param name="priority">The lower the nearest to the top.</param>
		public	SectionDrawer(Type typeSetting, int priority = -1) : this(null, typeSetting, priority)
		{
		}

		/// <summary>
		/// Initializes a Section and adds it into NGSettings.
		/// </summary>
		/// <param name="sectionName">Defines the name of the section.</param>
		/// <param name="typeSetting">The Type of your class inside NGSettings.</param>
		/// <param name="priority">The lower the nearest to the top.</param>
		public	SectionDrawer(string sectionName, Type typeSetting, int priority = -1)
		{
			this.sectionName = sectionName;
			this.typeSetting = typeSetting;

			FieldInfo[]	fields = typeof(NGSettings).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].FieldType == typeSetting)
				{
					this.fieldInfo = fields[i];
					break;
				}
			}

			InternalNGDebug.Assert(this.fieldInfo != null, "Field of type \"" + typeSetting + "\" does not exist in class \"" + typeof(NGSettings) + "\".");

			if (this.sectionName != null)
				NGSettingsWindow.AddSection(this.sectionName, this.OnGUI, priority);
		}

		public void	Uninit()
		{
			if (this.sectionName != null)
				NGSettingsWindow.RemoveSection(this.sectionName);
		}

		public void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				this.so = null;
				GUILayout.Label(LC.G("ConsoleSettings_NullTarget"));
				return;
			}

			if (this.so == null || this.so.targetObject == null || this.so.targetObject != Preferences.Settings)
			{
				this.so = new SerializedObject(Preferences.Settings);
				this.settings = this.fieldInfo.GetValue(Preferences.Settings) as NGSettings.Settings;

				if (this.settings != null)
					this.settings.InternalInitGUI();
			}
			else
				this.so.Update();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)) == true &&
				((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(NGSettingsWindow.Title, LC.G("ConsoleSettings_ResetConfirm"), LC.G("Yes"), LC.G("No")) == true))
			{
				object	settings = Activator.CreateInstance(this.fieldInfo.FieldType);

				this.settings = settings as NGSettings.Settings;

				if (this.settings != null)
					this.settings.InternalInitGUI();
				this.fieldInfo.SetValue(Preferences.Settings, settings);
			}
			GUILayout.EndHorizontal();

			SerializedProperty	iterator = this.so.FindProperty(this.fieldInfo.Name);
			SerializedProperty	end = iterator.GetEndProperty();
			bool				enterChildren = true;

			while (iterator.NextVisible(enterChildren) == true && SerializedProperty.EqualContents(iterator, end) == false)
			{
				EditorGUILayout.PropertyField(iterator, true);
				enterChildren = false;
			}

			this.so.ApplyModifiedProperties();
		}
	}
}