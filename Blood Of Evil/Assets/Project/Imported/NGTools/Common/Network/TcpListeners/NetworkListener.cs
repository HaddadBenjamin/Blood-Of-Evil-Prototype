using UnityEngine;

namespace NGTools.Network
{
	public abstract class NetworkListener : MonoBehaviour
	{
		public int	port;

		protected BaseServer	server;

		public void	SetServer(BaseServer server)
		{
			this.server = server;
		}

		public abstract void	StartServer();
		public abstract void	StopServer();
	}
}