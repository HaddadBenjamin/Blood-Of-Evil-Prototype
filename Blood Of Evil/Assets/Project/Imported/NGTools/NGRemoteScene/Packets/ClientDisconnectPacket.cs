using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientDisconnect)]
	internal sealed class ClientDisconnectPacket : Packet
	{
		private	ClientDisconnectPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientDisconnectPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Disconnecting NG Camera.");
		}
	}
}