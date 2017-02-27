namespace NGToolsEditor.NGConsole
{
	public interface ILogContentGetter
	{
		string	HeadMessage { get; }
		string	FullMessage { get; }
		string	StackTrace { get; }
		Frame[]	Frames { get; }
		string	Category { get; }
	}
}