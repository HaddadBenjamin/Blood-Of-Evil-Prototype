using NGTools;
using System.Net.Sockets;

namespace NGToolsEditor
{
	public class DefaultTcpClient : AbstractTcpClient
	{
		public override Client	CreateClient(string address, int port)
		{
			TcpClient	tcp = new TcpClient();
			tcp.Connect(address, port);

			return new Client(tcp);
		}
	}
}