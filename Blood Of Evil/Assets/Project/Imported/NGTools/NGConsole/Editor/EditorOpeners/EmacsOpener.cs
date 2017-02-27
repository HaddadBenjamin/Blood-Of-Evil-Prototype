namespace NGToolsEditor.NGConsole
{
	internal sealed class EmacsOpener : IEditorOpener
	{
		public string	defaultArguments { get { return "+$(Line) \"$(File)\" "; } }

		public bool	CanHandleEditor(string editor)
		{
			return editor.Contains("emacs");
		}

		public void	Open(string editorPath, string arguments, string file, int line)
		{
			Utility.OpenFileLine(editorPath, arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()), System.Diagnostics.ProcessWindowStyle.Normal, false);
		}
	}
}