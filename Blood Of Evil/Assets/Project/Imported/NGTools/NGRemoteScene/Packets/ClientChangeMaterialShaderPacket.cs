using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientChangeMaterialShader)]
	public class ClientChangeMaterialShaderPacket : Packet
	{
		public int	instanceID;
		public int	shaderInstanceID;

		protected	ClientChangeMaterialShaderPacket(ByteBuffer buffer) : base(buffer)
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