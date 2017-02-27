using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientSetSibling)]
	internal sealed class ClientSetSiblingPacket : Packet
	{
		public int	instanceID;
		public int	instanceIDParent;
		public int	siblingIndex;

		private	ClientSetSiblingPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientSetSiblingPacket(int instanceID, int instanceIDParent, int siblingIndex)
		{
			this.instanceID = instanceID;
			this.instanceIDParent = instanceIDParent;
			this.siblingIndex = siblingIndex;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Moving GameObject \"" + unityData.GetGameObjectName(this.instanceID) + "\" into \"" + unityData.GetGameObjectName(this.instanceIDParent) + "\" at position \"" + this.siblingIndex + "\".");
		}
	}
}