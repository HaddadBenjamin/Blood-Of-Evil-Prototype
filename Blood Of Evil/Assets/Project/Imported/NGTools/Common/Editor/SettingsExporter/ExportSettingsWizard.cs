using NGTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class ExportSettingsWizard : ScriptableWizard
	{
		[File(FileAttribute.Mode.Save, "")]
		public string	exportFile;

		private SettingsExporter.Node	root;
		private List<object>			instances;
		private Vector2					scrollPosition;
		private GUIStyle				richTextField;

		protected virtual void	OnEnable()
		{
			this.instances = new List<object>();

			foreach (var type in Utility.EachSubClassesOf(typeof(EditorWindow), (t) => t.IsDefined(typeof(ExportableAttribute), false)))
			{
				Object[]	instances = Resources.FindObjectsOfTypeAll(type);
				if (instances.Length > 0)
					this.instances.Add(instances[0]);
			}

			this.root = SettingsExporter.Collect(this.instances.ToArray());
			//this.OutputNode(this.root);
			Utility.LoadEditorPref(this);
		}

		protected virtual void	OnDestroy()
		{
			Utility.SaveEditorPref(this);
		}

		protected virtual void	OnGUI()
		{
			if (this.richTextField == null)
			{
				this.richTextField = new GUIStyle(GUI.skin.textField);
				this.richTextField.richText = true;
			}

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			this.exportFile = EditorGUILayout.TextField("Export File", this.exportFile);
#else
			this.DrawWizardGUI();
#endif

			using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
			{
				if (GUILayout.Button(LC.G("ExportSettings_Export")) == true)
				{
					if (SettingsExporter.Export(this.instances, this.root, exportFile) == true)
						Debug.Log(LC.G("ExportSettings_ExportSuccess"));
					else
						Debug.LogError(LC.G("ExportSettings_ExportFailed"));
				}
			}

			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			this.DrawNode(this.root);

			GUI.enabled = true;

			EditorGUILayout.EndScrollView();
		}

		private void	DrawNode(SettingsExporter.Node node)
		{
			bool	enabled = GUI.enabled;

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Space((GUI.depth - 1) * 16F);

				node.include = GUILayout.Toggle(node.include, Utility.NicifyVariableName(node.name));
				if (node.value != null)
					GUILayout.Label("<color=cyan>" + node.value + "</color>", this.richTextField);

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();

			++GUI.depth;
			for (int i = 0; i < node.children.Count; i++)
			{
				if (node.children[i].options == SettingsExporter.Node.Options.Normal)
				{
					if (enabled == true)
						GUI.enabled = node.include;
					this.DrawNode(node.children[i]);
				}
			}
			--GUI.depth;
		}

		private void	OutputNode(SettingsExporter.Node node, int depth = 0)
		{
			Debug.Log(new string('	', depth) + node.name + "=" + node.value);

			for (int i = 0; i < node.children.Count; i++)
				this.OutputNode(node.children[i], depth + 1);
		}
	}
}