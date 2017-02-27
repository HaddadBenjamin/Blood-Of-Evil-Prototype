namespace NGTools.NGRemoteScene
{
	public sealed class NetGameObject
	{
		public static NetGameObject[]	EmptyArray = {};

		public readonly bool			active;
		public readonly string			name;
		public readonly int				instanceID;
		public readonly NetGameObject[]	children;

		public	NetGameObject(ByteBuffer buffer)
		{
			this.active = buffer.ReadBoolean();
			this.name = buffer.ReadUnicodeString();
			this.instanceID = buffer.ReadInt32();

			int	length = buffer.ReadInt32();

			this.children = new NetGameObject[length];
			for (int i = 0; i < length; i++)
				this.children[i] = new NetGameObject(buffer);
		}
	}
}