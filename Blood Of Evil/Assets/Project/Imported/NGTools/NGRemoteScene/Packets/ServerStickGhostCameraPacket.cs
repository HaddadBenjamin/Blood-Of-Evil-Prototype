using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ServerStickGhostCamera)]
	internal sealed class ServerStickGhostCameraPacket : Packet
	{
		public int	ID;

		private	ServerStickGhostCameraPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerStickGhostCameraPacket(int ID)
		{
			this.ID = ID;
		}
	}
}