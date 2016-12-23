using System.Collections;

namespace NGTools
{
	public class ListModifier : ICollectionModifier
	{
		public int	Size
		{
			get
			{
				return this.list.Count;
			}
		}

		public IList	list;

		public	ListModifier(IList list)
		{
			this.list = list;
		}

		public object	Get(int index)
		{
			return this.list[index];
		}

		public void		Set(int index, object value)
		{
			this.list[index] = value;
		}
	}
}