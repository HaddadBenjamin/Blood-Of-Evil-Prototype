using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class UInt16Handler : TypeHandler
	{
		public	UInt16Handler() : base(typeof(UInt16))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt16);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((UInt16)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadUInt16();
		}
	}
}