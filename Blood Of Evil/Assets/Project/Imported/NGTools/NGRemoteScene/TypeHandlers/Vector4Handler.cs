using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(85)]
	internal sealed class Vector4Handler : TypeHandler
	{
		public	Vector4Handler() : base(typeof(Vector4))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector4);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Vector4	v = (Vector4)instance;

			buffer.Append(v.x);
			buffer.Append(v.y);
			buffer.Append(v.z);
			buffer.Append(v.w);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Vector4(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}