using UnityEngine;

namespace NGToolsEditor
{
	public abstract class AbstractModuleSettings : ScriptableObject, IGUIInitializer
	{
		private bool	guiInit = false;

		protected virtual void	Reset()
		{
			this.guiInit = false;
		}

		public void	InitGUI()
		{
			if (this.guiInit == true)
				return;

			this.guiInit = true;

			this.InitializeGUI();
		}

		protected abstract void	InitializeGUI();
	}
}