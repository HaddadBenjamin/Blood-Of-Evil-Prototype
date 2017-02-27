using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientPickGhostCameraAtCamera)]
	internal sealed class ClientPickGhostCameraAtCameraPacket : Packet
	{
		public int	ID;

		private	ClientPickGhostCameraAtCameraPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientPickGhostCameraAtCameraPacket(int ID)
		{
			this.ID = ID;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Picking ghost camera at the position of camera " + unityData.GetResourceName(typeof(Camera), this.ID) + ".");
		}
	}
}