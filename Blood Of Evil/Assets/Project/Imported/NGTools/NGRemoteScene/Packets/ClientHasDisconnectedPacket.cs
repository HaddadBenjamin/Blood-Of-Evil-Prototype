using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.ClientHasDisconnect)]
	public class ClientHasDisconnectedPacket : Packet
	{
		protected	ClientHasDisconnectedPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientHasDisconnectedPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Disconnecting client.");
		}
	}
}