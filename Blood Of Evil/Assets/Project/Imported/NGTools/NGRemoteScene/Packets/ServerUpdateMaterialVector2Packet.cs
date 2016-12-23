using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateMaterialVector2)]
	public class ServerUpdateMaterialVector2Packet : Packet
	{
		public int										instanceID;
		public string									propertyName;
		public Vector2									value;
		public ClientUpdateMaterialVector2Packet.Type	type;

		protected	ServerUpdateMaterialVector2Packet(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerUpdateMaterialVector2Packet(int instanceID, string propertyName, Vector2 value, ClientUpdateMaterialVector2Packet.Type type)
		{
			this.instanceID = instanceID;
			this.propertyName = propertyName;
			this.value = value;
			this.type = type;
		}
	}
}