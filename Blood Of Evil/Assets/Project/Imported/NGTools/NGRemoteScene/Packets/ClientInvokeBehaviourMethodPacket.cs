using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientInvokeBehaviourMethod)]
	public class ClientInvokeBehaviourMethodPacket : Packet
	{
		public int	gameObjectInstanceID;
		public int	instanceID;
		public string	method;
		public byte[]	arguments;

		protected	ClientInvokeBehaviourMethodPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientInvokeBehaviourMethodPacket(int gameObjectInstanceID, int instanceID, string method, byte[] arguments)
		{
			this.gameObjectInstanceID = gameObjectInstanceID;
			this.instanceID = instanceID;
			this.method = method;
			this.arguments = arguments;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Invoking " + unityData.GetGameObjectName(this.gameObjectInstanceID) + "." + unityData.GetBehaviourName(this.gameObjectInstanceID, this.instanceID) + "." + this.method);
		}
	}
}