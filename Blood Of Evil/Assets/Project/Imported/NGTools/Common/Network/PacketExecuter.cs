using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.Network
{
	public class PacketExecuter
	{
		private Dictionary<int, Action<Client, Packet>>	packets;

		protected	PacketExecuter()
		{
			this.packets = new Dictionary<int, Action<Client, Packet>>();
		}

		public void	ExecutePacket(Client sender, Packet packet)
		{
			Action<Client, Packet>	callback;

			if (this.packets.TryGetValue(packet.packetId, out callback) == true)
				callback(sender, packet);
		}

		public void	HandlePacket(int packetId, Action<Client, Packet> callback)
		{
			if (this.packets.ContainsKey(packetId) == true)
				Debug.LogError("Packet with id \"" + packetId + "\" is already handled by " + this.GetType().Name + ".");

			this.packets.Add(packetId, callback);
		}

		public void	UnhandlePacket(int packetId)
		{
			if (this.packets.ContainsKey(packetId) == false)
				Debug.LogError("Packet with id \"" + packetId + "\" is being removed but is not even handled.");

			this.packets.Remove(packetId);
		}
	}
}