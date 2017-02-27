namespace NGToolsEditor.NGConsole
{
	public abstract class Preset
	{
		/// <summary>
		/// Overwrites settings in the given <paramref name="instance"/>.
		/// </summary>
		/// <param name="instance"></param>
		public abstract void	SetSettings(NGSettings instance);
	}
}