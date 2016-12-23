namespace NGTools
{
	[PacketLinkTo(PacketId.ServerHasDisconnect)]
	public class ServerHasDisconnectedPacket : Packet
	{
		protected	ServerHasDisconnectedPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerHasDisconnectedPacket()
		{
		}
	}
}