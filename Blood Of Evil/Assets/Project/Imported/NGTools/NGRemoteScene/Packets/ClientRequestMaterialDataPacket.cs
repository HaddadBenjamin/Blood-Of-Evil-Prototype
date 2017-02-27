using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestMaterialData)]
	internal sealed class ClientRequestMaterialDataPacket : Packet
	{
		public int	instanceID;

		private	ClientRequestMaterialDataPacket(ByteBuffer buffer) : base(buffer)
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