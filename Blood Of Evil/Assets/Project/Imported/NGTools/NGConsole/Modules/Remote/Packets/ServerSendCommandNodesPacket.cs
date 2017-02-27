using NGTools.Network;
using NGTools.NGGameConsole;

namespace NGTools.NGConsole
{
	[PacketLinkTo(PacketId.CLI_ServerSendCommandNodes)]
	internal sealed class ServerSendCommandNodesPacket : Packet
	{
		public RemoteCommand	root;

		private	ServerSendCommandNodesPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendCommandNodesPacket(RemoteCommand root)
		{
			this.root = root;
		}

		public override void	Out(ByteBuffer buffer)
		{
			this.BrowseOut(buffer, this.root);
		}

		public override void	In(ByteBuffer buffer)
		{
			this.root = this.BrowseIn(buffer);
		}

		private void	BrowseOut(ByteBuffer buffer, CommandNode node)
		{
			buffer.AppendUnicodeString(node.name);
			buffer.AppendUnicodeString(node.description);
			buffer.Append(node.IsLeaf);
			buffer.Append(node.children.Count);

			for (int i = 0; i < node.children.Count; i++)
				this.BrowseOut(buffer, node.children[i]);
		}

		private RemoteCommand	BrowseIn(ByteBuffer buffer)
		{
			return new RemoteCommand(buffer);
		}
	}
}