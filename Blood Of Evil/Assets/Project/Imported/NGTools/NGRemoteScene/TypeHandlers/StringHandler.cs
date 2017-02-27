using System;

namespace NGTools.NGRemoteScene
{
	[Priority(10)]
	internal sealed class StringHandler : TypeHandler
	{
		public	StringHandler() : base(typeof(String))
		{
		}

		public override bool	CanHandle(Type type)
		{
			return type == typeof(String);
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			if (instance == null)
				buffer.Append(0);
			else
			{
				string	str = (string)instance;

				buffer.AppendUnicodeString(str);
			}
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return buffer.ReadUnicodeString();
		}
	}
}