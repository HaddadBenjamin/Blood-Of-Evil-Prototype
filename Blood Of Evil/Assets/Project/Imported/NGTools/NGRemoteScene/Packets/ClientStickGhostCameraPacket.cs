using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientStickGhostCamera)]
	internal sealed class ClientStickGhostCameraPacket : Packet
	{
		public int	ID;

		private	ClientStickGhostCameraPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientStickGhostCameraPacket(int ID)
		{
			this.ID = ID;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Sticking ghost camera to " + unityData.GetResourceName(typeof(Transform), this.ID) + ".");
		}
	}
}