using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.CLI_ClientRequestCommandNodes)]
	public class ClientRequestCommandNodesPacket : Packet
	{
		protected	ClientRequestCommandNodesPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestCommandNodesPacket()
		{
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting command nodes.");
		}
	}
}