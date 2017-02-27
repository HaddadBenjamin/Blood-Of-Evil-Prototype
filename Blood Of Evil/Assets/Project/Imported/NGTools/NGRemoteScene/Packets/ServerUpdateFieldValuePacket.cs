using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateFieldValue)]
	internal sealed class ServerUpdateFieldValuePacket : Packet
	{
		public string	fieldPath;
		public byte[]	rawValue;

		private	ServerUpdateFieldValuePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerUpdateFieldValuePacket(string fieldPath, byte[] rawValue)
		{
			this.fieldPath = fieldPath;
			this.rawValue = rawValue;
		}
	}
}