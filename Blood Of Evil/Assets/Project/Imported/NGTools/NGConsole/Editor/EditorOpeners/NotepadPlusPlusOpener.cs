namespace NGToolsEditor.NGConsole
{
	internal sealed class NotepadPlusPlusOpener : IEditorOpener
	{
		public string	defaultArguments { get { return "\"$(File)\" -n$(Line)"; } }

		public bool	CanHandleEditor(string editor)
		{
			return editor.Contains("notepad");
		}

		public void	Open(string editorPath, string arguments, string file, int line)
		{
			Utility.OpenFileLine(editorPath, arguments.Replace("$(File)", file).Replace("$(Line)", line.ToString()));
		}
	}
}