using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientInvokeBehaviourMethod, true)]
	internal sealed class ClientInvokeBehaviourMethodPacket : Packet
	{
		public int		gameObjectInstanceID;
		public int		instanceID;
		public string	methodSignature;
		public byte[]	arguments;

		private	ClientInvokeBehaviourMethodPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientInvokeBehaviourMethodPacket(int gameObjectInstanceID, int instanceID, string methodSignature, byte[] arguments)
		{
			this.gameObjectInstanceID = gameObjectInstanceID;
			this.instanceID = instanceID;
			this.methodSignature = methodSignature;
			this.arguments = arguments;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Invoking " + unityData.GetGameObjectName(this.gameObjectInstanceID) + "." + unityData.GetBehaviourName(this.gameObjectInstanceID, this.instanceID) + "." + this.methodSignature);
		}
	}
}