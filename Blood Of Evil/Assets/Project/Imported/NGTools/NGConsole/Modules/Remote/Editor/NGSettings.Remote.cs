using System;

namespace NGToolsEditor
{
	using UnityEngine;

	public partial class NGSettings : ScriptableObject
	{
		[Serializable]
		public class RemoteModuleSettings : Settings
		{
			public float	execButtonWidth = 70F;
			public GUIStyle	commandInputStyle;
			public GUIStyle	execButtonStyle;

			protected override void	InitGUI()
			{
				this.commandInputStyle = new GUIStyle(GUI.skin.textField);
				this.execButtonStyle = new GUIStyle("ToolbarButton");
				this.execButtonStyle.fontStyle = FontStyle.Bold;
			}
		}
		public RemoteModuleSettings	remoteModule = new RemoteModuleSettings();
	}
}