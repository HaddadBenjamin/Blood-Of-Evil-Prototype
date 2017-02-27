using NGTools.Network;
using System;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerSendResources)]
	internal sealed class ServerSendResourcesPacket : Packet
	{
		public Type		type;
		public string[]	resourceNames;
		public int[]	instanceIDs;

		private	ServerSendResourcesPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendResourcesPacket(Type type, string[] resourceNames, int[] instanceID)
		{
			this.type = type;
			this.resourceNames = resourceNames;
			this.instanceIDs = instanceID;
		}
	}
}