namespace NGTools.NGRemoteScene
{
	public sealed class NetGameObjectData
	{
		public readonly int				gameObjectInstanceID;
		public readonly string			tag;
		public readonly int				layer;
		public readonly bool			isStatic;
		public readonly NetComponent[]	components;

		public static void	Serialize(ServerGameObject NGGameObject, ByteBuffer buffer)
		{
			NGGameObject.ProcessComponents();

			buffer.Append(NGGameObject.instanceID);
			buffer.AppendUnicodeString(NGGameObject.gameObject.tag);
			buffer.Append(NGGameObject.gameObject.layer);
			buffer.Append(NGGameObject.gameObject.isStatic);
			buffer.Append(NGGameObject.components.Count);

			for (int i = NGGameObject.components.Count - 1; i >= 0; --i)
				NetComponent.Serialize(NGGameObject.components[i], buffer);
		}

		public static NetGameObjectData	Deserialize(ByteBuffer buffer)
		{
			return new NetGameObjectData(buffer);
		}

		private	NetGameObjectData(ByteBuffer buffer)
		{
			this.gameObjectInstanceID = buffer.ReadInt32();
			this.tag = buffer.ReadUnicodeString();
			this.layer = buffer.ReadInt32();
			this.isStatic = buffer.ReadBoolean();

			int	length = buffer.ReadInt32();

			this.components = new NetComponent[length];

			for (int i = 0; i < length; i++)
				this.components[i] = NetComponent.Deserialize(buffer);
		}
	}
}