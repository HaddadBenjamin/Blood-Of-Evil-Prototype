using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientRaycastScene)]
	internal sealed class ClientRaycastScenePacket : Packet
	{
		public float	viewportX;
		public float	viewportY;

		private	ClientRaycastScenePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRaycastScenePacket(float viewportX, float viewportY)
		{
			this.viewportX = viewportX;
			this.viewportY = viewportY;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientRaycastScenePacket	packet = pendingPacket as ClientRaycastScenePacket;

			if (packet != null && (packet.viewportX != this.viewportX || packet.viewportY != this.viewportY))
			{
				packet.viewportX = this.viewportX;
				packet.viewportY = this.viewportY;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Raycasting camera viewport (" + this.viewportX + ", " + this.viewportY + ").");
		}
	}
}