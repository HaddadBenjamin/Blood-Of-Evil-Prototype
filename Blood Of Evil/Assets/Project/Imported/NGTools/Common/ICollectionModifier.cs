namespace NGTools
{
	public interface ICollectionModifier
	{
		int	Size { get; }

		object	Get(int index);
		void	Set(int index, object value);
	}
}