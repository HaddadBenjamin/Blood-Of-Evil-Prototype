using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerStickGhostCamera)]
	public class ServerStickGhostCameraPacket : Packet
	{
		public int	ID;

		protected	ServerStickGhostCameraPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerStickGhostCameraPacket(int ID)
		{
			this.ID = ID;
		}
	}
	[PacketLinkTo(PacketId.Camera_ClientStickGhostCamera)]
	public class ClientStickGhostCameraPacket : Packet
	{
		public int	ID;

		protected	ClientStickGhostCameraPacket(ByteBuffer buffer) : base(buffer)
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