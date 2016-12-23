using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestEnumData)]
	public class ClientRequestEnumDataPacket : Packet
	{
		public string	type;

		protected	ClientRequestEnumDataPacket(ByteBuffer buffer) : base(buffer)
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