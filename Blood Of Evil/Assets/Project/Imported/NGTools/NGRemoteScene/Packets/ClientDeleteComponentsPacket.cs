using NGTools.Network;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientDeleteComponents, true)]
	internal sealed class ClientDeleteComponentsPacket : Packet
	{
		public List<int>	gameObjectInstanceIDs;
		public List<int>	instanceIDs;

		private bool	foldout;

		private	ClientDeleteComponentsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientDeleteComponentsPacket(int gameObjectInstanceID, int instanceID)
		{
			this.gameObjectInstanceIDs = new List<int>();
			this.instanceIDs = new List<int>();

			this.gameObjectInstanceIDs.Add(gameObjectInstanceID);
			this.instanceIDs.Add(instanceID);
		}

		public	ClientDeleteComponentsPacket()
		{
			this.gameObjectInstanceIDs = new List<int>();
			this.instanceIDs = new List<int>();
		}

		public void	Add(int gameObjectInstanceID, int instanceID)
		{
			this.gameObjectInstanceIDs.Add(gameObjectInstanceID);
			this.instanceIDs.Add(instanceID);
		}

		public override void	OnGUI(IUnityData unityData)
		{
			this.foldout = GUILayout.Toggle(this.foldout, "Deleting Components (" + this.instanceIDs.Count + ")");
			if (this.foldout == true)
			{
				for (int i = 0; i < this.gameObjectInstanceIDs.Count; i++)
					GUILayout.Label(unityData.GetGameObjectName(this.gameObjectInstanceIDs[i]) + "." + unityData.GetBehaviourName(this.gameObjectInstanceIDs[i], this.instanceIDs[i]));
			}
		}
	}
}