using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	internal sealed class AddFilter : TextFilter
	{
		public string	text;
		public int		position;
		public string	prefix;
		public string	suffix;

		public	AddFilter(NGRenamerWindow renamer) : base(renamer, "Add", 50)
		{
		}

		public override void	OnGUI()
		{
			using (LabelWidthRestorer.Get(70F))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				{
					this.text = EditorGUILayout.TextField("Insert", this.text);
					using (LabelWidthRestorer.Get(25F))
					{
						this.position = EditorGUILayout.IntField("At", this.position);
					}
				}
				EditorGUILayout.EndHorizontal();

				this.prefix = EditorGUILayout.TextField("Prefix", this.prefix);
				this.suffix = EditorGUILayout.TextField("Suffix", this.suffix);
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.enable = true;
					this.renamer.Invalidate();
				}
			}
		}

		public override string	Filter(string input)
		{
			if (string.IsNullOrEmpty(this.text) == false)
			{
				int	pos = this.position;

				if (pos < 0)
				{
					pos = input.Length + pos + 1;
					if (pos < 0)
						pos = 0;
				}
				else if (pos >= input.Length)
					pos = input.Length;

				input = input.Insert(pos, this.text);
			}

			if (string.IsNullOrEmpty(this.prefix) == false)
				input = this.prefix + input;

			if (string.IsNullOrEmpty(this.suffix) == false)
				input = input + this.suffix;

			return input;
		}
	}
}