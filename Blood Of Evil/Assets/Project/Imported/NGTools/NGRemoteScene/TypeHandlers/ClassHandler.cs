using System;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	public sealed class GenericClass
	{
		public bool	isNull { get { return this.names.Length == 0; } }

		public string[] names { get; private set; }
		public Type[]	types { get; private set; }
		public object[]	values { get; private set; }

		public	GenericClass() : this(0)
		{
		}

		public	GenericClass(int fieldCount)
		{
			this.names = new string[fieldCount];
			this.types = new Type[fieldCount];
			this.values = new object[fieldCount];
		}

		public void	SetAll(string[] names, Type[] types, object[] values)
		{
			this.names = names;
			this.types = types;
			this.values = values;
		}

		public void	SetValue(string field, object value)
		{
			for (int i = 0; i < this.names.Length; i++)
			{
				if (this.names[i] == field)
				{
					this.values[i] = value;
					return;
				}
			}

			throw new ArgumentException("Field \"" + field + "\" was not found.");
		}

		public object	GetValue(string field)
		{
			for (int i = 0; i < this.names.Length; i++)
			{
				if (this.names[i] == field)
					return this.values[i];
			}

			throw new ArgumentException("Field \"" + field + "\" was not found.");
		}

		public Type		GetType(string field)
		{
			for (int i = 0; i < this.names.Length; i++)
			{
				if (this.names[i] == field)
					return this.types[i];
			}

			throw new ArgumentException("Field \"" + field + "\" was not found.");
		}

		public override string	ToString()
		{
			return "GenericClass(" + string.Join(", ", this.names) + ")";
		}
	}

	[Priority(1000)]
	internal sealed class ClassHandler : TypeHandler
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(GenericClass) || type.IsClass == true && type.IsUnityArray() == false;
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			FieldInfo[]	fields = fieldType.GetFields();

			if (instance == null)
				buffer.Append(true);
			else
			{
				buffer.Append(false);
				buffer.Append(fields.Length);

				for (int i = 0; i < fields.Length; i++)
				{
					buffer.AppendUnicodeString(fields[i].Name);

					TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(fields[i].FieldType);

					if (subHandler != null)
					{
						buffer.AppendUnicodeString(TypeHandlersManager.GetNetworkType(fields[i].FieldType).GetShortAssemblyType());

						subHandler.Serialize(buffer, fields[i].FieldType, fields[i].GetValue(instance));
					}
					// Append empty type.
					else
						buffer.Append(0);
				}
			}
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			bool	isNull = buffer.ReadBoolean();

			GenericClass	genericClass;

			if (isNull == false)
			{
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
			}
			else
				genericClass = new GenericClass();

			return genericClass;
		}
	}
}