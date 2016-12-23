using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerSendCameraTransform)]
	public class ServerSendCameraTransformPacket : Packet
	{
		public float	positionX;
		public float	positionY;
		public float	positionZ;
		public float	rotationX;
		public float	rotationY;

		protected	ServerSendCameraTransformPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendCameraTransformPacket(Vector3 position, float rotationX, float rotationY)
		{
			this.positionX = position.x;
			this.positionY = position.y;
			this.positionZ = position.z;
			this.rotationX = rotationX;
			this.rotationY = rotationY;
		}
	}
}