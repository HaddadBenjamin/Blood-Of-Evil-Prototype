using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NGTools.Network
{
	public class DefaultTcpListener : AbstractTcpListener
	{
		public override void	StartServer()
		{
			if (this.tcpListener != null)
				return;

			try
			{
				this.tcpListener = new TcpListener(IPAddress.Any, this.port);
				this.tcpListener.Start(this.backLog);
				this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptClient), null);

				InternalNGDebug.LogFile("Started TCPListener IPAddress.Any:" + this.port);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		private void	AcceptClient(IAsyncResult ar)
		{
			// TcpListener is null when the server stops.
			if (this.tcpListener == null)
				return;

			Client	client = new Client(this.tcpListener.EndAcceptTcpClient(ar), Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor);
			this.clients.Add(client);

			InternalNGDebug.LogFile("Accepted Client " + client.tcpClient.Client.RemoteEndPoint);

			this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptClient), null);
		}
	}
}