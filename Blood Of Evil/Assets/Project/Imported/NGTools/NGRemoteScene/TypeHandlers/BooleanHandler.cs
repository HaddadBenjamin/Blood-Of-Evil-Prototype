using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class BooleanHandler : TypeHandler
	{
		public	BooleanHandler() : base(typeof(Boolean))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Boolean);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Boolean)instance);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadBoolean();
		}
	}
}