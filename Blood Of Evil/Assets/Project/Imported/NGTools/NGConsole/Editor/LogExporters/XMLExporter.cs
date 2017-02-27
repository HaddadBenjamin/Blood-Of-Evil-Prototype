using System;
using System.Collections.Generic;
using System.Text;

namespace NGToolsEditor.NGConsole
{
	internal sealed class XMLExporter : ILogExporter
	{
		private StringBuilder	buffer;

		public void	OnEnable()
		{
		}

		public void	OnDestroy()
		{
		}

		public void	OnGUI()
		{
		}

		public void	AddColumn(string key, string value, string[] attributes)
		{
			this.buffer.Append('<');
			this.buffer.Append(key);
			this.buffer.Append('>');
			this.buffer.Append(value);
			this.buffer.Append("</");
			this.buffer.Append(key);
			this.buffer.Append(">");
		}

		public string	Generate(List<Row> rows, ExportRowsEditorWindow.ExportSettings settings)
		{
			this.buffer = Utility.GetBuffer();

			for (int i = 0; i < rows.Count; i++)
			{
				ILogContentGetter	logContent = rows[i] as ILogContentGetter;

				if (settings.outputIndex == true)
				{
					this.buffer.Append("<i>");
					this.buffer.Append(i);
					this.buffer.Append("</i>");
				}
				if (settings.outputTime == true)
				{
					this.buffer.Append("<time>");
					this.buffer.Append(rows[i].log.time);
					this.buffer.Append("</time>");
				}
				if (settings.outputContent == ExportRowsEditorWindow.OutputContent.FirstLine)
				{
					this.buffer.Append("<content>");
					if (logContent != null)
						this.buffer.Append(logContent.HeadMessage);
					this.buffer.Append("</content>");
				}
				else if (settings.outputContent == ExportRowsEditorWindow.OutputContent.FullLog)
				{
					this.buffer.Append("<content>");
					if (logContent != null)
						this.buffer.Append(logContent.FullMessage);
					this.buffer.Append("</content>");
				}
				if (settings.outputLogFile == true)
				{
					this.buffer.Append("<file>");
					this.buffer.Append(rows[i].log.file);
					this.buffer.Append("</file>");
				}
				if (settings.outputLogFileLine == true)
				{
					this.buffer.Append("<line>");
					this.buffer.Append(rows[i].log.line);
					this.buffer.Append("</line>");
				}
				if (settings.outputStackTrace == true)
				{
					this.buffer.Append("<stackTrace>");
					if (logContent != null)
						this.buffer.Append(logContent.StackTrace);
					this.buffer.Append("</stackTrace>");
				}

				if (settings.callbackLog != null)
					settings.callbackLog(this, rows[i]);

				this.buffer.Append(Environment.NewLine);
			}

			// Remove last row separator.
			if (this.buffer.Length - 1 >= 0)
				this.buffer.Length -= 1;

			return Utility.ReturnBuffer(this.buffer);
		}
	}
}