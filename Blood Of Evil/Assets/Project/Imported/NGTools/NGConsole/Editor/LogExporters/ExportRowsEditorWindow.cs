using NGTools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	public class ExportRowsEditorWindow : EditorWindow
	{
		public enum OutputContent
		{
			FirstLine,
			FullLog
		}

		[File(FileAttribute.Mode.Save, "log")]
		public string	exportFile;
		public int		selectedExporter;

		public sealed class ExportSettings
		{
			public bool				outputIndex = true;
			public bool				outputTime = true;
			public OutputContent	outputContent = OutputContent.FirstLine;
			public bool				outputLogFile = false;
			public bool				outputLogFileLine = false;
			public bool				outputStackTrace = false;

			public Action<ILogExporter, Row>	callbackLog;
		}
		private ExportSettings	settings = new ExportSettings();

		private ILogExporter[]	exporters;
		private string[]		names;
		private string			preview;
		private List<Row>		rows;
		private Vector2			scrollPosition;

		public static void	Export(List<Row> rows, Action<ILogExporter, Row> callbackLog = null)
		{
			ExportRowsEditorWindow	wizard = ScriptableWizard.GetWindow<ExportRowsEditorWindow>(true, "Export logs", true);

			wizard.rows = rows;
			wizard.settings.callbackLog = callbackLog;
		}

		protected virtual void	OnEnable()
		{
			List<ILogExporter>	exporters = new List<ILogExporter>();
			List<string>		names = new List<string>();

			foreach (Type c in Utility.EachAssignableFrom(typeof(ILogExporter), t => t.UnderlyingSystemType != typeof(ILogExporter)))
			{
				exporters.Add((ILogExporter)Activator.CreateInstance(c));
				names.Add(Utility.NicifyVariableName(c.Name));
			}

			this.exporters = exporters.ToArray();
			this.names = names.ToArray();

			Utility.LoadEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
			Utility.LoadEditorPref(this.settings, NGEditorPrefs.GetPerProjectPrefix());

			for (int i = 0; i < this.exporters.Length; i++)
				this.exporters[i].OnEnable();
		}

		protected virtual void	OnDestroy()
		{
			Utility.SaveEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
			Utility.SaveEditorPref(this.settings, NGEditorPrefs.GetPerProjectPrefix());

			for (int i = 0; i < this.exporters.Length; i++)
				this.exporters[i].OnDestroy();
		}

		protected virtual void	OnGUI()
		{
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				EditorGUI.BeginChangeCheck();
				int	e = EditorGUILayout.Popup(LC.G("Exporter"), this.selectedExporter, this.names);
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.selectedExporter = e;
					this.UpdatePreview();
				}

				if (0 <= this.selectedExporter && this.selectedExporter < this.exporters.Length)
				{
					NGEditorGUILayout.SaveFileField(LC.G("ExportFilePath"), this.exportFile, string.Empty, string.Empty);

					EditorGUI.BeginChangeCheck();
					this.settings.outputIndex = EditorGUILayout.Toggle(LC.G("OutputIndex"), this.settings.outputIndex);
					this.settings.outputTime = EditorGUILayout.Toggle(LC.G("OutputTime"), this.settings.outputTime);
					this.settings.outputContent = (OutputContent)EditorGUILayout.EnumPopup(LC.G("OutputContent"), this.settings.outputContent);
					this.settings.outputLogFile = EditorGUILayout.Toggle(LC.G("OutputLogFile"), this.settings.outputLogFile);
					this.settings.outputLogFileLine = EditorGUILayout.Toggle(LC.G("OutputLogFileLine"), this.settings.outputLogFileLine);
					this.settings.outputStackTrace = EditorGUILayout.Toggle(LC.G("OutputStackTrace"), this.settings.outputStackTrace);

					this.exporters[this.selectedExporter].OnGUI();
					if (EditorGUI.EndChangeCheck() == true ||
						string.IsNullOrEmpty(this.preview) == true)
					{
						this.UpdatePreview();
					}

					GUILayout.Label(string.Format(LC.G("PreviewOfNFirstRows"), Constants.PreviewRowsCount));
					GUILayout.TextArea(this.preview);

					EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(this.exportFile));
					{
						using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
						{
							if (GUILayout.Button(LC.G("Export")) == true)
								this.ExportLogs();
						}
					}
					EditorGUI.EndDisabledGroup();
				}
			}
			EditorGUILayout.EndScrollView();
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		private void	ExportLogs()
		{
			try
			{
				File.WriteAllText(this.exportFile, this.exporters[this.selectedExporter].Generate(this.rows, this.settings));
				Debug.Log(LC.G("LogsExportedTo") + "\"" + this.exportFile + "\".");
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		private void	UpdatePreview()
		{
			var	rows = new List<Row>();

			for (int i = 0; i < Constants.PreviewRowsCount && i < this.rows.Count; i++)
				rows.Add(this.rows[i]);

			this.preview = this.exporters[this.selectedExporter].Generate(rows, this.settings);
		}
	}
}