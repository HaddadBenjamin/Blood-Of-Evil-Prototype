namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerSendTexture)]
	public class ServerSendTexturePacket : Packet
	{
		public float	time;
		public byte[]	data;

		protected	ServerSendTexturePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendTexturePacket(float time, byte[] data)
		{
			this.time = time;
			this.data = data;
		}
	}
}