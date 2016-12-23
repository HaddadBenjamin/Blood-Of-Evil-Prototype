using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestMaterialData)]
	public class ClientRequestMaterialDataPacket : Packet
	{
		public int	instanceID;

		protected	ClientRequestMaterialDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestMaterialDataPacket(int instanceID)
		{
			this.instanceID = instanceID;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting data from Material \"" + unityData.GetResourceName(typeof(Material), this.instanceID) + "\" (" + this.instanceID + ").");
		}
	}
}