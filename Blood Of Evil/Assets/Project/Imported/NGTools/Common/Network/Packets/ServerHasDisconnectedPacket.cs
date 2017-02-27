namespace NGTools.Network
{
	[PacketLinkTo(PacketId.ServerHasDisconnect)]
	internal sealed class ServerHasDisconnectedPacket : Packet
	{
		private	ServerHasDisconnectedPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerHasDisconnectedPacket()
		{
		}
	}
}