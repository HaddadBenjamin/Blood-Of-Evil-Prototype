#if UNITY_5
using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class UInt64Handler : TypeHandler
	{
		public	UInt64Handler() : base(typeof(UInt64))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(UInt64);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((UInt64)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadUInt64();
		}
	}
}
#endif