using System.Collections.Generic;

namespace NGToolsEditor.NGConsole
{
	public interface IStreams
	{
		List<StreamLog>	Streams { get; }
		int				WorkingStream { get; }

		void			FocusStream(int i);
		void			DeleteStream(int i);
	}
}