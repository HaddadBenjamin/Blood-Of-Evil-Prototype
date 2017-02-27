using NGTools.Network;

namespace NGToolsEditor.NGConsole
{
	public abstract class AbstractTcpClient
	{
		public abstract Client	CreateClient(string address, int port);
	}
}