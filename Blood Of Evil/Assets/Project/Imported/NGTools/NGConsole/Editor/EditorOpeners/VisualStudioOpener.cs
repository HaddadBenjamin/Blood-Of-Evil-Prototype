using NGTools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class VisualStudioOpener : IEditorOpener
	{
		public const string realPath = Constants.RootFolderName + @"\NGConsole\Editor\EditorOpeners\OpenLineVisualStudio.exe";

		public string	defaultArguments { get { return "\"$(File)\" $(Line)"; } }

		private string	exePath;

		public bool	CanHandleEditor(string editor)
		{
			return editor.Contains("devenv") || editor.Contains("WDExpress") || editor.Contains("VSWinExpress");
		}

		public void	Open(string editorPath, string arguments, string file, int line)
		{
			try
			{
				if (string.IsNullOrEmpty(this.exePath) == true)
				{
					this.SearchExe();
					if (string.IsNullOrEmpty(this.exePath) == true)
					{
						Debug.LogWarning("NG Console can not find the file OpenLineVisualStudio.exe.");
						return;
					}
				}

				Utility.OpenFileLine(this.exePath, arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()));
			}
			catch (Exception e)
			{
				InternalNGDebug.LogException(e);
			}
		}

		private void	SearchExe()
		{
			Stack<string>	dirs = new Stack<string>();
			Stack<string>	exes = new Stack<string>();

			dirs.Push("Assets");
			while (dirs.Count > 0)
			{
				foreach (var d in Directory.GetDirectories(dirs.Pop()))
				{
					foreach (var f in Directory.GetFiles(d, "OpenLineVisualStudio.exe"))
					{
						exes.Push(f);
					}
					dirs.Push(d);
				}
			}

			foreach (var exe in exes)
			{
				if (exe.Contains(VisualStudioOpener.realPath) == true)
				{
					this.exePath = exe;
					break;
				}
			}
		}
	}
}