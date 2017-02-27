using UnityEngine;

namespace NGTools.NGGameConsole
{
	internal class LocalCommandParser : CommandParser
	{
		private NGCLI	cli;
		private Texture2D	hover;
		private Texture2D	idle;

		private string		cachedPartialColorMarkup;
		private GUIContent	content = new GUIContent();

		public LocalCommandParser(NGCLI cli)
		{
			this.cli = cli;

			this.hover = new Texture2D(1, 1);
			this.hover.SetPixel(0, 0, cli.hoverCompletionBgColor);
			this.hover.Apply();

			this.idle = new Texture2D(1, 1);
			this.idle.SetPixel(0, 0, cli.completionBgColor);
			this.idle.Apply();

			this.cachedPartialColorMarkup = "<color=#" + ((int)(this.cli.partialCompletionColor.r * 255)).ToString("X2") + ((int)(this.cli.partialCompletionColor.g * 255)).ToString("X2") + ((int)(this.cli.partialCompletionColor.b * 255)).ToString("X2") + ">";
		}

		public override Rect	PostGUI(Rect r, ref string command)
		{
			if (this.matchingCommands != null)
			{
				string[]	commands;
				string[]	arguments;
				Vector2		size;

				this.ParseInput(command, out commands, out arguments);

				int	commandLength = commands[commands.Length - 1].Length;

				this.content.text = precachedParentCommand;
				size = this.cli.commandInputStyle.CalcSize(this.content);
				r.x += size.x;
				r.width = 0F;
				r.height = size.y;

				arguments = new string[this.matchingCommands.Length];
				for (int i = 0; i < this.matchingCommands.Length; i++)
				{
					if (commandLength > 0)
						arguments[i] = this.cachedPartialColorMarkup + this.matchingCommands[i].Substring(0, commandLength) + "</color>" + this.matchingCommands[i].Substring(commandLength);
					else
						arguments[i] = this.matchingCommands[i];

					if (i == this.selectedMatchingCommand)
						arguments[i] = "<b>" + arguments[i] + "</b>";

					this.content.text = arguments[i];
					size = this.cli.highlightedMatchStyle.CalcSize(this.content);

					if (r.width < size.x)
						r.width = size.x;
				}

				r.height = size.y;

				for (int i = 0; i < this.matchingCommands.Length; i++)
				{
					r.y -= r.height;

					if (Event.current.type == EventType.Repaint)
					{
						if (r.Contains(Event.current.mousePosition) == false)
							GUI.DrawTexture(r, this.idle);
						else
							GUI.DrawTexture(r, this.hover);
					}

					if (GUI.Button(r, arguments[i], this.cli.highlightedMatchStyle) == true)
					{
						int		argumentsPosition = command.IndexOf(NGCLI.CommandsArgumentsSeparator);
						string	rawArguments = string.Empty;

						if (argumentsPosition != -1)
							rawArguments = command.Substring(argumentsPosition + 1);

						commands[commands.Length - 1] = this.matchingCommands[i];

						if (rawArguments != string.Empty)
						{
							command = string.Join(NGCLI.CommandsSeparator.ToString(), commands) +
											NGCLI.CommandsArgumentsSeparator +
											rawArguments;
							this.SetCursor(command, command.Length - rawArguments.Length - 1);
						}
						else
						{
							command = string.Join(NGCLI.CommandsSeparator.ToString(), commands);
							this.SetCursor(command, command.Length);
						}

						this.matchingCommands = null;
						break;
					}
				}
			}

			return r;
		}
	}
}