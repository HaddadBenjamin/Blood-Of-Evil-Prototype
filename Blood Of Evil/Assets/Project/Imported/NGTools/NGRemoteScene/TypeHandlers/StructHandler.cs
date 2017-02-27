using System;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	[Priority(500)]
	internal sealed class StructHandler : TypeHandler
	{
		public override bool	CanHandle(Type type)
		{
			return type.IsStruct();
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			FieldInfo[]	fields = fieldType.GetFields();

			buffer.Append(fields.Length);

			for (int i = 0; i < fields.Length; i++)
			{
				buffer.AppendUnicodeString(fields[i].Name);

				TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(fields[i].FieldType);

				if (subHandler != null)
				{
					buffer.AppendUnicodeString(fields[i].FieldType.GetShortAssemblyType());

					subHandler.Serialize(buffer, fields[i].FieldType, fields[i].GetValue(instance));
				}
				// Append empty type.
				else
					buffer.Append(0);
			}
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			GenericClass	genericClass;

			int	fieldCount = buffer.ReadInt32();

			genericClass = new GenericClass(fieldCount);

			for (int i = 0; i < fieldCount; i++)
			{
				genericClass.names[i] = buffer.ReadUnicodeString();

				string	fieldAssemblyQualifiedName = buffer.ReadUnicodeString();

				if (string.IsNullOrEmpty(fieldAssemblyQualifiedName) == false)
				{
					genericClass.types[i] = Type.GetType(fieldAssemblyQualifiedName);

					if (genericClass.types[i] != null)
					{
						TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(genericClass.types[i]);

						if (subHandler != null)
							genericClass.values[i] = subHandler.Deserialize(buffer, genericClass.types[i]);
					}
				}
			}

			return genericClass;
		}
	}
}