using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(35)]
	internal sealed class Vector3Handler : TypeHandler
	{
		public	Vector3Handler() : base(typeof(Vector3))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Vector3);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Vector3	v = (Vector3)instance;

			buffer.Append(v.x);
			buffer.Append(v.y);
			buffer.Append(v.z);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Vector3(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}