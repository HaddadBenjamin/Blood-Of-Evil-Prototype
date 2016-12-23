using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class MonoDevelopOpener : IEditorOpener
	{
		public string	defaultArguments { get { return "--nologo \"$(File);$(Line)\""; } }

		public bool	CanHandleEditor(string editor)
		{
			return string.IsNullOrEmpty(editor) || editor.Contains("MonoDevelop");
		}

		public void	Open(string editorPath, string arguments, string file, int line)
		{
			if (string.IsNullOrEmpty(editorPath) == true)
			{
#if UNITY_EDITOR_WIN
				// Try to use the default executable.
				if (System.IO.File.Exists(@"C:\Program Files\Unity\MonoDevelop\bin\MonoDevelop.exe") == true)
					Utility.OpenFileLine(@"C:\Program Files\Unity\MonoDevelop\bin\MonoDevelop.exe", arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()));
				else
				{
					TextAsset	fileAsset = AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset)) as TextAsset;
					AssetDatabase.OpenAsset(fileAsset, line);
					return;
				}
#else
				TextAsset	fileAsset = AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset)) as TextAsset;
				AssetDatabase.OpenAsset(fileAsset, line);
				//Application.OpenURL(file);
				return;
#endif
			}
			else
				Utility.OpenFileLine(editorPath, arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()));
		}
	}
}