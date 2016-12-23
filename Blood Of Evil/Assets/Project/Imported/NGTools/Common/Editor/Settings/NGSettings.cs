using System;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		public const int	Version = 1;

#if !NGT_DEBUG
		[HideInInspector]
#endif
		public int	version =  NGSettings.Version;

		[Serializable]
		public abstract class Settings
		{
			[SerializeField, HideInInspector]
			private bool	init;

			public void	InternalInitGUI()
			{
				if (this.init == true)
					return;

				this.init = true;
				this.InitGUI();
			}

			protected abstract void	InitGUI();
		}
	}
}