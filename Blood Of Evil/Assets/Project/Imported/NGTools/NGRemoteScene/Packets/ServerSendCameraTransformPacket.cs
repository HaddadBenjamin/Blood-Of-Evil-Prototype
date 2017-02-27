using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ServerSendCameraTransform)]
	internal sealed class ServerSendCameraTransformPacket : Packet
	{
		public float	positionX;
		public float	positionY;
		public float	positionZ;
		public float	rotationX;
		public float	rotationY;

		private	ServerSendCameraTransformPacket(ByteBuffer buffer) : base(buffer)
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