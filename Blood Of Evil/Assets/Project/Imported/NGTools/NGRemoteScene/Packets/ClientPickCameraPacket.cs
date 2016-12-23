using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientPickCamera)]
	public class ClientPickCameraPacket : Packet
	{
		public int	ID;

		protected	ClientPickCameraPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientPickCameraPacket(int ID)
		{
			this.ID = ID;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Picking camera " + unityData.GetResourceName(typeof(Camera), this.ID) + ".");
		}
	}
}