using System.Collections.Generic;
using UnityEngine;

namespace NGTools.Tests
{
	public class TestPrefs : MonoBehaviour
	{
		public List<string>	keys;

		private string	k;
		private string	v;

		protected virtual void	Awake()
		{
			this.k = string.Empty;
			this.v = string.Empty;
		}

		protected virtual void	OnGUI()
		{
			Rect	r = new Rect(30F, 30F, 200F, 20F);

			this.k = GUI.TextField(r, this.k);
			r.x += r.width;

			this.v = GUI.TextField(r, this.v);
			r.x += r.width;

			if (GUI.Button(r, "Add Key/Value") == true)
			{
				PlayerPrefs.SetString(this.k, this.v);
				this.keys.Add(this.k);
			}

			r.x -= r.width;
			r.x -= r.width;
			r.y += r.height;
			r.y += r.height;

			r.height = 50F;

			for (int i = 0; i < this.keys.Count; i++)
			{
				if (GUI.Button(r, this.keys[i]) == true)
				{
					PlayerPrefs.DeleteKey(this.keys[i]);
					this.keys.RemoveAt(i);
					break;
				}
				r.x += r.width + 10F;

				GUI.Label(r, PlayerPrefs.GetString(this.keys[i], "NULL"), GUI.skin.label);
				r.x -= r.width + 10F;
				r.y += r.height;
			}

			r.y += r.height;
			r.width = 600F;
			if (GUI.Button(r, "Clear all PlayerPrefs") == true)
			{
				PlayerPrefs.DeleteAll();
			}
		}
	}
}