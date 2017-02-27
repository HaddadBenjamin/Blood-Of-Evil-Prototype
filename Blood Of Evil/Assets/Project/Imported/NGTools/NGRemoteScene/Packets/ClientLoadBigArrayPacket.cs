using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientLoadBigArray, true)]
	internal sealed class ClientLoadBigArrayPacket : Packet
	{
		public string	arrayPath;

		private	ClientLoadBigArrayPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientLoadBigArrayPacket(string arrayPath)
		{
			this.arrayPath = arrayPath;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Loading big array at \"" + this.arrayPath + "\".");
		}
	}
}