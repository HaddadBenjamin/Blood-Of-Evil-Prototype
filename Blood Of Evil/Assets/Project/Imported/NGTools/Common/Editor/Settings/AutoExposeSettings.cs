using NGTools;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class AutoExposeSettings<T> where T : AbstractModuleSettings
	{
		public readonly T				instance;
		public readonly IGUIInitializer	GUIInitializer;

		private string					sectionTitle;
		private SerializedObject		serializedObject;
		private SerializedProperty[]	serializedProperties;

		/// <summary>
		/// Adds a new section in ConsoleSettings with an inspector displaying all public non-static fields from type T.
		/// </summary>
		/// <param name="sectionTitle">Title of the section.</param>
		/// <param name="assetPath">Location in the project to save and load the instance.</param>
		public	AutoExposeSettings(string sectionTitle, string assetPath) : this(sectionTitle, assetPath, AutoExposeSettings<T>.GetAllPublicNonStaticFields())
		{
		}

		/// <summary>
		/// Adds a new section in ConsoleSettings with an inspector displaying all public non-static fields from type T.
		/// </summary>
		/// <param name="sectionTitle">Title of the section.</param>
		/// <param name="assetPath">Location in the project to save and load the instance.</param>
		/// <param name="exposedFields">List of public non-static fields to expose.</param>
		public	AutoExposeSettings(string sectionTitle, string assetPath, string[] exposedFields)
		{
			this.sectionTitle = sectionTitle;

			this.instance = Utility.LoadAssetAtPath<T>(assetPath);
			if (this.instance == null)
			{
				this.instance = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(this.instance, assetPath);
				AssetDatabase.SaveAssets();
			}

			this.GUIInitializer = this.instance as IGUIInitializer;

			this.serializedObject = new SerializedObject(this.instance);
			this.serializedProperties = new SerializedProperty[exposedFields.Length];
			for (int i = 0; i < exposedFields.Length; i++)
			{
				this.serializedProperties[i] = this.serializedObject.FindProperty(exposedFields[i]);
				InternalNGDebug.Assert(this.serializedProperties[i] != null, "Field \"" + exposedFields[i] + "\" was not found in type " + typeof(T).FullName + ".", this.instance);
			}

			NGSettingsWindow.AddSection(sectionTitle, this.OnGUI);
		}

		public void	Uninit()
		{
			NGSettingsWindow.RemoveSection(this.sectionTitle);
		}

		public void	OnGUI()
		{
			if (this.GUIInitializer != null)
				this.GUIInitializer.InitGUI();

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				NGEditorGUILayout.PingObject("Ping", this.instance);
			}
			GUILayout.EndHorizontal();

#if UNITY_5_6 || UNITY_5_6_OR_NEWER
			this.serializedObject.UpdateIfRequiredOrScript();
#else
			this.serializedObject.UpdateIfDirtyOrScript();
#endif

			for (int i = 0; i < this.serializedProperties.Length; i++)
				EditorGUILayout.PropertyField(this.serializedProperties[i], true);

			this.serializedObject.ApplyModifiedProperties();
		}

		private static string[]	GetAllPublicNonStaticFields()
		{
			FieldInfo[]	fi = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
			string[]	fieldNames = new string[fi.Length];

			for (int i = 0; i < fi.Length; i++)
				fieldNames[i] = fi[i].Name;

			return fieldNames;
		}
	}
}