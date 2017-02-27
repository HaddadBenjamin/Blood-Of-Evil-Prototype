using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class InputsManager
	{
		public List<InputsGroup>	groups = new List<InputsGroup>();

		/// <summary>
		/// Adds <paramref name="command"/> in the given <paramref name="group"/> if not in already.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="command"></param>
		public void	AddCommand(string group, InputCommand command)
		{
			for (int i = 0; i < this.groups.Count; i++)
			{
				if (this.groups[i].name == group)
				{
					if (this.groups[i].ExistCommand(command) == false)
						this.groups[i].commands.Add(command);
					return;
				}
			}

			this.groups.Add(new InputsGroup(group, command));
		}

		/// <summary>
		/// Adds a new shortcut command in the given <paramref name="group"/> if the name is not in it already.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="name"></param>
		/// <param name="keyCode"></param>
		/// <param name="control"></param>
		/// <param name="shift"></param>
		/// <param name="alt"></param>
		public void	AddCommand(string group, string name, KeyCode keyCode, bool control = false, bool shift = false, bool alt = false)
		{
			for (int i = 0; i < this.groups.Count; i++)
			{
				if (this.groups[i].name == group)
				{
					if (this.groups[i].ExistCommand(name) == false)
						this.groups[i].commands.Add(new InputCommand(name, keyCode, control, shift, alt));
					return;
				}
			}

			this.groups.Add(new InputsGroup(group, new InputCommand(name, keyCode, control, shift, alt)));
		}

		public bool	Check(string group, string commandName)
		{
			for (int i = 0; i < this.groups.Count; i++)
			{
				if (this.groups[i].name == group)
					return this.groups[i].Check(commandName);
			}

			return false;
		}
	}
}