using System;
using System.Reflection;
using System.Text;

namespace NGTools
{
	public sealed class NetMethod
	{
		public readonly string		name;
		public readonly Type[]		argumentTypes;
		public readonly string[]	argumentNames;

		public static void	Serialize(ServerMethodInfo method, ByteBuffer buffer)
		{
			buffer.AppendUnicodeString(method.methodInfo.Name);
			buffer.Append(method.argumentTypes.Length);

			for (int i = 0; i < method.argumentTypes.Length; i++)
			{
				buffer.AppendUnicodeString(method.argumentTypes[i].GetShortAssemblyType());
				buffer.AppendUnicodeString(method.argumentNames[i]);
			}
		}

		public static NetMethod	Deserialize(ByteBuffer buffer)
		{
			return new NetMethod(buffer);
		}

		private	NetMethod(ByteBuffer buffer)
		{
			this.name = buffer.ReadUnicodeString();
			this.argumentNames = new string[buffer.ReadInt32()];
			this.argumentTypes = new Type[this.argumentNames.Length];

			for (int i = 0; i < this.argumentNames.Length; i++)
			{
				string	aqn = buffer.ReadUnicodeString();
				this.argumentTypes[i] = Type.GetType(aqn);

				// Happen with generic methods.
				if (this.argumentTypes[i] == null)
					this.argumentTypes[i] = typeof(Missing);

				this.argumentNames[i] = buffer.ReadUnicodeString();
			}
		}
	}
}