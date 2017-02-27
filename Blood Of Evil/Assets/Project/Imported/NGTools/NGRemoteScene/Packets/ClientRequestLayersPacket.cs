using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestLayers)]
	internal sealed class ClientRequestLayersPacket : Packet
	{
		private	ClientRequestLayersPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestLayersPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting Layers.");
		}
	}
}