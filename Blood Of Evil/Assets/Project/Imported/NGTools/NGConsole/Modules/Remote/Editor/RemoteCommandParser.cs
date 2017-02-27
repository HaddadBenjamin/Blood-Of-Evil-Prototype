using NGTools.NGGameConsole;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class RemoteCommandParser : CommandParser
	{
		private GUIStyle	highlightedMatchStyle;
		private string		highlightedColor = "#48FF00";

		public override Rect	PostGUI(Rect r, ref string command)
		{
			if (this.matchingCommands != null)
			{
				if (this.highlightedMatchStyle == null)
				{
					this.highlightedMatchStyle = new GUIStyle(GUI.skin.label);
					this.highlightedMatchStyle.richText = true;
				}

				string[]	commands;
				string[]	arguments;
				this.ParseInput(command, out commands, out arguments);

				r.height = 16F;
				r.x = GUI.skin.textField.CalcSize(new GUIContent(this.precachedParentCommand)).x;
				r.width = 200F;

				for (int i = 0; i < this.matchingCommands.Length; i++)
				{
					r.y -= r.height;

					if (Event.current.type == EventType.Repaint)
					{
						if (r.Contains(Event.current.mousePosition) == false)
							EditorGUI.DrawRect(r, Color.black);
						else
							EditorGUI.DrawRect(r, Color.white);
					}

					string	highlighted = ((i == this.selectedMatchingCommand) ? "<b>" : "") +
						"<color=" + this.highlightedColor + ">" +
						this.matchingCommands[i].Substring(0, commands[commands.Length - 1].Length) +
						"</color>" +
						this.matchingCommands[i].Substring(commands[commands.Length - 1].Length) +
						((i == this.selectedMatchingCommand) ? "</b>" : "");

					if (GUI.Button(r, highlighted, this.highlightedMatchStyle) == true)
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

				r.x = 0F;
			}

			return r;
		}
	}
}