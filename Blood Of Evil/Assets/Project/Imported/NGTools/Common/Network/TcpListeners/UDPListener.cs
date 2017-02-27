using System;
using System.Net;
using System.Net.Sockets;

namespace NGTools.Network
{
	public class UDPListener : NetworkListener
	{
		public UdpClient	client;

		private IPEndPoint	endPoint;
		private IPEndPoint	clientEndPoint;

		public override void	StartServer()
		{
			this.endPoint = new IPEndPoint(IPAddress.Any, this.port);
			this.clientEndPoint = new IPEndPoint(IPAddress.Any, this.port);

			this.client = new UdpClient(this.endPoint);
			this.client.EnableBroadcast = true;
			this.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			this.client.BeginReceive(new AsyncCallback(this.ReceivedPacket), null);
			InternalNGDebug.LogFile("Started UDPListener.");
		}

		public override void	StopServer()
		{
			this.client.Close();
			this.client = null;

			InternalNGDebug.LogFile("Stopped UDPListener.");
		}

		private ByteBuffer	packetBuffer = new ByteBuffer(1920 * 1080 * 3);
		private ByteBuffer	sendBuffer = new ByteBuffer(1920 * 1080 * 3);

		public void	Send(Packet packet)
		{
			this.sendBuffer.Clear();
			this.packetBuffer.Clear();

			packet.Out(this.packetBuffer);
			this.sendBuffer.Append(packet.packetId);
			this.sendBuffer.Append((uint)this.packetBuffer.Length);
			this.sendBuffer.Append(this.packetBuffer);
			//Debug.Log("Sending " + packet.GetType().Name + " of " + this.sendBuffer.Length + " bytes.");

			byte[]	data = this.sendBuffer.Flush();
			this.client.BeginSend(data, data.Length, this.clientEndPoint, new AsyncCallback(this.SendPacket), null);
		}

		private void	SendPacket(IAsyncResult ar)
		{
			this.client.EndSend(ar);
			//Debug.Log("Server sent " + this.client.EndSend(ar) + " bytes.");
		}

		private void	ReceivedPacket(IAsyncResult ar)
		{
			/*byte[]	data = */this.client.EndReceive(ar, ref this.clientEndPoint);
			//Debug.Log("Server received " + data.Length + " bytes.");
			this.client.BeginReceive(new AsyncCallback(this.ReceivedPacket), null);
		}
	}
}