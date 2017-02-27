namespace NGTools.NGRemoteScene
{
	public class EnumData
	{
		public readonly bool		hasFlagAttribute;
		public readonly string[]	names;
		public readonly int[]		values;

		public	EnumData(bool flag, string[] names, int[] values)
		{
			this.hasFlagAttribute = flag;
			this.names = names;
			this.values = values;
		}
	}
}