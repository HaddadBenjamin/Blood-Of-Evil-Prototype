using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientPickCamera)]
	internal sealed class ClientPickCameraPacket : Packet
	{
		public int	ID;

		private	ClientPickCameraPacket(ByteBuffer buffer) : base(buffer)
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