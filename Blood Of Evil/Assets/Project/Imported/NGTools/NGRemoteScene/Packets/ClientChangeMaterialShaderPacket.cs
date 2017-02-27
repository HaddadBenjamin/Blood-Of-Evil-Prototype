using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientChangeMaterialShader, true)]
	internal sealed class ClientChangeMaterialShaderPacket : Packet
	{
		public int	instanceID;
		public int	shaderInstanceID;

		private	ClientChangeMaterialShaderPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientChangeMaterialShaderPacket(int instanceID, int shaderInstanceID)
		{
			this.instanceID = instanceID;
			this.shaderInstanceID = shaderInstanceID;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Changing material " + this.instanceID + " with shader " + this.shaderInstanceID + ".");
		}
	}
}