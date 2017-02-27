using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(60)]
	internal sealed class RectHandler : TypeHandler
	{
		public	RectHandler() : base(typeof(Rect))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Rect);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Rect	v = (Rect)instance;

			buffer.Append(v.x);
			buffer.Append(v.y);
			buffer.Append(v.width);
			buffer.Append(v.height);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Rect(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}