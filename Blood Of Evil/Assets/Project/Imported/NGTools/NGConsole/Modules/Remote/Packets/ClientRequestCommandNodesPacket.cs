using NGTools.Network;
using UnityEngine;

namespace NGTools.NGConsole
{
	[PacketLinkTo(PacketId.CLI_ClientRequestCommandNodes)]
	internal sealed class ClientRequestCommandNodesPacket : Packet
	{
		private	ClientRequestCommandNodesPacket(ByteBuffer buffer) : base(buffer)
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