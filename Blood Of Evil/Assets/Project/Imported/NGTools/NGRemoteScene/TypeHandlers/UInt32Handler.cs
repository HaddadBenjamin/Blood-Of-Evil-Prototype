using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class UInt32Handler : TypeHandler
	{
		public	UInt32Handler() : base(typeof(UInt32))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt32);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((UInt32)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadUInt32();
		}
	}
}