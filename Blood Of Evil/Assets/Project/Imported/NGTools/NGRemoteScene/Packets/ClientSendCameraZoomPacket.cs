using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientSendCameraZoom)]
	public class ClientSendCameraZoomPacket : Packet
	{
		public float	factor;

		protected	ClientSendCameraZoomPacket(ByteBuffer buffer) : base(buffer)
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