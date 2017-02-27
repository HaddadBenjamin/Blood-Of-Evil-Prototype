using System.Collections.Generic;
using System.Net.Sockets;

namespace NGTools.Network
{
	public abstract class AbstractTcpListener : NetworkListener
	{
		//private static byte[]	pollDiscBuffer = new byte[1];

		public int	backLog = 1;

		public List<Client>	clients { get; private set; }

		protected TcpListener	tcpListener;

		private List<Packet>	delayedPackets;

		protected virtual void	Awake()
		{
			this.clients = new List<Client>();
			this.delayedPackets = new List<Packet>();
		}

		protected virtual void	Update()
		{
			if (this.tcpListener == null)
				return;

			for (int i = 0; i < this.clients.Count; i++)
			{
				if (this.DetectClientDisced(this.clients[i]) == true)
				{
					this.clients[i].Close();
					this.clients.RemoveAt(i);
					--i;
					continue;
				}

				this.clients[i].Write();
			}

			if (this.delayedPackets.Count > 0)
			{
				for (int i = 0; i < this.delayedPackets.Count; i++)
					this.BroadcastPacket(this.delayedPackets[i]);
				this.delayedPackets.Clear();
			}

			for (int i = 0; i < this.clients.Count; i++)
				this.clients[i].ExecReceivedCommands(this.server.executer);
		}

		private bool	DetectClientDisced(Client client)
		{
			if (client.tcpClient.Connected == false)
				return true;

			//try
			//{
			//	if (client.tcpClient.Client.Poll(0, SelectMode.SelectRead) == true)
			//	{
			//		if (client.tcpClient.Client.Receive(pollDiscBuffer, SocketFlags.Peek) == 0)
			//			return true;
			//	}
			//}
			//catch (Exception ex)
			//{
			//	InternalNGDebug.Log(Errors.Scene_Exception, ex.Message + Environment.NewLine + ex.StackTrace);
			//}

			return false;
		}

		public override void	StopServer()
		{
			if (this.tcpListener != null)
			{
				this.BroadcastPacket(new ServerHasDisconnectedPacket());

				for (int i = 0; i < this.clients.Count; i++)
				{
					if (this.DetectClientDisced(this.clients[i]) == true)
					{
						this.clients[i].Close();
						this.clients.RemoveAt(i);
						--i;
						continue;
					}

					this.clients[i].Write();
				}

				this.tcpListener.Server.Close();
				this.tcpListener = null;

				InternalNGDebug.LogFile("Stopped AbstractTcpListener.");
			}
		}

		public void	BroadcastPacket(Packet packet)
		{
			for (int i = 0; i < this.clients.Count; i++)
				this.clients[i].AddPacket(packet);
		}

		public void	BroadcastPostPacket(Packet packet)
		{
			for (int i = 0; i < this.clients.Count; i++)
				this.delayedPackets.Add(packet);
		}
	}
}