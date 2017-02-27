using NGTools.Network;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerSendHierarchy)]
	internal sealed class ServerSendHierarchyPacket : Packet
	{
		public readonly List<ServerGameObject>	serverRoot;
		public NetGameObject[]					clientRoot;

		private	ServerSendHierarchyPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendHierarchyPacket(List<ServerGameObject> root)
		{
			this.serverRoot = root;
		}

		public override void	Out(ByteBuffer buffer)
		{
			buffer.Append(this.serverRoot.Count);

			for (int i = this.serverRoot.Count - 1; i >= 0; --i)
				this.BrowseOut(this.serverRoot[i], buffer);
		}

		public override void	In(ByteBuffer buffer)
		{
			int	length = buffer.ReadInt32();

			this.clientRoot = new NetGameObject[length];

			for (int i = 0; i < length; i++)
				this.clientRoot[i] = new NetGameObject(buffer);
		}

		private void	BrowseOut(ServerGameObject node, ByteBuffer buffer)
		{
			buffer.Append(node.gameObject.activeSelf);
			buffer.AppendUnicodeString(node.gameObject.name);
			buffer.Append(node.instanceID);
			buffer.Append(node.children.Count);

			for (int i = 0; i < node.children.Count; i++)
				this.BrowseOut(node.children[i], buffer);
		}
	}
}