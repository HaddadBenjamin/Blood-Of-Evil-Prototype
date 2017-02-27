using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	[Serializable]
	internal sealed class MaskTypeFilter : ILogFilter
	{
		[Exportable]
		private bool	enabled;
		public bool		Enabled { get { return this.enabled; } set { if (this.enabled != value) { this.enabled = value; if (this.ToggleEnable != null) this.ToggleEnable(); } } }

		public event Action	ToggleEnable;

		[Exportable]
		private int		maskType;

		[NonSerialized]
		private GUIContent	logContent;
		[NonSerialized]
		private GUIContent	warningContent;
		[NonSerialized]
		private GUIContent	errorContent;
		[NonSerialized]
		private GUIContent	exceptionContent;

		public void	Init()
		{
			this.logContent = new GUIContent();
			this.logContent.image = Utility.GetConsoleIcon("console.infoicon.sml");

			this.warningContent = new GUIContent();
			this.warningContent.image = Utility.GetConsoleIcon("console.warnicon.sml");

			this.errorContent = new GUIContent();
			this.errorContent.image = Utility.GetConsoleIcon("console.erroricon.sml");

			this.exceptionContent = new GUIContent();
			this.exceptionContent.image = Utility.GetConsoleIcon("console.erroricon.sml");
		}

		public FilterResult	CanDisplay(Row row)
		{
			if ((row.log.mode & Mode.ScriptingException) != 0)
			{
				if ((this.maskType & (1 << (int)LogType.Exception)) != 0)
					return FilterResult.Accepted;
				return FilterResult.None;
			}
			else if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptingError | Mode.Fatal | Mode.Error | Mode.Assert | Mode.AssetImportError | Mode.ScriptingAssertion)) != 0)
			{
				if ((this.maskType & (1 << (int)LogType.Error)) != 0)
					return FilterResult.Accepted;
				return FilterResult.None;
			}
			else if ((row.log.mode & (Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning)) != 0)
			{
				if ((this.maskType & (1 << (int)LogType.Warning)) != 0)
					return FilterResult.Accepted;
				return FilterResult.None;
			}

			if ((this.maskType & (1 << (int)LogType.Log)) != 0)
				return FilterResult.Accepted;
			return FilterResult.None;
		}

		public void	OnGUI()
		{
			if (this.logContent == null)
				this.Init();

			GUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				EditorGUILayout.PrefixLabel(LC.G("AcceptedTypes"));

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle((this.maskType & (1 << (int)LogType.Log)) != 0, this.logContent, Preferences.Settings.general.menuButtonStyle);
				if (EditorGUI.EndChangeCheck())
					this.maskType = ((int)this.maskType ^ (1 << (int)LogType.Log));

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle((this.maskType & (1 << (int)LogType.Warning)) != 0, this.warningContent, Preferences.Settings.general.menuButtonStyle);
				if (EditorGUI.EndChangeCheck())
					this.maskType = ((int)this.maskType ^ (1 << (int)LogType.Warning));

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle((this.maskType & (1 << (int)LogType.Error)) != 0, this.errorContent, Preferences.Settings.general.menuButtonStyle);
				if (EditorGUI.EndChangeCheck())
					this.maskType = ((int)this.maskType ^ (1 << (int)LogType.Error));

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle((this.maskType & (1 << (int)LogType.Exception)) != 0, this.exceptionContent, Preferences.Settings.general.menuButtonStyle);
				if (EditorGUI.EndChangeCheck())
					this.maskType = ((int)this.maskType ^ (1 << (int)LogType.Exception));
			}
			GUILayout.EndHorizontal();
		}

		public void	ContextMenu(GenericMenu menu, Row row, int i)
		{
		}
	}
}