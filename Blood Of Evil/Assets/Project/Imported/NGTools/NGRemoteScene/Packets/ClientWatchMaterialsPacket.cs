using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientWatchMaterials)]
	public class ClientWatchMaterialsPacket : Packet
	{
		public int[]	instanceIDs;

		protected	ClientWatchMaterialsPacket(ByteBuffer buffer) : base(buffer)
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