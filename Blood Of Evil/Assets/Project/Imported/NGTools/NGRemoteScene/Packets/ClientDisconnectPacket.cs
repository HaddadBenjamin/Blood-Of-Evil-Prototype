using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientDisconnect)]
	public class ClientDisconnectPacket : Packet
	{
		protected	ClientDisconnectPacket(ByteBuffer buffer) : base(buffer)
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