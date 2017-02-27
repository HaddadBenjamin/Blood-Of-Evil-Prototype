using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ServerSendTexture)]
	internal sealed class ServerSendTexturePacket : Packet
	{
		public float	time;
		public byte[]	data;

		private	ServerSendTexturePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendTexturePacket(float time, byte[] data)
		{
			this.time = time;
			this.data = data;
		}
	}
}