using NGTools.Network;
using System;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerSendEnumData)]
	internal sealed class ServerSendEnumDataPacket : Packet
	{
		public string	type;
		public bool		hasFlagAttribute;
		public string[]	names;
		public int[]	values;

		private	ServerSendEnumDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendEnumDataPacket(string type)
		{
			Type	t = Type.GetType(type);

			this.type = type;
			this.hasFlagAttribute = t.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0;
			this.names = Enum.GetNames(t);

			Array	a = Enum.GetValues(t);
			this.values = new int[a.Length];
			for (int i = 0; i < this.values.Length; i++)
				this.values[i] = (int)a.GetValue(i);
		}
	}
}