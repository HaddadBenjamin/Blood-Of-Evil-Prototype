using NGTools;
using System;
using System.Collections.Generic;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class InputsGroup
	{
		public string				name;
		public List<InputCommand>	commands;

		public	InputsGroup(string name, InputCommand command)
		{
			this.name = name;
			this.commands = new List<InputCommand>();
			this.commands.Add(command);
		}

		public bool	ExistCommand(string commandName)
		{
			for (int i = 0; i < this.commands.Count; i++)
			{
				if (this.commands[i].name == commandName)
					return true;
			}

			return false;
		}

		public bool	ExistCommand(InputCommand command)
		{
			return this.ExistCommand(command.name);
		}

		public bool	Check(string commandName)
		{
			for (int i = 0; i < this.commands.Count; i++)
			{
				if (this.commands[i].name == commandName)
					return this.commands[i].Check();
			}

			InternalNGDebug.InternalLogWarning("ShortcutCommand \"" + commandName + "\" was not found.");
			return false;
		}
	}
}