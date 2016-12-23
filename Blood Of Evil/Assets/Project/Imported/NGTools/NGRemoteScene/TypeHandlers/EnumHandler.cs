using System;

namespace NGTools
{
	public class EnumInstance
	{
		public enum IsFlag
		{
			Unset,
			Value,
			Flag
		}

		public string	type;
		public int		value;

		private IsFlag	flag;

		public	EnumInstance(string type, int value)
		{
			this.flag = IsFlag.Unset;
			this.type = type;
			this.value = value;
		}

		public IsFlag	GetFlag()
		{
			return this.flag;
		}

		public void	SetFlag(IsFlag flag)
		{
			this.flag = flag;
		}
	}

	[Priority(0)]
	public class EnumHandler : TypeHandler
	{
		public override bool	CanHandle(Type type)
		{
			return type.IsEnum == true;
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.AppendUnicodeString(fieldType.GetShortAssemblyType());
			buffer.Append((Int32)instance);
		}

		public override object	Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new EnumInstance(buffer.ReadUnicodeString(), buffer.ReadInt32());
		}

		public override object	DeserializeRealValue(NGServerScene manager, ByteBuffer buffer, Type fieldType)
		{
			buffer.position += buffer.ReadInt32();
			return buffer.ReadInt32();
		}
	}
}