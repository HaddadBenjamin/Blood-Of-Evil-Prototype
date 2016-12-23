﻿using System;

namespace NGTools
{
	[Priority(0)]
	public class ByteHandler : TypeHandler
	{
		public	ByteHandler() : base(typeof(Byte))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Byte);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.Append((Byte)instance);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadByte();
		}
	}
}