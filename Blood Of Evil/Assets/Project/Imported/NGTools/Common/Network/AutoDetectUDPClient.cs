using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NGTools.Network
{
	public sealed class AutoDetectUDPClient
	{
		public const float		UDPPingInterval = 3F;
		public static byte[]	UDPPingMessage = new byte[] { (byte)'N', (byte)'G', (byte)'S', (byte)'S' };
		public static byte[]	UDPEndMessage = new byte[] { (byte)'E', (byte)'N' };

		private MonoBehaviour	behaviour;

		private UdpClient		Client;
		private IPEndPoint[]	BroadcastEndPoint;
		private float			pingInterval;

#if !UNITY_4_5
		private Coroutine	pingCoroutine;
#endif

		public	AutoDetectUDPClient(MonoBehaviour behaviour, int port, int targetPortMin, int targetPortMax, float pingInterval)
		{
			this.behaviour = behaviour;
			this.pingInterval = pingInterval;

			this.Client = new UdpClient(port);
			this.Client.EnableBroadcast = true;

			this.BroadcastEndPoint = new IPEndPoint[targetPortMax - targetPortMin + 1];

			for (int i = 0; i < this.BroadcastEndPoint.Length; i++)
				this.BroadcastEndPoint[i] = new IPEndPoint(IPAddress.Broadcast, targetPortMin + i);

#if !UNITY_4_5
			this.pingCoroutine =
#endif
			this.behaviour.StartCoroutine(this.AsyncSendPing());
		}

		public void	Stop()
		{
#if UNITY_4_5
			this.behaviour.StopCoroutine("AsyncSendPing");
#else
			this.behaviour.StopCoroutine(this.pingCoroutine);
#endif

			for (int i = 0; i < this.BroadcastEndPoint.Length; i++)
				this.Client.Send(AutoDetectUDPClient.UDPEndMessage, AutoDetectUDPClient.UDPEndMessage.Length, this.BroadcastEndPoint[i]);

			this.Client.Close();
		}

		private IEnumerator	AsyncSendPing()
		{
			AsyncCallback	callback = new AsyncCallback(this.SendPresence);
			WaitForSeconds	wait = new WaitForSeconds(this.pingInterval);

			while (true)
			{
				for (int i = 0; i < this.BroadcastEndPoint.Length; i++)
					this.Client.BeginSend(AutoDetectUDPClient.UDPPingMessage, AutoDetectUDPClient.UDPPingMessage.Length, this.BroadcastEndPoint[i], callback, null);

				yield return wait;
			}
		}

		private void	SendPresence(IAsyncResult ar)
		{
			this.Client.EndSend(ar);
		}
	}
}