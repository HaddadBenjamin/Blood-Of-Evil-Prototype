using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ServerSendCameraData)]
	internal sealed class CameraDataPacket : Packet
	{
		public byte		moduleID;
		public float	time;
		public byte[]	data;

		private	CameraDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	CameraDataPacket(byte moduleID, float time, byte[] data)
		{
			this.moduleID = moduleID;
			this.time = time;
			this.data = data;
		}
	}
}