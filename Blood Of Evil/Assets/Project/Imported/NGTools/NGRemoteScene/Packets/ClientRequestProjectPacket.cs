using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestProject)]
	public class ClientRequestProjectPacket : Packet
	{
		protected	ClientRequestProjectPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestProjectPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting Project.");
		}
	}
}