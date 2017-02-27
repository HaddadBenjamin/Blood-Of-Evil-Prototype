using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientSendCameraTransformRotation)]
	internal sealed class ClientSendCameraTransformRotationPacket : Packet
	{
		public Vector2	rotation;

		private	ClientSendCameraTransformRotationPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSendCameraTransformRotationPacket(Vector2 rotation)
		{
			this.rotation = rotation;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientSendCameraTransformRotationPacket	packet = pendingPacket as ClientSendCameraTransformRotationPacket;

			if (packet != null && packet.rotation != this.rotation)
			{
				packet.rotation = this.rotation;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Setting camera rotation " + this.rotation + ".");
		}
	}
}