using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Server_ErrorNotification)]
	internal sealed class ErrorNotificationPacket : Packet
	{
		public int		error;
		public string	message;

		private	ErrorNotificationPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ErrorNotificationPacket(int error, string message)
		{
			this.error = error;
			this.message = message;
		}
	}
}