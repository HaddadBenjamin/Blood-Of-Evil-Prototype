namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerSendCameraData)]
	public class CameraDataPacket : Packet
	{
		public byte		moduleID;
		public float	time;
		public byte[]	data;

		protected	CameraDataPacket(ByteBuffer buffer) : base(buffer)
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