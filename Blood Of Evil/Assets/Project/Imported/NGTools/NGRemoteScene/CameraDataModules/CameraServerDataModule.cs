namespace NGTools.NGRemoteScene
{
	public abstract class CameraServerDataModule : CameraDataModule
	{
		protected	CameraServerDataModule(byte moduleID, int priority, string name) : base(moduleID, priority, name)
		{
		}

		/// <summary>
		/// Called in Awake on the server side.
		/// </summary>
		/// <param name="scene"></param>
		public virtual void	Awake(NGServerScene scene)
		{
		}

		/// <summary>
		/// Called in OnDestroy on the server side.
		/// </summary>
		/// <param name="scene"></param>
		public virtual void	OnDestroy(NGServerScene scene)
		{
		}

		/// <summary>
		/// Called in OnGUI on the server side.
		/// </summary>
		/// <param name="data"></param>
		public virtual void	OnGUI(ICameraData data)
		{
		}

		/// <summary>
		/// Called in Update on the server side.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="listener"></paramx
		public virtual void	Update(ICameraData data)
		{
		}
	}
}