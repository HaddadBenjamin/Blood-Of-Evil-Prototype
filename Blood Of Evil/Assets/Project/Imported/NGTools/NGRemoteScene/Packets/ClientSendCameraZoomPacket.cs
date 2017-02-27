using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientSendCameraZoom)]
	internal sealed class ClientSendCameraZoomPacket : Packet
	{
		public float	factor;

		private	ClientSendCameraZoomPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSendCameraZoomPacket(float factor)
		{
			this.factor = factor;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Zooming camera (" + this.factor + ").");
		}
	}
}