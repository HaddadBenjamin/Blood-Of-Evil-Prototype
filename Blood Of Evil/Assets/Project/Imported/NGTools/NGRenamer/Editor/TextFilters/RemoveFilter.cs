using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	internal sealed class RemoveFilter : TextFilter
	{
		public int	firstN;
		public int	lastN;
		public int	startPosition;
		public int	endPosition;

		public	RemoveFilter(NGRenamerWindow renamer) : base(renamer, "Remove", 100)
		{
		}

		public override void	OnGUI()
		{
			using (LabelWidthRestorer.Get(60F))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				{
					this.firstN = EditorGUILayout.IntField("First N", this.firstN);
					if (this.firstN < 0)
						this.firstN = 0;
					this.lastN = EditorGUILayout.IntField("Last N", this.lastN);
					if (this.lastN < 0)
						this.lastN = 0;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				{
					this.startPosition = EditorGUILayout.IntField("From", this.startPosition);
					if (this.startPosition < 0)
						this.startPosition = 0;
					this.endPosition = EditorGUILayout.IntField("To", this.endPosition);
					if (this.endPosition < this.startPosition)
						this.endPosition = this.startPosition;
				}
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.enable = true;
					this.renamer.Invalidate();
				}
			}
		}

		public override string	Filter(string input)
		{
			if (this.firstN > 0)
				input = input.Substring(this.firstN);

			if (this.lastN > 0)
				input = input.Substring(0, input.Length - this.lastN);

			if (this.startPosition > 0 && this.startPosition < input.Length)
			{
				int	count = this.endPosition - this.startPosition + 1;

				if (this.startPosition + count > input.Length)
					count = input.Length - this.startPosition;

				input = input.Remove(this.startPosition, count);
			}

			return input;
		}
	}
}