using System;
using System.Collections;

namespace NGTools.NGRemoteScene
{
	internal sealed class ArrayData
	{
		public readonly	Type	type;
		public readonly	bool	isBigArray;
		public int				length;
		public Array			array;

		public	ArrayData(Type type, bool isBigArray, int length)
		{
			this.type = type;
			this.isBigArray = isBigArray;
			this.length = length;
		}
	}

	[Priority(100)]
	internal sealed class ArrayHandler : TypeHandler
	{
		public const int	BigArrayThreshold = 256;

		/// <summary>
		/// Forces the handler to serialize big array. It is a use once variable.
		/// </summary>
		public bool	forceBigArray = false;

		public override bool	CanHandle(Type type)
		{
			return type.IsUnityArray();
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			IEnumerable	array = instance as IEnumerable;

			if (array == null)
			{
				buffer.Append(0);
				buffer.Append(false);
				return;
			}

			bool	isBigArray = false;
			int		count = 0;

			if (fieldType.IsArray == true)
			{
				Array	a = array as Array;

				count = a.Length;
				isBigArray = a.Length > ArrayHandler.BigArrayThreshold;
			}
			else if (typeof(IList).IsAssignableFrom(fieldType) == true)
			{
				IList	a = array as IList;

				count = a.Count;
				isBigArray = a.Count > ArrayHandler.BigArrayThreshold;
			}
			else
				throw new InvalidCastException("Array of type \"" + fieldType + "\" is not supported.");

			buffer.Append(count);

			if (this.forceBigArray == true)
				buffer.Append(false);
			else
				buffer.Append(isBigArray);

			if (isBigArray == false || this.forceBigArray == true)
			{
				Type		subType = Utility.GetArraySubType(fieldType);
				TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(subType);
				InternalNGDebug.Assert(subHandler != null, "TypeHandler for " + subType + " is not supported.");

				ByteBuffer	arrayBuffer = new ByteBuffer(256);

				foreach (var item in array)
					subHandler.Serialize(arrayBuffer, subType, item);

				buffer.Append(arrayBuffer);
			}

			// Reset value.
			this.forceBigArray = false;
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			int			count = buffer.ReadInt32();
			ArrayData	arrayData = new ArrayData(fieldType, buffer.ReadBoolean(), count);

			if (arrayData.isBigArray == false)
			{
				Type		subType = Utility.GetArraySubType(fieldType);
				TypeHandler	subHandler = TypeHandlersManager.GetTypeHandler(subType);

				InternalNGDebug.Assert(subHandler != null, "TypeHandler for " + subType + " is not supported." + fieldType);

				arrayData.array = Array.CreateInstance(subType, count);

				for (int i = 0; i < count; i++)
				{
					try
					{
						arrayData.array.SetValue(subHandler.Deserialize(buffer, subType), i);
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException("Array of type " + fieldType.Name + " at " + i + " failed.", ex);
						throw;
					}
				}
			}

			return arrayData;
		}
	}
}