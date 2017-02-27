using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class SingleHandler : TypeHandler
	{
		public	SingleHandler() : base(typeof(Single))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Single);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Single)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadSingle();
		}
	}
}