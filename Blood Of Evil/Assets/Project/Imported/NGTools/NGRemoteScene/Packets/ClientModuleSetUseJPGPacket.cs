using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientModuleSetUseJPG)]
	internal sealed class ClientModuleSetUseJPGPacket : Packet
	{
		public bool	useJPG;

		private	ClientModuleSetUseJPGPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientModuleSetUseJPGPacket(bool useJPG)
		{
			this.useJPG = useJPG;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientModuleSetUseJPGPacket	packet = pendingPacket as ClientModuleSetUseJPGPacket;

			if (packet != null)
			{
				packet.useJPG = this.useJPG;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			if (this.useJPG == true)
				GUILayout.Label("Camera selected JPG format.");
			else
				GUILayout.Label("Camera selected PNG format.");
		}
	}
}