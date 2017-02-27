namespace NGToolsEditor.NGConsole
{
	internal sealed class SublimeOpener : IEditorOpener
	{
		public string	defaultArguments { get { return "\"$(File):$(Line)\""; } }

		public bool	CanHandleEditor(string editor)
		{
			return editor.Contains("sublime");
		}

		public void	Open(string editorPath, string arguments, string file, int line)
		{
			Utility.OpenFileLine(editorPath, arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()));
		}
	}
}