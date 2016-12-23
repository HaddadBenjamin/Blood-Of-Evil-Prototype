using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientUpdateMaterialVector2)]
	public class ClientUpdateMaterialVector2Packet : Packet
	{
		public enum Type
		{
			Scale,
			Offset
		}

		public int		instanceID;
		public string	propertyName;
		public Vector2	value;
		public Type		type;

		protected	ClientUpdateMaterialVector2Packet(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientUpdateMaterialVector2Packet(int instanceID, string propertyName, Vector2 value, Type type)
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