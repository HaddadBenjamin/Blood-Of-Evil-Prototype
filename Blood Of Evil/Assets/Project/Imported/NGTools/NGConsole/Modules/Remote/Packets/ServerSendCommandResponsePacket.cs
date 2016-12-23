namespace NGTools
{
	[PacketLinkTo(PacketId.CLI_ServerSendCommandResponse)]
	public class ServerSendCommandResponsePacket : Packet
	{
		public int			requestId;
		public ExecResult	returnValue;
		public string		response;

		protected	ServerSendCommandResponsePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendCommandResponsePacket(int requestId, ExecResult returnValue, string response)
		{
			this.requestId = requestId;
			this.returnValue = returnValue;
			this.response = response;
		}
	}
}