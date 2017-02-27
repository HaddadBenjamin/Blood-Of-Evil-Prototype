using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ServerSendAllCameras)]
	internal sealed class ServerSendAllCamerasPacket : Packet
	{
		public int[]	IDs;
		public string[]	names;
		public int		ghostCameraId;

		private	ServerSendAllCamerasPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendAllCamerasPacket(Camera[] cameras, int ghostCameraId)
		{
			this.IDs = new int[cameras.Length];
			this.names = new string[cameras.Length];
			this.ghostCameraId = ghostCameraId;

			for (int i = 0; i < cameras.Length; i++)
			{
				this.IDs[i] = cameras[i].GetInstanceID();
				this.names[i] = cameras[i].name;
			}
		}
	}
}