using NGTools;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGAssetsFinder
{
	internal sealed class Match
	{
		public bool			valid;
		public string		path;
		public List<Match>	subMatches = new List<Match>(0);
		public List<int>	arrayIndexes = new List<int>(0);

		public string	nicifiedPath;
		private bool	open;
		public bool		Open
		{
			get
			{
				return this.open;
			}
			set
			{
				this.open = value;
				if (Event.current != null && Event.current.alt == true)
				{
					for (int i = 0; i < this.subMatches.Count; i++)
						this.subMatches[i].Open = value;
				}
			}
		}

		public object			instance;
		public IFieldModifier	fieldModifier;

		public	Match(object instance, IFieldModifier fieldModifier)
		{
			this.path = fieldModifier.Name;
			this.instance = instance;
			this.fieldModifier = fieldModifier;
		}

		public void	PreCacheGUI()
		{
			this.nicifiedPath = Utility.NicifyVariableName(this.path);
			this.open = true;

			for (int i = 0; i < this.subMatches.Count; i++)
				this.subMatches[i].PreCacheGUI();
		}
	}
}