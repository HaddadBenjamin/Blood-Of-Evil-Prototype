namespace NGTools
{
	/// <summary>
	/// <para>Sends all primary data of a GameObject.</para>
	/// </summary>
	/// <seealso cref="NGTools.ClientRequestGameObjectDataPacket"/>
	[PacketLinkTo(PacketId.Scene_ServerSendGameObjectData)]
	public class ServerSendGameObjectDataPacket : Packet
	{
		public readonly ServerGameObject	serverGameObject;
		public NetGameObjectData			gameObjectData;

		protected	ServerSendGameObjectDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendGameObjectDataPacket(ServerGameObject NGGameObject)
		{
			this.serverGameObject = NGGameObject;
		}

		public override void	Out(ByteBuffer buffer)
		{
			NetGameObjectData.Serialize(this.serverGameObject, buffer);
		}

		public override void	In(ByteBuffer buffer)
		{
			this.gameObjectData = NetGameObjectData.Deserialize(buffer);
		}
	}
}