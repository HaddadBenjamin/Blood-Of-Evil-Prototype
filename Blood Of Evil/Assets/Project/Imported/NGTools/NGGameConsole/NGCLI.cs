using System;
using UnityEngine;

namespace NGTools
{
	public partial class PacketId
	{
		public const int	Logger_ServerSendLog = 1000;
		public const int	CLI_ClientRequestCommandNodes = 2001;
		public const int	CLI_ServerSendCommandNodes = 2002;
		public const int	CLI_ClientSendCommand = 2003;
		public const int	CLI_ServerSendCommandResponse = 2004;
	}

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

		protected virtual void	Awake()
		{
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
		}

		protected virtual void	OnEnable()
		{
			if (this.gameConsole != null)
				this.gameConsole.ReserveFootSpace(this.inputArea.height);
		}

		protected virtual void	OnDisable()
		{
			if (this.gameConsole != null)
				this.gameConsole.ReserveFootSpace(-this.inputArea.height);
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
			{
				this.Exec();
			}

			r.x -= width - this.execButtonWidth;
			r.width = width;
			this.parser.PostGUI(r, ref this.command);
		}

#if UNITY_EDITOR
		private void	OnValidate()
		{
			if (this.execButtonWidth < 10F)
				this.execButtonWidth = 10F;
		}
#endif


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
	}
}