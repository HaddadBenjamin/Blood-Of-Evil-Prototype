using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ClientLoadBigArray)]
	public class ClientLoadBigArrayPacket : Packet
	{
		public string	arrayPath;

		protected	ClientLoadBigArrayPacket(ByteBuffer buffer) : base(buffer)
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