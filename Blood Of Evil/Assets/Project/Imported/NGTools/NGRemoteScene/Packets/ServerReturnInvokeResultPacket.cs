using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerReturnInvokeResult, true)]
	internal sealed class ServerReturnInvokeResultPacket : Packet
	{
		public string	result;

		private	ServerReturnInvokeResultPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerReturnInvokeResultPacket(string result)
		{
			this.result = result;
		}
	}
}