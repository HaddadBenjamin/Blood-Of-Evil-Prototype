using System;

namespace NGTools.NGRemoteScene
{
	[Priority(40)]
	internal sealed class CharHandler : TypeHandler
	{
		public	CharHandler() : base(typeof(Char))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Char);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Char)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadChar();
		}
	}
}