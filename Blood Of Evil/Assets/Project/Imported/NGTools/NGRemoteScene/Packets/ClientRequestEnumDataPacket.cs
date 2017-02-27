using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestEnumData)]
	internal sealed class ClientRequestEnumDataPacket : Packet
	{
		public string	type;

		private	ClientRequestEnumDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestEnumDataPacket(string type)
		{
			this.type = type;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting Enum of type \"" + this.type + "\".");
		}
	}
}