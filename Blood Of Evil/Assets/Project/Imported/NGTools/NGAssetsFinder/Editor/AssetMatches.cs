using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGAssetsFinder
{
	internal sealed class AssetMatches
	{
		public enum Type
		{
			Reference,
			Component
		}

		public Object				origin;
		public List<Match>			matches;
		public List<AssetMatches>	children;
		public Type					type;

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
					for (int i = 0; i < this.children.Count; i++)
						this.children[i].Open = value;
				}
			}
		}


		public bool	allowSceneObject;

		public GUIContent	content;

		public	AssetMatches(Object origin)
		{
			this.origin = origin;
			this.matches = new List<Match>();
			this.children = new List<AssetMatches>();

			this.type = Type.Reference;

			this.open = true;
		}

		public void	PreCacheGUI()
		{
			for (int i = 0; i < this.matches.Count; i++)
				this.matches[i].PreCacheGUI();
			for (int i = 0; i < this.children.Count; i++)
				this.children[i].PreCacheGUI();

			this.content = new GUIContent();

			if (this.origin is GameObject)
			{
				PrefabType	t = PrefabUtility.GetPrefabType(this.origin);
				this.allowSceneObject = t != PrefabType.Prefab && t != PrefabType.ModelPrefab && t != PrefabType.None;

				if (this.matches.Count > 0 || this.children.Count > 0)
					this.content.text = this.origin.name;
				else
					this.content.text = this.origin.name + " (Component)";
			}
			else if (this.origin is Component)
			{
				PrefabType	t = PrefabUtility.GetPrefabType((this.origin as Component).gameObject);
				this.allowSceneObject = t != PrefabType.Prefab && t != PrefabType.ModelPrefab && t != PrefabType.None;
				if (this.matches.Count > 0 || this.children.Count > 0)
					this.content.text = Utility.NicifyVariableName(this.origin.GetType().Name);
				else
					this.content.text = Utility.NicifyVariableName(this.origin.GetType().Name) + " (Component)";
			}
			else
			{
				this.allowSceneObject = true;
			}

			this.content.image = Utility.GetIcon(this.origin.GetInstanceID());
		}
	}
}