using System;
using System.Reflection;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		public const int	Version = 1;

		public static event Action<NGSettings>	Initialize;

		[HideInInspector]
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

		public void	Init()
		{
			FieldInfo[]	fields = typeof(NGSettings).GetFields(BindingFlags.Public | BindingFlags.Instance);

			for (int i = 0; i < fields.Length; i++)
			{
				NGSettings.Settings	settings = fields[i].GetValue(this) as NGSettings.Settings;

				if (settings != null)
					settings.InternalInitGUI();
			}

			if (NGSettings.Initialize != null)
				NGSettings.Initialize(this);
		}
	}
}