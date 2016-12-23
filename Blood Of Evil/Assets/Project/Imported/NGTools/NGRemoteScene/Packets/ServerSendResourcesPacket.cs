﻿using System;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ServerSendResources)]
	public class ServerSendResourcesPacket : Packet
	{
		public Type		type;
		public string[]	resourceNames;
		public int[]	instanceIDs;

		protected	ServerSendResourcesPacket(ByteBuffer buffer) : base(buffer)
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