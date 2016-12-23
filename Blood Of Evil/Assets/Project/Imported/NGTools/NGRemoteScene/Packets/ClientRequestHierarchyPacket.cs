using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestHierarchy)]
	public class ClientRequestHierarchyPacket : Packet
	{
		protected	ClientRequestHierarchyPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestHierarchyPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting Hierarchy.");
		}
	}
}