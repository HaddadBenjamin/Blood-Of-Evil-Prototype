using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientConnect)]
	public class ClientConnectPacket : Packet
	{
		public int					width;
		public int					height;
		public int					depth;
		public int					targetRefresh;
		public RenderTextureFormat	format;
		public byte[]				modulesAvailable;

		protected	ClientConnectPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientConnectPacket(int width, int height, int depth, int targetRefresh, RenderTextureFormat format, byte[] modulesAvailable)
		{
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.targetRefresh = targetRefresh;
			this.format = format;
			this.modulesAvailable = modulesAvailable;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Connecting NG Camera.");
		}
	}
}