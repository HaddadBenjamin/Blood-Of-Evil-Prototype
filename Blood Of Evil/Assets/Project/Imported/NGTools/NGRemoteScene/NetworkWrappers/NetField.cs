using System;

namespace NGTools.NGRemoteScene
{
	using UnityEngine;

	public sealed class NetField
	{
		private static ByteBuffer	fieldBuffer = new ByteBuffer(256);

		public readonly Type				fieldType;
		public readonly string				name;
		public readonly bool				isPublic;
		public readonly	FieldInnerValueType	innerValueType;

		public object	value;

		public readonly TypeHandler	handler;

		public static void	Serialize(object instance, IFieldModifier field, ByteBuffer buffer)
		{
			fieldBuffer.Clear();

			fieldBuffer.AppendUnicodeString(field.Type.GetShortAssemblyType());
			fieldBuffer.AppendUnicodeString(field.Name);
			fieldBuffer.Append(field.IsPublic);
			// We need to send FieldInnerValueType, in the case of RefType, FieldType might be null.
			fieldBuffer.Append((int)TypeHandlersManager.GetFieldInnerValueType(field.Type));

			TypeHandler	handler = TypeHandlersManager.GetTypeHandler(field.Type);

			//InternalNGDebug.Log("NetF.Ser" + field.Type + " " + field.Name + "=" + handler);
			if (handler != null)
				handler.Serialize(fieldBuffer, field.Type, field.GetValue(instance));

			buffer.Append(fieldBuffer.Length);
			buffer.Append(fieldBuffer);
		}

		public static NetField	Deserialize(ByteBuffer buffer)
		{
			return new NetField(buffer);
		}

		private	NetField(ByteBuffer buffer)
		{
			int	chunkFieldLength = buffer.ReadInt32();
			int	fallbackEndPosition = buffer.Position + chunkFieldLength;

			this.fieldType = Type.GetType(buffer.ReadUnicodeString());
			this.name = buffer.ReadUnicodeString();
			this.isPublic = buffer.ReadBoolean();
			this.innerValueType = (FieldInnerValueType)buffer.ReadInt32();

			if (this.innerValueType != FieldInnerValueType.Null)
			{
				Type	fieldTypeSubstitute = this.fieldType;

				this.handler = TypeHandlersManager.GetTypeHandler(fieldTypeSubstitute);

				if (this.handler == null)
				{
					// Especially for RefType and ArrayRefType, we need to call them manually.
					// Because we do not care about reference types, we directly work with GenericClass on client-side.
					if (this.innerValueType == FieldInnerValueType.RefType ||
						this.innerValueType == FieldInnerValueType.StructType)
					{
						this.handler = TypeHandlersManager.GetRefTypeHandler();
					}
					else if (this.innerValueType == FieldInnerValueType.ArrayRefType)
						this.handler = TypeHandlersManager.GetArrayRefTypeHandler();

					// BUT, ref-types like string must be handled, thus fieldTypeSubstitute instead of fieldType.
					if (this.innerValueType == FieldInnerValueType.RefType)
						fieldTypeSubstitute = typeof(GenericClass);
					else if (this.innerValueType == FieldInnerValueType.ArrayRefType)
						fieldTypeSubstitute = typeof(GenericClass[]);
				}
				else if (this.innerValueType == FieldInnerValueType.ArrayRefType ||
						 this.innerValueType == FieldInnerValueType.ArrayValueType ||
						 this.innerValueType == FieldInnerValueType.ArrayUnityObject)
				{
					Type		subType = Utility.GetArraySubType(fieldTypeSubstitute);
					TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(subType);

					if (typeof(Object).IsAssignableFrom(subType) == true)
					{
						typeHandler = TypeHandlersManager.GetTypeHandler(typeof(Object));
						fieldTypeSubstitute = typeof(UnityObject[]);
					}

					if (typeHandler == TypeHandlersManager.GetRefTypeHandler() ||
						typeHandler == TypeHandlersManager.GetStructTypeHandler())
					{
						fieldTypeSubstitute = typeof(GenericClass[]);
					}
				}

				try
				{
					//NGDebug.MTLog(this.name + " " + this.handler + " || " + fieldTypeSubstitute + "## " + this.innerValueType);
					if (this.handler != null)
						this.value = this.handler.Deserialize(buffer, fieldTypeSubstitute);
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException("Field " + this.name + " of type " + this.fieldType, ex);
				}
			}

			buffer.Position = fallbackEndPosition;
		}
	}
}