namespace NGTools.NGGameConsole
{
	public class RemoteCommand : CommandNode
	{
		public override bool		IsLeaf { get { return this.isLeaf; }}

		private bool	isLeaf;

		public	RemoteCommand(ByteBuffer buffer) : base(null, buffer.ReadUnicodeString(), buffer.ReadUnicodeString())
		{
			this.isLeaf = buffer.ReadBoolean();

			int	childrenCount = buffer.ReadInt32();

			for (int i = 0; i < childrenCount; i++)
				this.AddChild(new RemoteCommand(buffer));
		}

		/// <summary>
		/// Generates a full tree from a CommandeNode tree.
		/// </summary>
		/// <param name="node"></param>
		public	RemoteCommand(CommandNode node) : base(null, node.name, node.description)
		{
			this.isLeaf = node.IsLeaf;

			for (int i = 0; i < node.children.Count; i++)
				this.AddChild(new RemoteCommand(node.children[i]));
		}

		public override string	GetSetInvoke(params string[] args)
		{
			return "Waiting...";
		}
	}
}