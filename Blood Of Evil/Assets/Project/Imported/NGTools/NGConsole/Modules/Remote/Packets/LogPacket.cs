using NGTools.Network;
using UnityEngine;

namespace NGTools.NGConsole
{
	[PacketLinkTo(PacketId.Logger_ServerSendLog)]
	internal sealed class LogPacket : Packet
	{
		public string	condition;
		public string	stackTrace;
		public LogType	logType;

		private	LogPacket(ByteBuffer buffer) : base(buffer)
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