namespace NGTools.NGRemoteScene
{
	public abstract class CameraDataModule
	{
		public readonly byte	moduleID;
		public readonly int		priority;
		public readonly string	name;

		public bool	active = true;

		protected	CameraDataModule(byte moduleID, int priority, string name)
		{
			this.moduleID = moduleID;
			this.priority = priority;
			this.name = name;
		}
	}
}