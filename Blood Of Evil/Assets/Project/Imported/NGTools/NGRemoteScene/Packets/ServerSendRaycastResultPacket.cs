using System.Collections.Generic;
using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Camera_ServerSendRaycastResult)]
	public class ServerSendRaycastResultPacket : Packet
	{
		public int[]	instanceIDs;
		public string[]	names;

		protected	ServerSendRaycastResultPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendRaycastResultPacket(IList<GameObject> gameObjects)
		{
			this.instanceIDs = new int[gameObjects.Count];
			this.names = new string[gameObjects.Count];

			for (int i = 0; i < gameObjects.Count; i++)
			{
				this.instanceIDs[i] = gameObjects[i].GetInstanceID();
				this.names[i] = gameObjects[i].name;
			}
		}
	}
}