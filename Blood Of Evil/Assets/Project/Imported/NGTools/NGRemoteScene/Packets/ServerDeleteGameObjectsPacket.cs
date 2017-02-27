using NGTools.Network;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerDeleteGameObjects)]
	internal sealed class ServerDeleteGameObjectsPacket : Packet
	{
		public List<int>	instanceIDs;

		private	ServerDeleteGameObjectsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerDeleteGameObjectsPacket(int instanceID)
		{
			this.instanceIDs = new List<int>();
			this.instanceIDs.Add(instanceID);
		}

		public	ServerDeleteGameObjectsPacket()
		{
			this.instanceIDs = new List<int>();
		}

		/// <summary>
		/// Adds a GameObject to the list of deleted GameObjects.
		/// </summary>
		/// <param name="instanceID"></param>
		public void	Add(int instanceID)
		{
			this.instanceIDs.Add(instanceID);
		}
	}
}