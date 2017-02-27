#if UNITY_5
using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class DoubleHandler : TypeHandler
	{
		public	DoubleHandler() : base(typeof(Double))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Double);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Double)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadDouble();
		}
	}
}
#endif