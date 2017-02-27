using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestHierarchy)]
	internal sealed class ClientRequestHierarchyPacket : Packet
	{
		private	ClientRequestHierarchyPacket(ByteBuffer buffer) : base(buffer)
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