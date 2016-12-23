using NGTools;

namespace NGToolsEditor
{
	public abstract class AbstractTcpClient
	{
		public abstract Client	CreateClient(string address, int port);
	}
}