using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerSendLayers)]
	internal sealed class ServerSendLayersPacket : Packet
	{
		public const int	MaxLayers = 32;

		public string[]	layers;

		private	ServerSendLayersPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendLayersPacket()
		{
		}

		public override void	Out(ByteBuffer buffer)
		{
			for (int i = 0; i < ServerSendLayersPacket.MaxLayers; i++)
			{
				string	layer = LayerMask.LayerToName(i);
				buffer.AppendUnicodeString(layer);
			}
		}

		public override void	In(ByteBuffer buffer)
		{
			this.layers = new string[ServerSendLayersPacket.MaxLayers];

			for (int i = 0; i < ServerSendLayersPacket.MaxLayers; i++)
				this.layers[i] = buffer.ReadUnicodeString();
		}
	}
}