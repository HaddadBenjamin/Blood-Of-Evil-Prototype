using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// <para>Sends all fields and properties from a Component + enable.</para>
	/// </summary>
	[PacketLinkTo(PacketId.Scene_ServerSendComponent)]
	internal sealed class ServerSendComponentPacket : Packet
	{
		public readonly ServerComponent	serverComponent;
		public int						gameObjectInstanceID;
		public NetComponent				component;

		private	ServerSendComponentPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendComponentPacket(int gameObjectInstanceID, ServerComponent serverComponent)
		{
			this.gameObjectInstanceID = gameObjectInstanceID;
			this.serverComponent = serverComponent;
		}

		public override void	Out(ByteBuffer buffer)
		{
			buffer.Append(this.gameObjectInstanceID);
			NetComponent.Serialize(this.serverComponent, buffer);
		}

		public override void	In(ByteBuffer buffer)
		{
			this.gameObjectInstanceID = buffer.ReadInt32();
			this.component = NetComponent.Deserialize(buffer);
		}
	}
}