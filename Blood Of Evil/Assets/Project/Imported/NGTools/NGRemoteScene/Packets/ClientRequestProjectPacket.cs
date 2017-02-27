using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestProject)]
	internal sealed class ClientRequestProjectPacket : Packet
	{
		private	ClientRequestProjectPacket(ByteBuffer buffer) : base(buffer)
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