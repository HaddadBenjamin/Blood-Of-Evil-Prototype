using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	/// <summary>
	/// Gives the possibility to assign a class a description.
	/// </summary>
	public interface ICommandHelper
	{
		string	Helper { get; }
	}

	public class NGCLI : MonoBehaviour
	{
		[Serializable]
		public class AliasBehaviour
		{
			public string		alias;
			public Behaviour	behaviour;
		}

		public const string		ReceivedCommandColor = "green";
		public const string		PendingCommandColor = "yellow";
		public const string		ErrorCommandColor = "red";
		public const char		CommandsSeparator = '.';
		public static char[]	ArgumentsSeparator = new char[] { ' ', '	' };
		public const char		CommandsArgumentsSeparator = ' ';
		public static char[]	ForbiddenChars = new char[] { ' ' };
		public static string[]	EmptyArray = {};

		private static List<NGCLI>	instances = new List<NGCLI>();

		[Header("Keep NG CLI alive between scenes.")]
		public bool	dontDestroyOnLoad = true;

		[Header("[Required] Available commands to execute.")]
		public AliasBehaviour[]	rootCommands;

		[Header("[Optional] Link to a GameConsole to stick.")]
		public NGGameConsole	gameConsole;
		[Header("Height is used in both cases."),
		Header("[Optional] If no GameConsole, defines the position of the input on the screen.")]
		public Rect				inputArea;

		public Color	completionBgColor = Color.cyan;
		public Color	hoverCompletionBgColor = Color.grey;
		public Color	partialCompletionColor;
		public GUIStyle	highlightedMatchStyle;
		public GUIStyle	commandInputStyle;
		public float	execButtonWidth = 70F;
		public GUIStyle	execButtonStyle;

		internal LocalCommandParser	parser;

		private string	command = string.Empty;

		protected virtual void Reset()
		{
			GUICallback.Open(() =>
			{
				this.highlightedMatchStyle = new GUIStyle(GUI.skin.label);
				this.commandInputStyle = new GUIStyle(GUI.skin.textField);
				this.execButtonStyle = new GUIStyle(GUI.skin.button);
			});
		}

		protected virtual void	Awake()
		{
			if (this.parser != null)
				return;

			for (int i = 0; i < NGCLI.instances.Count; i++)
			{
				if (NGCLI.instances[i].GetType() == this.GetType())
				{
					UnityEngine.Object.Destroy(this.gameObject);
					return;
				}
			}

			this.parser = new LocalCommandParser(this);
			this.parser.CallExec += this.Exec;

			if (this.rootCommands != null && this.rootCommands.Length > 0)
			{
				for (int i = 0; i < this.rootCommands.Length; i++)
				{
					if (string.IsNullOrEmpty(this.rootCommands[i].alias) == true)
					{
						InternalNGDebug.Log(Errors.CLI_RootCommandEmptyAlias, "Root command #" + i + " as an empty alias.");
						continue;
					}
					if (this.rootCommands[i].behaviour == null)
					{
						InternalNGDebug.Log(Errors.CLI_RootCommandNullBehaviour, "Root command \"" + this.rootCommands[i].alias + "\" has no behaviour.");
						continue;
					}

					ICommandHelper	helper = this.rootCommands[i].behaviour as ICommandHelper;
					string			description = string.Empty;

					if (helper != null)
						description = helper.Helper;

					this.parser.AddRootCommand(new BehaviourCommand(this.rootCommands[i].alias,
																	description,
																	this.rootCommands[i].behaviour));
				}
			}
			else
				InternalNGDebug.Log(Errors.CLI_EmptyRootCommand, "There is not root command in your CLI.");

			if (this.dontDestroyOnLoad == true)
			{
				NGCLI.instances.Add(this);
				UnityEngine.Object.DontDestroyOnLoad(this.transform.root.gameObject);
			}
		}

		protected virtual void	OnEnable()
		{
			if (this.gameConsole != null)
			{
				this.gameConsole.ReserveFootSpace(this.inputArea.height);
				//this.gameConsole.AddSetting("NG CLI", this.GUISettings);
			}
		}

		protected virtual void	OnDisable()
		{
			if (this.gameConsole != null)
			{
				this.gameConsole.ReserveFootSpace(-this.inputArea.height);
				//this.gameConsole.RemoveSetting("NG CLI", this.GUISettings);
			}
		}

		protected virtual void	OnGUI()
		{
			if (this.gameConsole != null &&
				((this.gameConsole.visible == false || this.gameConsole.enabled == false) ||
				 this.gameConsole.renderingMode != NGGameConsole.RenderingMode.Logs))
			{
				return;
			}

			GUI.depth = -1;

			Rect	r = this.inputArea;
			if (this.gameConsole != null)
			{
				r = new Rect(this.gameConsole.windowSize.x,
							 this.gameConsole.windowSize.y + this.gameConsole.windowSize.height - this.inputArea.height,
							 this.gameConsole.windowSize.width,
							 r.height);

				if (this.gameConsole.resizable == true)
					r.width -= 25F;
			}

			float	width = r.width;

			this.parser.HandleKeyboard(ref this.command);

			r.width -= this.execButtonWidth;
			GUI.SetNextControlName(CommandParser.CommandTextFieldName);
			this.command = GUI.TextField(r, this.command, this.commandInputStyle);
			if (GUI.changed == true)
				this.parser.UpdateMatchesAvailable(this.command);

			r.x += r.width;
			r.width = this.execButtonWidth;
			if (GUI.Button(r, "Exec", this.execButtonStyle) == true)
				this.Exec();

			r.x -= width - this.execButtonWidth;
			r.width = width;
			this.parser.PostGUI(r, ref this.command);
		}

		private void	OnValidate()
		{
			if (this.execButtonWidth < 10F)
				this.execButtonWidth = 10F;
		}

		private void	Exec()
		{
			GameLog		log;
			string		result = string.Empty;
			ExecResult	returnValue = this.parser.Exec(this.command, ref result);

			if (returnValue == ExecResult.Success)
			{
				log = new GameLog(this.command,
								  "<color=" + NGCLI.ReceivedCommandColor + ">></color> " + result,
								  LogType.Log,
								  this.gameConsole != null ? this.gameConsole.timeFormat : string.Empty);
			}
			else
			{
				log = new GameLog(this.command,
								  "<color=" + NGCLI.ErrorCommandColor + ">></color> " + result,
								  LogType.Error,
								  this.gameConsole != null ? this.gameConsole.timeFormat : string.Empty);
			}

			log.opened = true;

			if (this.gameConsole != null)
				this.gameConsole.AddGameLog(log);

			this.command = string.Empty;
			this.parser.Clear();
		}

		private void	GUISettings()
		{
			GUILayout.Button("Test");
		}
	}
}