using NGTools.Network;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerDeleteComponents)]
	internal sealed class ServerDeleteComponentsPacket : Packet
	{
		public List<int>	gameObjectInstanceIDs;
		public List<int>	instanceIDs;

		private	ServerDeleteComponentsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerDeleteComponentsPacket(int gameObjectInstanceID, int instanceID)
		{
			this.gameObjectInstanceIDs = new List<int>();
			this.instanceIDs = new List<int>();

			this.gameObjectInstanceIDs.Add(gameObjectInstanceID);
			this.instanceIDs.Add(instanceID);
		}

		public	ServerDeleteComponentsPacket()
		{
			this.gameObjectInstanceIDs = new List<int>();
			this.instanceIDs = new List<int>();
		}

		public void	Add(int gameObjectInstanceID, int instanceID)
		{
			this.gameObjectInstanceIDs.Add(gameObjectInstanceID);
			this.instanceIDs.Add(instanceID);
		}
	}
}