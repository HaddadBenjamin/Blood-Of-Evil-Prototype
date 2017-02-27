using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientRequestAllCameras)]
	internal sealed class ClientRequestAllCamerasPacket : Packet
	{
		private	ClientRequestAllCamerasPacket(ByteBuffer buffer) : base(buffer)
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