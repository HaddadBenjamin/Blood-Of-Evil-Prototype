using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class Int16Handler : TypeHandler
	{
		public	Int16Handler() : base(typeof(Int16))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int16);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Int16)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadInt16();
		}
	}
}