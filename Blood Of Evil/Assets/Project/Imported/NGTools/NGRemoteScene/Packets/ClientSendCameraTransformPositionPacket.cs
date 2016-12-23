using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ClientSendCameraTransformPosition)]
	public class ClientSendCameraTransformPositionPacket : Packet
	{
		public Vector3	position;

		protected	ClientSendCameraTransformPositionPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSendCameraTransformPositionPacket(Vector3 position)
		{
			this.position = position;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientSendCameraTransformPositionPacket	packet = pendingPacket as ClientSendCameraTransformPositionPacket;

			if (packet != null && packet.position != this.position)
			{
				packet.position = this.position;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Setting camera position " + this.position + ".");
		}
	}
}