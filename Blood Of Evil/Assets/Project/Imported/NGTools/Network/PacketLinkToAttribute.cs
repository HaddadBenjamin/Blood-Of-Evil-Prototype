using System;

namespace NGTools
{
	[AttributeUsage(AttributeTargets.Class)]
	public class PacketLinkToAttribute : Attribute
	{
		public readonly	int	packetId;

		public	PacketLinkToAttribute(int packetId)
		{
			this.packetId = packetId;
		}
	}
}