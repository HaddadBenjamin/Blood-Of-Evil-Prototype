using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientPickGhostCameraAtCamera)]
	public class ClientPickGhostCameraAtCameraPacket : Packet
	{
		public int	ID;

		protected	ClientPickGhostCameraAtCameraPacket(ByteBuffer buffer) : base(buffer)
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