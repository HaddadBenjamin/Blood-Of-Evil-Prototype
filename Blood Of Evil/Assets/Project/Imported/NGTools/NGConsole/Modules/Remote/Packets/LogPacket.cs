using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Logger_ServerSendLog)]
	public class LogPacket : Packet
	{
		public string	condition;
		public string	stackTrace;
		public LogType	logType;

		protected	LogPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	LogPacket(string condition, string stackTrace, LogType logType)
		{
			this.condition = condition;
			this.stackTrace = stackTrace;
			this.logType = logType;
		}
	}
}