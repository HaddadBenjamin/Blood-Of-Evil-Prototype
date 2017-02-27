using System;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	public sealed class NetMethod
	{
		public readonly string		name;
		public readonly Type		returnType;
		public readonly Type[]		argumentTypes;
		public readonly string[]	argumentNames;

		public static void	Serialize(ServerMethodInfo method, ByteBuffer buffer)
		{
			buffer.AppendUnicodeString(method.methodInfo.Name);
			buffer.AppendUnicodeString(method.methodInfo.ReturnType.GetShortAssemblyType());
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
			this.returnType = Type.GetType(buffer.ReadUnicodeString());
			this.argumentNames = new string[buffer.ReadInt32()];
			this.argumentTypes = new Type[this.argumentNames.Length];

			for (int i = 0; i < this.argumentNames.Length; i++)
			{
				this.argumentTypes[i] = Type.GetType(buffer.ReadUnicodeString());

				// Happen with generic methods.
				if (this.argumentTypes[i] == null)
					this.argumentTypes[i] = typeof(Missing);

				this.argumentNames[i] = buffer.ReadUnicodeString();
			}
		}
	}
}