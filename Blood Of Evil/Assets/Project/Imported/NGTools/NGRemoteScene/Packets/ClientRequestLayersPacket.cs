using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestLayers)]
	public class ClientRequestLayersPacket : Packet
	{
		protected	ClientRequestLayersPacket(ByteBuffer buffer) : base(buffer)
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