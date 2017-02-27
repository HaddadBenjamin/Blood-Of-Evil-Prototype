using NGTools;
using NGTools.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor;

namespace NGToolsEditor.Network
{
	public sealed class AutoDetectUDPListener
	{
		public sealed class NGServerInstance
		{
			public string	server;
			public double	pingMaxLastTime;
		}
		
		public const double	UDPServerPingLifetime = AutoDetectUDPClient.UDPPingInterval * 3D;

		public  List<NGServerInstance>	NGServerInstances = new List<NGServerInstance>();

		private readonly EditorWindow	window;
		private UdpClient				UDPBroadcastServer;
		private IPEndPoint				clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
		private AsyncCallback			callback;

		public	AutoDetectUDPListener(EditorWindow window, int portMin, int portMax)
		{
			this.window = window;

			this.callback = new AsyncCallback(this.OnUDPBroadcastPacketReceived);

			if (this.UDPBroadcastServer != null && this.UDPBroadcastServer.Client.Connected == true)
				this.UDPBroadcastServer.Close();

			for (int port = portMin; port <= portMax; port++)
			{
				try
				{
					this.UDPBroadcastServer = new UdpClient(port);
					this.UDPBroadcastServer.BeginReceive(this.callback,null);
					break;
				}
				catch (SocketException)
				{
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
					this.UDPBroadcastServer = null;
				}
			}

			Utility.RegisterIntervalCallback(this.CheckNGServersAlive, 200);
		}

		public void	Stop()
		{
			Utility.UnregisterIntervalCallback(this.CheckNGServersAlive);
			if (this.UDPBroadcastServer != null)
				this.UDPBroadcastServer.Close();
		}


		private void	CheckNGServersAlive()
		{
			lock (this.NGServerInstances)
			{
				double	now = Utility.ConvertToUnixTimestamp(DateTime.Now);

				for (int i = 0; i < this.NGServerInstances.Count; i++)
				{
					if (this.NGServerInstances[i].pingMaxLastTime + AutoDetectUDPListener.UDPServerPingLifetime < now)
					{
						//InternalNGDebug.InternalLog(this.NGServerInstances[i].server + " is dead");
						this.NGServerInstances.RemoveAt(i);
						this.window.Repaint();
						--i;
					}
				}
			}
		}

		private void	OnUDPBroadcastPacketReceived(IAsyncResult ar)
		{
			lock (this.clientEndPoint)
			{
				byte[]	data = this.UDPBroadcastServer.EndReceive(ar, ref this.clientEndPoint);
				string	content = this.clientEndPoint.ToString();

				lock (this.NGServerInstances)
				{
					int	n = this.NGServerInstances.FindIndex((s) => s.server == content);

					if (n == -1 && this.IsEqual(data, AutoDetectUDPClient.UDPPingMessage) == true)
					{
						//InternalNGDebug.InternalLog("Add " + content + " (" + BitConverter.ToString(data) + ").");
						this.NGServerInstances.Add(new NGServerInstance() { server = content, pingMaxLastTime = Utility.ConvertToUnixTimestamp(DateTime.Now) + AutoDetectUDPListener.UDPServerPingLifetime });
						EditorApplication.delayCall += this.window.Repaint;
					}
					else
					{
						if (this.IsEqual(data, AutoDetectUDPClient.UDPEndMessage) == true)
						{
							//InternalNGDebug.InternalLog("Kill " + content + " (" + BitConverter.ToString(data) + ").");
							this.NGServerInstances.RemoveAt(n);
							EditorApplication.delayCall += this.window.Repaint;
						}
						else if (this.IsEqual(data, AutoDetectUDPClient.UDPPingMessage) == true)
						{
							//InternalNGDebug.InternalLog("Alive " + content + " (" + BitConverter.ToString(data) + ").");
							this.NGServerInstances[n].pingMaxLastTime = Utility.ConvertToUnixTimestamp(DateTime.Now) + AutoDetectUDPListener.UDPServerPingLifetime;
						}
						else
						{
							InternalNGDebug.InternalLog("Unknown UDP ping (" + BitConverter.ToString(data) + ").");
						}
					}
				}

				this.UDPBroadcastServer.BeginReceive(this.callback, null);
			}
		}

		private bool	IsEqual(byte[] a, byte[] b)
		{
			if (a.Length == b.Length)
			{
				for (int i = 0; i < a.Length; i++)
				{
					if (a[i] != b[i])
						return false;
				}
				return true;
			}
			return false;
		}
	}
}