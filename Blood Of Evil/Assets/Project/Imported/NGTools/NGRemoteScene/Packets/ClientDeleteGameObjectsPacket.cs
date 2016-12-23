using System.Collections.Generic;
using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientDeleteGameObjects)]
	public class ClientDeleteGameObjectsPacket : Packet
	{
		public List<int>	instanceIDs;

		private bool	foldout;

		protected	ClientDeleteGameObjectsPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientDeleteGameObjectsPacket(int instanceID)
		{
			this.instanceIDs = new List<int>();
			this.instanceIDs.Add(instanceID);
		}

		public	ClientDeleteGameObjectsPacket()
		{
			this.instanceIDs = new List<int>();
		}

		public void	Add(int instanceID)
		{
			this.instanceIDs.Add(instanceID);
		}

		public override void	OnGUI(IUnityData unityData)
		{
			this.foldout = GUILayout.Toggle(this.foldout, "Deleting Game Objects (" + this.instanceIDs.Count + ")");
			if (this.foldout == true)
			{
				for (int i = 0; i < this.instanceIDs.Count; i++)
					GUILayout.Label(unityData.GetGameObjectName(this.instanceIDs[i]));
			}
		}
	}
}