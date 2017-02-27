using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGFav
{
	using UnityEngine;

	[Serializable]
	public sealed class AssetsSelection
	{
		private static List<Object>	cacheObjects = new List<Object>();

		public Object	this[int i]
		{
			get
			{
				return this.refs[i].@object;
			}
		}

		public List<SelectionItem>	refs = new List<SelectionItem>();

		public	AssetsSelection(Object[] objects)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				try
				{
					this.refs.Add(new SelectionItem(objects[i]));
				}
				catch (MissingMethodException)
				{
				}
			}
		}

		public	AssetsSelection(int[] instanceIDs)
		{
			for (int i = 0; i < instanceIDs.Length; i++)
			{
				try
				{
					Object	obj = EditorUtility.InstanceIDToObject(instanceIDs[i]);

					if (obj != null)
						this.refs.Add(new SelectionItem(obj));
				}
				catch (MissingMethodException)
				{
				}
			}
		}

		public void	Select()
		{
			if (this.refs.Count == 1)
				UnityEditor.Selection.objects = new Object[] { this.refs[0].@object };
			else
			{
				AssetsSelection.cacheObjects.Clear();

				for (int i = 0; i < this.refs.Count; i++)
				{
					if (this.refs[i].@object != null)
						AssetsSelection.cacheObjects.Add(this.refs[i].@object);
				}

				UnityEditor.Selection.objects = AssetsSelection.cacheObjects.ToArray();
			}
		}

		public int	GetSelectionHash()
		{
			int	hash = 0;

			for (int i = 0; i < this.refs.Count; i++)
			{
				// Yeah, what? Is there a problem with my complex anti-colisionning hash function?
				if (this.refs[i].@object != null)
					hash += this.refs[i].@object.GetInstanceID();
			}

			return hash;
		}
	}
}