using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientUpdateMaterialProperty)]
	public class ClientUpdateMaterialPropertyPacket : Packet
	{
		public int		instanceID;
		public string	propertyName;
		public byte[]	rawValue;

		protected	ClientUpdateMaterialPropertyPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientUpdateMaterialPropertyPacket(int instanceID, string propertyName, byte[] rawValue)
		{
			this.instanceID = instanceID;
			this.propertyName = propertyName;
			this.rawValue = rawValue;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientUpdateMaterialPropertyPacket	a = pendingPacket as ClientUpdateMaterialPropertyPacket;

			if (a != null &&
				a.instanceID == this.instanceID &&
				a.propertyName.Equals(this.propertyName) == true)
			{
				a.rawValue = this.rawValue;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Updating Material " + unityData.GetResourceName(typeof(Material), this.instanceID) + "." + this.propertyName + ".");
		}
	}
}