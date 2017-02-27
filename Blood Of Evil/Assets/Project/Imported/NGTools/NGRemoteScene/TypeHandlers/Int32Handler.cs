using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class Int32Handler : TypeHandler
	{
		public	Int32Handler() : base(typeof(Int32))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int32);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Int32)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadInt32();
		}
	}
}