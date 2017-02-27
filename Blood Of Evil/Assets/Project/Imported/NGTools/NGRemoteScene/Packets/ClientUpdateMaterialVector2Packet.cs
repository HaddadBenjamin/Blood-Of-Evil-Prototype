using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public enum MaterialVector2Type
	{
		Scale,
		Offset
	}

	[PacketLinkTo(PacketId.Scene_ClientUpdateMaterialVector2, true)]
	internal sealed class ClientUpdateMaterialVector2Packet : Packet
	{
		public int		instanceID;
		public string	propertyName;
		public Vector2	value;
		public MaterialVector2Type		type;

		private	ClientUpdateMaterialVector2Packet(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientUpdateMaterialVector2Packet(int instanceID, string propertyName, Vector2 value, MaterialVector2Type type)
		{
			this.instanceID = instanceID;
			this.propertyName = propertyName;
			this.value = value;
			this.type = type;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientUpdateMaterialVector2Packet	a = pendingPacket as ClientUpdateMaterialVector2Packet;

			if (a != null &&
				a.instanceID == this.instanceID &&
				a.type == this.type &&
				a.propertyName.Equals(this.propertyName) == true)
			{
				a.value = this.value;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Updating Material " + unityData.GetResourceName(typeof(Material), this.instanceID) + "." + this.propertyName + "." + this.type + " (" + this.value + ").");
		}
	}
}