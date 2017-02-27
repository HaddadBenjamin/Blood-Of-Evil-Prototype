using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerUpdateMaterialVector2)]
	internal sealed class ServerUpdateMaterialVector2Packet : Packet
	{
		public int					instanceID;
		public string				propertyName;
		public Vector2				value;
		public MaterialVector2Type	type;

		private	ServerUpdateMaterialVector2Packet(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerUpdateMaterialVector2Packet(int instanceID, string propertyName, Vector2 value, MaterialVector2Type type)
		{
			this.instanceID = instanceID;
			this.propertyName = propertyName;
			this.value = value;
			this.type = type;
		}
	}
}