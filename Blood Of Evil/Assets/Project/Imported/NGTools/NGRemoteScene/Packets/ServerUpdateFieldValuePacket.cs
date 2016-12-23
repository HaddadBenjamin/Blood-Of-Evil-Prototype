namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateFieldValue)]
	public class ServerUpdateFieldValuePacket : Packet
	{
		public string	fieldPath;
		public byte[]	rawValue;

		protected	ServerUpdateFieldValuePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerUpdateFieldValuePacket(string fieldPath, byte[] rawValue)
		{
			this.fieldPath = fieldPath;
			this.rawValue = rawValue;
		}
	}
}