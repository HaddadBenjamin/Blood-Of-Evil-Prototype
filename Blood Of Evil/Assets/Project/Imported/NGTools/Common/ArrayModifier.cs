using System;

namespace NGTools
{
	public class ArrayModifier : ICollectionModifier
	{
		public int	Size
		{
			get
			{
				return this.array.Length;
			}
		}

		public Type	Type
		{
			get
			{
				return Utility.GetArraySubType(this.array.GetType());
			}
		}

		public Array	array;

		public	ArrayModifier(Array array)
		{
			this.array = array;
		}

		public object	Get(int index)
		{
			return this.array.GetValue(index);
		}

		public void		Set(int index, object value)
		{
			this.array.SetValue(value, index);
		}
	}
}