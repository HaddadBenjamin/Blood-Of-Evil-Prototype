#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public enum ExecResult
	{
		Success,
		NonLeafCommand,
		InvalidCommand
	}

	public abstract class CommandParser
	{
		public const string	CommandTextFieldName = "CmdInput";

		public static string[]	DefaultHelpCommands = { "help", "?" };

		public event Action	CallExec;

		public CommandNode	root { get; private set; }

		public string	precachedParentCommand;

		protected List<string>	historic;
		protected int			currentHistoric;
		protected string		backupCommand;
		protected string		lastCommand;

		protected string[]	matchingCommands;
		protected int		selectedMatchingCommand;

		public	CommandParser()
		{
			this.root = new CommandNode(null, "root", string.Empty);
			this.historic = new List<string>();
			this.currentHistoric = -1;
		}

		public void	SetRoot(CommandNode root)
		{
			this.root = root;
		}

		public void	AddRootCommand(CommandNode command)
		{
			this.root.AddChild(command);
		}

		public void	RemoveRootCommand(CommandNode command)
		{
			this.root.RemoveChild(command);
		}

		public abstract Rect	PostGUI(Rect r, ref string command);

		/// <summary></summary>
		/// <param name="input"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public ExecResult	Exec(string input, ref string result)
		{
			for (int j = 0; j < CommandParser.DefaultHelpCommands.Length; j++)
			{
				if (CommandParser.DefaultHelpCommands[j].Equals(input) == true)
				{
					Utility.sharedBuffer.Length = 0;

					Utility.sharedBuffer.AppendLine("Usage:");
					Utility.sharedBuffer.AppendLine("command.subCommand");
					Utility.sharedBuffer.AppendLine("command.subCommand arg1");
					Utility.sharedBuffer.AppendLine("command.subCommand arg1 \"foo bar\" 12 3.45");
					Utility.sharedBuffer.AppendLine();
					Utility.sharedBuffer.AppendLine("Commands available:");

					for (int k = 0; k < this.root.children.Count; k++)
						Utility.sharedBuffer.AppendLine(this.root.children[k].name);

					Utility.sharedBuffer.Length -= Environment.NewLine.Length;

					result = Utility.sharedBuffer.ToString();
					return ExecResult.Success;
				}
			}

			string[]	commands;
			string[]	arguments;
			this.ParseInput(input, out commands, out arguments);
			int			i;
			CommandNode	command = this.GetHighestValidCommand(commands, true, out i);

			this.historic.Add(input);
			this.currentHistoric = -1;

			if (commands.Length != i)
			{
				result = input + " is not recognized as a valid command. Type \"" + string.Join("\", \"", CommandParser.DefaultHelpCommands) + "\" to display usage.";
				return ExecResult.InvalidCommand;
			}

			if (command.IsLeaf == false)
			{
				result = input + " is not recognized as a valid command. Type \"" + string.Join("\", \"", CommandParser.DefaultHelpCommands) + "\" to display usage.";
				return ExecResult.NonLeafCommand;
			}

			try
			{
				result = command.GetSetInvoke(arguments);
			}
			catch (Exception ex)
			{
				result = ex.Message;
				return ExecResult.InvalidCommand;
			}

			return ExecResult.Success;
		}

		public void	HandleKeyboard(ref string command)
		{
			if (GUI.GetNameOfFocusedControl() != CommandParser.CommandTextFieldName)
				return;

			if (Event.current.type == EventType.KeyDown)
			{
				if (this.matchingCommands != null)
				{
					if (Event.current.keyCode == KeyCode.Escape)
					{
						this.matchingCommands = null;

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.Return ||
							 Event.current.keyCode == KeyCode.RightArrow ||
							 Event.current.keyCode == KeyCode.Tab)
					{
						if (this.selectedMatchingCommand != -1)
						{
							string[]	commands;
							string[]	arguments;
							this.ParseInput(command, out commands, out arguments);
							int		argumentsPosition = command.IndexOf(NGCLI.CommandsArgumentsSeparator);
							string	rawArguments = string.Empty;

							if (argumentsPosition != -1)
								rawArguments = command.Substring(argumentsPosition + 1);

							if (commands[commands.Length - 1] != this.matchingCommands[this.selectedMatchingCommand])
							{
								commands[commands.Length - 1] = this.matchingCommands[this.selectedMatchingCommand];

								if (rawArguments != string.Empty)
								{
									command = string.Join(NGCLI.CommandsSeparator.ToString(), commands) + NGCLI.CommandsArgumentsSeparator + rawArguments;
									this.SetCursor(command, command.Length - rawArguments.Length - 1);
								}
								else
								{
									command = string.Join(NGCLI.CommandsSeparator.ToString(), commands);
									this.SetCursor(command, command.Length);
								}
							}
						}
						else if (this.matchingCommands.Length == 1)
							this.SetCursor(this.matchingCommands[0], this.matchingCommands[0].Length);
						else
							this.SetCursor(command, command.Length);

						this.matchingCommands = null;

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.DownArrow)
					{
						if (this.selectedMatchingCommand >= 0)
							--this.selectedMatchingCommand;

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.UpArrow)
					{
						if (this.selectedMatchingCommand < this.matchingCommands.Length - 1)
							++this.selectedMatchingCommand;

						Event.current.Use();
					}
				}
				else
				{
					if (Event.current.keyCode == KeyCode.UpArrow)
					{
						if (this.currentHistoric == -1 && this.historic.Count > 0)
						{
							this.currentHistoric = this.historic.Count - 1;
							this.backupCommand = command;
							command = this.historic[this.currentHistoric];
							this.SetCursor(command, command.Length);
						}
						else if (this.currentHistoric > 0)
						{
							--this.currentHistoric;
							command = this.historic[this.currentHistoric];
							this.SetCursor(command, command.Length);
						}

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.DownArrow &&
								this.currentHistoric != -1)
					{
						if (this.currentHistoric + 1 == this.historic.Count)
						{
							this.currentHistoric = -1;
							command = this.backupCommand;
							this.SetCursor(command, command.Length);
						}
						else if (this.currentHistoric + 1 < this.historic.Count)
						{
							++this.currentHistoric;
							command = this.historic[this.currentHistoric];
							this.SetCursor(command, command.Length);
						}

						Event.current.Use();
					}
					else if (Event.current.keyCode == KeyCode.Return)
					{
						if (this.CallExec != null)
							this.CallExec();

						Event.current.Use();
					}
				}
			}

			if ((Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) &&
				(Event.current.keyCode == KeyCode.Tab || Event.current.character == '\t'))
			{
				if (Event.current.type == EventType.KeyDown)
				{
					string[]	commands;
					string[]	arguments;
					this.ParseInput(command, out commands, out arguments);
					string[]	matches = this.GetMatchingChildrenNames(commands);
					string		completion = string.Empty;

					// Complete last command.
					if (matches.Length == 1)
					{
						completion = matches[0];

						this.matchingCommands = null;
					}
					// Complete last command as much as possible.
					else if (matches.Length >= 2)
					{
						int	i;

						this.GetHighestValidCommand(commands, true, out i);

						if (i != commands.Length)
						{
							int	stringShared = commands[commands.Length - 1].Length;

							while (matches[0].Length > stringShared)
							{
								char	refChar = matches[0][stringShared];

								for (int j = 1; j < matches.Length; j++)
								{
									if (matches[j].Length <= stringShared ||
										matches[j][stringShared] != refChar)
									{
										goto doubleBreak;
									}
								}

								++stringShared;
							}

							doubleBreak:

							//if (string.Compare(commands[commands.Length - 1], 0, matches[0], 0, stringShared) == 0)
								completion = matches[0].Substring(0, stringShared);
						}

						this.matchingCommands = matches;
						this.selectedMatchingCommand = -1;

						Utility.sharedBuffer.Length = 0;

						// Concat commands till the last valid node.
						for (i = 0; i < commands.Length - 1; i++)
						{
							Utility.sharedBuffer.Append(commands[i]);
							Utility.sharedBuffer.Append(NGCLI.CommandsSeparator);
						}

						this.precachedParentCommand = Utility.sharedBuffer.ToString();
					}

					if (completion != string.Empty &&
						commands[commands.Length - 1] != completion)
					{
						int		argumentsPosition = command.IndexOf(NGCLI.CommandsArgumentsSeparator);
						string	rawArguments = string.Empty;

						if (argumentsPosition != -1)
							rawArguments = command.Substring(argumentsPosition + 1);

						commands[commands.Length - 1] = completion;

						if (rawArguments != string.Empty)
						{
							command = string.Join(NGCLI.CommandsSeparator.ToString(), commands) + NGCLI.CommandsArgumentsSeparator + rawArguments;
							this.SetCursor(command, command.Length - rawArguments.Length - 1);
						}
						else
						{
							command = string.Join(NGCLI.CommandsSeparator.ToString(), commands);
							this.SetCursor(command, command.Length);
						}
					}

					Event.current.Use();
				}
			}
		}

		public void	Clear()
		{
			this.precachedParentCommand = null;
			this.matchingCommands = null;
			this.currentHistoric = -1;
		}

		public void	UpdateMatchesAvailable(string command)
		{
			if (command == string.Empty)
			{
				this.matchingCommands = null;
				return;
			}

			if (this.lastCommand == command)
				return;

			this.lastCommand = command;

			string[]	commands;
			string[]	arguments;
			this.ParseInput(command, out commands, out arguments);
			string[]	matches = this.GetMatchingChildrenNames(commands);

			// Complet last command as much as possible.
			// Except when there is one argument.
			if (matches.Length >= 1 && arguments.Length == 0)
			{
				this.matchingCommands = matches;
				this.selectedMatchingCommand = -1;
			}
			else
				this.matchingCommands = null;

			Utility.sharedBuffer.Length = 0;

			// Concat commands till the last valid node.
			for (int i = 0; i < commands.Length - 1; i++)
			{
				Utility.sharedBuffer.Append(commands[i]);
				Utility.sharedBuffer.Append(NGCLI.CommandsSeparator);
			}

			this.precachedParentCommand = Utility.sharedBuffer.ToString();
		}

		public string[]		GetMatchingChildrenNames(string[] commands)
		{
			CommandNode[]	commandNodes = this.GetMatchingChildren(commands);
			List<string>	result = new List<string>();

			for (int i = 0; i < commandNodes.Length; i++)
				result.Add(commandNodes[i].name);

			return result.ToArray();
		}

		public CommandNode[]	GetMatchingChildren(string[] commands)
		{
			List<CommandNode>	result = new List<CommandNode>();
			int					highestNode = 0;
			CommandNode			highestCommand = this.GetHighestValidCommand(commands, false, out highestNode);

			// Whereas there is no highest input or it is empty.
			if (highestNode >= commands.Length)
			{
				for (int i = 0; i < highestCommand.children.Count; i++)
					result.Add(highestCommand.children[i]);
			}
			else if (highestCommand != null)
			{
				for (int i = 0; i < highestCommand.children.Count; i++)
				{
					if (highestCommand.children[i].name.StartsWith(commands[highestNode], StringComparison.InvariantCultureIgnoreCase) == true)
						result.Add(highestCommand.children[i]);
				}
			}

			return result.ToArray();
		}

		public void	SetCursor(string text, int position)
		{
			TextEditor	editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			editor.content = new GUIContent(text);
#else
			editor.text = text;
#endif
			editor.MoveTextEnd();
		}

		public void		ParseInput(string input, out string[] commands, out string[] arguments)
		{
			int	separator = input.IndexOf(NGCLI.CommandsArgumentsSeparator);

			if (separator == -1)
			{
				commands = this.ParseCommands(input);
				arguments = NGCLI.EmptyArray;
			}
			else
			{
				commands = this.ParseCommands(input.Substring(0, separator));
				arguments = this.ParseArguments(input.Substring(separator + 1));
			}
		}

		private string[]	ParseCommands(string input)
		{
			string[]	nodes = input.Split(NGCLI.CommandsSeparator);
			int			i = 0;

			// Get the highest valid node.
			for (; i < nodes.Length; i++)
			{
				if (nodes[i].IndexOfAny(NGCLI.ForbiddenChars) != -1)
					break;
			}

			string[]	validNodes = new string[i];

			for (int j = 0; j < validNodes.Length; j++)
				validNodes[j] = nodes[j];

			return validNodes;
		}

		private string[]	ParseArguments(string input)
		{
			List<string>	arguments = new List<string>();
			bool			quoted = false;

			Utility.sharedBuffer.Length = 0;

			for (int i = 0; i < input.Length; i++)
			{
				if (this.IsCharIn(input[i], NGCLI.ArgumentsSeparator) == true && Utility.sharedBuffer.Length == 0)
				{
					while (i < input.Length && this.IsCharIn(input[i], NGCLI.ArgumentsSeparator) == true)
						++i;

					if (i >= input.Length)
						break;
				}

				if (input[i] == '"')
				{
					if (Utility.sharedBuffer.Length == 0)
						quoted = true;
					else if (quoted == true &&
							 i + 1 < input.Length &&
							 this.IsCharIn(input[i + 1], NGCLI.ArgumentsSeparator) == true)
					{
						quoted = false;
						Utility.sharedBuffer.Append(input[i]);
						arguments.Add(Utility.sharedBuffer.ToString());
						Utility.sharedBuffer.Length = 0;
					}
					else
						quoted = false;
				}
				else if (quoted == false &&
						 this.IsCharIn(input[i], NGCLI.ArgumentsSeparator) == true)
				{
					arguments.Add(Utility.sharedBuffer.ToString());
					Utility.sharedBuffer.Length = 0;
				}
				else
					Utility.sharedBuffer.Append(input[i]);
			}

			if (Utility.sharedBuffer.Length > 0)
				arguments.Add(Utility.sharedBuffer.ToString());

			return arguments.ToArray();
		}

		protected CommandNode	GetHighestValidCommand(string[] inputs, bool matchExact, out int i)
		{
			CommandNode			highestCommand = this.root;
			List<CommandNode>	matching = new List<CommandNode>();
			int					max = (matchExact == true) ? inputs.Length : inputs.Length - 1;

			for (i = 0; i < max; i++)
			{
				matching.Clear();

				if (inputs[i] == string.Empty)
					return null;

				for (int j = 0; j < highestCommand.children.Count; j++)
				{
					if (i + 1 == inputs.Length)
					{
						if ((matchExact == true && highestCommand.children[j].name == inputs[i]) ||
							(matchExact == false && highestCommand.children[j].name.StartsWith(inputs[i]) == true))
						{
							matching.Add(highestCommand.children[j]);
						}
					}
					else if (highestCommand.children[j].name == inputs[i])
						matching.Add(highestCommand.children[j]);
				}

				if (matching.Count == 0)
					return null;
				if (matching.Count == 1 && matching[0].name == inputs[i])
					highestCommand = matching[0];
				else
					break;
			}

			return highestCommand;
		}

		private bool		HasEvenBackslashes(StringBuilder sb)
		{
			int	backslashes = 0;

			for (int i = sb.Length - 1; i > 0; --i)
			{
				if (sb[i] == '\\')
					++backslashes;
				else
					break;
			}

			return (backslashes & 1) == 0;
		}

		private bool	IsCharIn(char c, char[] list)
		{
			for (int i = 0; i < list.Length; i++)
			{
				if (c == list[i])
					return true;
			}

			return false;
		}
	}
}