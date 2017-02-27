namespace NGToolsEditor.NGConsole
{
	public interface IEditorOpener
	{
		/// <summary>Overwrites the default arguments in Console Settings when the executable is handled.</summary>
		string	defaultArguments { get; }

		/// <summary>
		/// Checks whether the editor can be handled.
		/// </summary>
		/// <param name="editor">Path of the executable.</param>
		/// <returns></returns>
		bool	CanHandleEditor(string editor);

		/// <summary>
		/// Opens the <paramref name="file"/> at <paramref name="line"/>.
		/// </summary>
		/// <param name="editorPath">The path of the executable.</param>
		/// <param name="arguments">Arguments used by the executable (Given by Console Settings).</param>
		/// <param name="file"></param>
		/// <param name="line"></param>
		void	Open(string editorPath, string arguments, string file, int line);
	}
}