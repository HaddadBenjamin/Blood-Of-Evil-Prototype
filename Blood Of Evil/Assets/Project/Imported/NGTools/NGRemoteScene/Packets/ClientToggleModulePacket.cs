using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Camera_ClientToggleModule)]
	internal sealed class ClientToggleModulePacket : Packet
	{
		public byte	moduleID;
		public bool	active;

		private	ClientToggleModulePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientToggleModulePacket(byte moduleID, bool active)
		{
			this.moduleID = moduleID;
			this.active = active;
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientToggleModulePacket	packet = pendingPacket as ClientToggleModulePacket;

			if (packet != null && packet.moduleID == this.moduleID)
			{
				packet.active = this.active;
				return true;
			}

			return false;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			if (this.active == true)
				GUILayout.Label("Activating camera module " + this.moduleID + ".");
			else
				GUILayout.Label("Deactivating camera module " + this.moduleID + ".");
		}
	}
}