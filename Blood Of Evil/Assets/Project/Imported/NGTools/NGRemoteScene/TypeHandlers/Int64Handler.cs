#if UNITY_5
using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class Int64Handler : TypeHandler
	{
		public	Int64Handler() : base(typeof(Int64))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Int64);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Int64)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadInt64();
		}
	}
}
#endif