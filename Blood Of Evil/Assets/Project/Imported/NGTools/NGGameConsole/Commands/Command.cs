using System;
using System.Collections.Generic;

namespace NGTools.NGGameConsole
{
	public class CommandNode
	{
		private enum Invalidate
		{
			Children = 1
		}

		public virtual bool		IsLeaf { get { return false; } }

		public readonly string	name;
		public readonly string	description;

		private string[]		commandNames;
		public string[]			CommandNames
		{
			get
			{
				if ((this.invalidate & Invalidate.Children) != Invalidate.Children)
				{
					this.commandNames = new string[this.children.Count];
					for (int i = 0; i < this.children.Count; i++)
						this.commandNames[i] = this.children[i].name;
				}
				return this.commandNames;
			}
		}

		public List<CommandNode>	children;

		protected object	instance;

		private Invalidate		invalidate;

		public	CommandNode(object instance, string name, string description)
		{
			this.instance = instance;
			this.name = name;
			this.description = description;
			this.children = new List<CommandNode>();
		}

		/// <summary>Do never call this function! It must be override!</summary>
		/// <param name="args"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Thrown when invoking this function.</exception>
		public virtual string	GetSetInvoke(params string[] args)
		{
			throw new InvalidOperationException("GetSetInvoke must not be be called.");
		}

		public void	AddChild(CommandNode command)
		{
			this.invalidate |= Invalidate.Children;
			this.children.Add(command);
		}

		public void	RemoveChild(CommandNode command)
		{
			this.invalidate |= Invalidate.Children;
			this.children.Remove(command);
		}
	}
}