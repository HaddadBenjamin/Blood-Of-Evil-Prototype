namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateMaterialProperty)]
	public class ServerUpdateMaterialPropertyPacket : Packet
	{
		public int		instanceID;
		public string	propertyName;
		public byte[]	rawValue;

		protected	ServerUpdateMaterialPropertyPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerUpdateMaterialPropertyPacket(int instanceID, string propertyName, byte[] rawValue)
		{
			this.instanceID = instanceID;
			this.propertyName = propertyName;
			this.rawValue = rawValue;
		}
	}
}