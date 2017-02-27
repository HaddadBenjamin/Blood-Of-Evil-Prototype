using NGTools.Network;
using NGTools.NGGameConsole;

namespace NGTools.NGConsole
{
	[PacketLinkTo(PacketId.CLI_ServerSendCommandResponse)]
	internal sealed class ServerSendCommandResponsePacket : Packet
	{
		public int			requestId;
		public ExecResult	returnValue;
		public string		response;

		private	ServerSendCommandResponsePacket(ByteBuffer buffer) : base(buffer)
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