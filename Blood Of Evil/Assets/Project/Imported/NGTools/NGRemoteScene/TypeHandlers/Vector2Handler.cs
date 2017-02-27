using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(35)]
	internal sealed class Vector2Handler : TypeHandler
	{
		public	Vector2Handler() : base(typeof(Vector2))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector2);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Vector2	v = (Vector2)instance;

			buffer.Append(v.x);
			buffer.Append(v.y);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Vector2(buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}