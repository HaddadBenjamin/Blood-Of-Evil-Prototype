using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(60)]
	internal sealed class ColorHandler : TypeHandler
	{
		public	ColorHandler() : base(typeof(Color))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Color);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Color	c = (Color)instance;

			buffer.Append(c.r);
			buffer.Append(c.g);
			buffer.Append(c.b);
			buffer.Append(c.a);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Color(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}