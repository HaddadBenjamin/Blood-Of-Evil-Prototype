using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	internal sealed class ExtensionFilter : TextFilter
	{
		public enum Operation
		{
			Lower,
			Upper,
			Add,
			Remove
		}

		public Operation	@operator;
		public string		extension;

		public	ExtensionFilter(NGRenamerWindow renamer) : base(renamer, "Extension", 20)
		{
		}

		public override void	OnGUI()
		{
			using (LabelWidthRestorer.Get(70F))
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				{
					this.@operator = (Operation)EditorGUILayout.EnumPopup(this.@operator);
					if (this.@operator == Operation.Add)
						this.extension = EditorGUILayout.TextField(this.extension);
				}
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.enable = true;
					this.renamer.Invalidate();
				}
			}
		}

		public override void	Highlight(string input, List<int> highlightedPositions)
		{
			if (this.@operator == Operation.Lower || this.@operator == Operation.Upper || this.@operator == Operation.Remove)
			{
				string	ext = this.GetExtension(input);

				if (string.IsNullOrEmpty(ext) == false)
				{
					for (int i = input.Length - ext.Length; i < input.Length; i++)
					{
						if (highlightedPositions.Contains(i) == false)
							highlightedPositions.Add(i);
					}
				}
			}
		}

		public override string	Filter(string input)
		{
			if (this.@operator == Operation.Add)
				return input + "." + extension;
			if (this.@operator == Operation.Lower)
			{
				string	ext = this.GetExtension(input);

				if (string.IsNullOrEmpty(ext) == false)
					return input.Substring(0, input.Length - ext.Length) + ext.ToLower();
			}
			if (this.@operator == Operation.Upper)
			{
				string	ext = this.GetExtension(input);

				if (string.IsNullOrEmpty(ext) == false)
					return input.Substring(0, input.Length - ext.Length) + ext.ToUpper();
			}
			if (this.@operator == Operation.Remove)
			{
				string	ext = this.GetExtension(input);

				if (string.IsNullOrEmpty(ext) == false)
					return input.Substring(0, input.Length - ext.Length);
			}

			return input;
		}

		private string	GetExtension(string input)
		{
			int	n = input.LastIndexOf('.');

			if (n != -1)
				return input.Substring(n);
			return string.Empty;
		}
	}
}