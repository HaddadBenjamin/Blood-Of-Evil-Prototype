using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientWatchGameObjects)]
	public class ClientWatchGameObjectsPacket : Packet
	{
		public int[]	instanceIDs;

		protected	ClientWatchGameObjectsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientWatchGameObjectsPacket(int[] instanceIDs)
		{
			this.instanceIDs = instanceIDs;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			Utility.sharedBuffer.Length = 0;

			for (int i = 0; i < this.instanceIDs.Length; i++)
			{
				Utility.sharedBuffer.Append(unityData.GetGameObjectName(this.instanceIDs[i]));
				Utility.sharedBuffer.Append(", ");
			}

			if (Utility.sharedBuffer.Length > 0)
				Utility.sharedBuffer.Length -= 2;

			GUILayout.Label("Watching GameObjects " + Utility.sharedBuffer.ToString());
		}
	}
}