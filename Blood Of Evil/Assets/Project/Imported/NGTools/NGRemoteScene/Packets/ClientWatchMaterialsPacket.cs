using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientWatchMaterials)]
	internal sealed class ClientWatchMaterialsPacket : Packet
	{
		public int[]	instanceIDs;

		private	ClientWatchMaterialsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientWatchMaterialsPacket(int[] instanceIDs)
		{
			this.instanceIDs = instanceIDs;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			Utility.sharedBuffer.Length = 0;

			for (int i = 0; i < this.instanceIDs.Length; i++)
			{
				Utility.sharedBuffer.Append(unityData.GetResourceName(typeof(Material), this.instanceIDs[i]));
				Utility.sharedBuffer.Append(", ");
			}

			if (Utility.sharedBuffer.Length > 0)
				Utility.sharedBuffer.Length -= 2;

			GUILayout.Label("Watching Materials " + Utility.sharedBuffer.ToString());
		}
	}
}