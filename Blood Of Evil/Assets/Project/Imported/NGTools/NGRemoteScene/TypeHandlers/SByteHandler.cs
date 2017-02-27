using System;

namespace NGTools.NGRemoteScene
{
	[Priority(0)]
	internal sealed class SByteHandler : TypeHandler
	{
		public	SByteHandler() : base(typeof(SByte))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(SByte);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((SByte)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadSByte();
		}
	}
}