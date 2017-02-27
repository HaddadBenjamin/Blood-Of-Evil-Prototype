using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Priority(70)]
	internal sealed class QuaternionHandler : TypeHandler
	{
		public	QuaternionHandler() : base(typeof(Quaternion))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(Quaternion);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			Quaternion	v = (Quaternion)instance;

			buffer.Append(v.x);
			buffer.Append(v.y);
			buffer.Append(v.z);
			buffer.Append(v.w);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new Quaternion(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
		}
	}
}