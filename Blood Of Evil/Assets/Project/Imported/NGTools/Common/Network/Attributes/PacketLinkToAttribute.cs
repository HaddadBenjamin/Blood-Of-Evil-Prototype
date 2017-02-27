using System;

namespace NGTools.Network
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class PacketLinkToAttribute : Attribute
	{
		public readonly	int		packetId;
		public readonly	bool	isBatchable;

		public	PacketLinkToAttribute(int packetId, bool isBatchable = false)
		{
			this.packetId = packetId;
			this.isBatchable = isBatchable;
		}
	}
}