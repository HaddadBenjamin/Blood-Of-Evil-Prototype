using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateMaterialProperty)]
	internal sealed class ServerUpdateMaterialPropertyPacket : Packet
	{
		public int		instanceID;
		public string	propertyName;
		public byte[]	rawValue;

		private	ServerUpdateMaterialPropertyPacket(ByteBuffer buffer) : base(buffer)
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