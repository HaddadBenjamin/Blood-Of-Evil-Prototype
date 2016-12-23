namespace NGTools
{
	[PacketLinkTo(PacketId.Server_ErrorNotification)]
	public class ErrorNotificationPacket : Packet
	{
		public int		error;
		public string	message;

		protected	ErrorNotificationPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ErrorNotificationPacket(int error, string message)
		{
			this.error = error;
			this.message = message;
		}
	}
}