using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.CLI_ClientSendCommand)]
	public class ClientSendCommandPacket : Packet
	{
		public int		requestId;
		public string	command;

		protected	ClientSendCommandPacket(ByteBuffer buffer) : base(buffer)
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