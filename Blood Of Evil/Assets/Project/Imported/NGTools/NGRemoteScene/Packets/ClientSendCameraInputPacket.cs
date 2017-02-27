using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientSendCameraInput)]
	internal sealed class ClientSendCameraInputPacket : Packet
	{
		public bool		forward;
		public bool		backward;
		public bool		left;
		public bool		right;
		public float	speed;

		private	ClientSendCameraInputPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSendCameraInputPacket(bool forward, bool backward, bool left, bool right, float speed)
		{
			this.forward = forward;
			this.backward = backward;
			this.left = left;
			this.right = right;
			this.speed = speed;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientSendCameraInputPacket	packet = pendingPacket as ClientSendCameraInputPacket;

			if (packet != null &&
				(packet.forward != this.forward ||
				 packet.backward != this.backward ||
				 packet.left != this.left ||
				 packet.right != this.right ||
				 packet.speed != this.speed))
			{
				packet.forward = this.forward;
				packet.backward = this.backward;
				packet.left = this.left;
				packet.right = this.right;
				packet.speed = this.speed;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Moving camera (F" + this.forward + " B" + this.backward + " L" + this.left + " R" + this.right + " x" + this.speed + ").");
		}
	}
}