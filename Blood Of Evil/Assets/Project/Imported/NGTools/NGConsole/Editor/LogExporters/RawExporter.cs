using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class RawExporter : ILogExporter
	{
		public string	dataSeparator = "	";
		public string	logSeparator = Environment.NewLine;

		private StringBuilder	buffer;

		public void	OnEnable()
		{
			Utility.LoadEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
		}

		public void	OnDestroy()
		{
			Utility.SaveEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
		}

		public void	OnGUI()
		{
			EditorGUILayout.LabelField(LC.G("DataSeparator"));
			this.dataSeparator = EditorGUILayout.TextArea(this.dataSeparator);

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField(LC.G("LogSeparator"));

				if (GUILayout.Button("CR") == true)
					this.logSeparator += "\r";

				if (GUILayout.Button("LF") == true)
					this.logSeparator += "\n";
			}
			EditorGUILayout.EndHorizontal();

			this.logSeparator = EditorGUILayout.TextArea(this.logSeparator);
		}

		public void	AddColumn(string key, string value, string[] attributes)
		{
			this.buffer.Append(value);
			this.buffer.Append(this.dataSeparator);
		}

		public string	Generate(List<Row> rows, ExportRowsEditorWindow.ExportSettings settings)
		{
			this.buffer = Utility.GetBuffer();

			for (int i = 0; i < rows.Count; i++)
			{
				ILogContentGetter	logContent = rows[i] as ILogContentGetter;

				if (settings.outputIndex == true)
				{
					this.buffer.Append(i);
					this.buffer.Append(this.dataSeparator);
				}
				if (settings.outputTime == true)
				{
					this.buffer.Append(rows[i].log.time);
					this.buffer.Append(this.dataSeparator);
				}
				if (settings.outputContent == ExportRowsEditorWindow.OutputContent.FirstLine)
				{
					if (logContent != null)
						this.buffer.Append(logContent.HeadMessage);
					this.buffer.Append(this.dataSeparator);
				}
				else if (settings.outputContent == ExportRowsEditorWindow.OutputContent.FullLog)
				{
					if (logContent != null)
						this.buffer.Append(logContent.FullMessage);
					this.buffer.Append(this.dataSeparator);
				}
				if (settings.outputLogFile == true)
				{
					this.buffer.Append(rows[i].log.file);
					this.buffer.Append(this.dataSeparator);
				}
				if (settings.outputLogFileLine == true)
				{
					this.buffer.Append(rows[i].log.line);
					this.buffer.Append(this.dataSeparator);
				}
				if (settings.outputStackTrace == true)
				{
					if (logContent != null)
						this.buffer.Append(logContent.StackTrace);
					this.buffer.Append(this.dataSeparator);
				}

				if (settings.callbackLog != null)
					settings.callbackLog(this, rows[i]);

				// Remove last dataSeparator.
				if (this.buffer.Length - this.dataSeparator.Length >= 0)
					this.buffer.Length -= this.dataSeparator.Length;

				this.buffer.Append(this.logSeparator);
			}

			// Remove last dataSeparator.
			if (this.buffer.Length - this.logSeparator.Length >= 0)
				this.buffer.Length -= this.logSeparator.Length;

			return Utility.ReturnBuffer(buffer);
		}
	}
}