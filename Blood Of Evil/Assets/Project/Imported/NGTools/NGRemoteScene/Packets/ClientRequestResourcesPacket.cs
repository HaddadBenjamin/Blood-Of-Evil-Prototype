using System;
using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientRequestResources)]
	public class ClientRequestResourcesPacket : Packet
	{
		public Type	type;

		protected	ClientRequestResourcesPacket(ByteBuffer buffer) : base(buffer)
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