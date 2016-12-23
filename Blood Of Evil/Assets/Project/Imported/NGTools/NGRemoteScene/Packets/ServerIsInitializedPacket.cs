using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerIsInitialized)]
	public class ServerIsInitializedPacket : Packet
	{
		public int						width;
		public int						height;
		public int						depth;
		public RenderTextureFormat		format;
		public byte[]					modules;

		protected	ServerIsInitializedPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerIsInitializedPacket(int width, int height, int depth, RenderTextureFormat format, byte[] modules)
		{
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			this.modules = modules;
		}
	}
}