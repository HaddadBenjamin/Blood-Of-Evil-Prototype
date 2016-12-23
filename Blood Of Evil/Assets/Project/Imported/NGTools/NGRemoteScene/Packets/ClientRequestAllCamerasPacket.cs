using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientRequestAllCameras)]
	public class ClientRequestAllCamerasPacket : Packet
	{
		protected	ClientRequestAllCamerasPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestAllCamerasPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting all cameras.");
		}
	}
}