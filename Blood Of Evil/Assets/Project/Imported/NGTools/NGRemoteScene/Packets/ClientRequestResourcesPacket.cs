using NGTools.Network;
using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientRequestResources)]
	internal sealed class ClientRequestResourcesPacket : Packet
	{
		public Type	type;

		private	ClientRequestResourcesPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestResourcesPacket(Type type)
		{
			this.type = type;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting resources of type \"" + this.type.Name + "\".");
		}
	}
}