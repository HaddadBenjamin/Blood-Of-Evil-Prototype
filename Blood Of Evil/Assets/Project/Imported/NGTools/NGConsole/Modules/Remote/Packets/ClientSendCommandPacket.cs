using NGTools.Network;
using UnityEngine;

namespace NGTools.NGConsole
{
	[PacketLinkTo(PacketId.CLI_ClientSendCommand)]
	internal sealed class ClientSendCommandPacket : Packet
	{
		public int		requestId;
		public string	command;

		private	ClientSendCommandPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSendCommandPacket(int requestId, string command)
		{
			this.requestId = requestId;
			this.command = command;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Sending command (" + this.requestId + ") \"" + this.command + "\".");
		}
	}
}