using System;
using UnityEditor;

namespace NGToolsEditor.NGRenamer
{
	internal sealed class NumberFilter : TextFilter
	{
		public enum Mode
		{
			Prefix,
			Suffix,
			PrefixAndSuffix,
			Insert,
		}

		public enum Base
		{
			Binary = 2,
			Octal = 8,
			Decimal = 10,
			Hex = 16,
		}

		public enum RomanNumerals
		{
			Upper,
			Lower
		}

		public Mode	mode = Mode.Prefix;
		public Base	baseNumber = Base.Decimal;
		public RomanNumerals	romanCase = RomanNumerals.Lower;
		public int	position;
		public int	start;
		public int	inc;
		public int	pad;

		public	NumberFilter(NGRenamerWindow renamer) : base(renamer, "Number", 10)
		{
		}

		public override void	OnGUI()
		{
			using (LabelWidthRestorer.Get(70F))
			{
				EditorGUI.BeginChangeCheck();
				using (LabelWidthRestorer.Get(40F))
				{
					EditorGUILayout.BeginHorizontal();
					{
						this.mode = (Mode)EditorGUILayout.EnumPopup("Mode", this.mode);

						if (this.mode == Mode.Insert)
						{
							using (LabelWidthRestorer.Get(25F))
							{
								this.position = EditorGUILayout.IntField("At", this.position);
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				
					EditorGUILayout.BeginHorizontal();
					{
						this.start = EditorGUILayout.IntField("Start", this.start);
						this.inc = EditorGUILayout.IntField("Inc", this.inc);
						this.pad = EditorGUILayout.IntField("Pad", this.pad);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					this.baseNumber = (Base)EditorGUILayout.EnumPopup("Base", this.baseNumber);
					using (LabelWidthRestorer.Get(110F))
					{
						this.romanCase = (RomanNumerals)EditorGUILayout.EnumPopup("Roman Numerals", this.romanCase);
					}
					EditorGUILayout.EndHorizontal();
				}

				if (EditorGUI.EndChangeCheck() == true)
				{
					this.enable = true;
					this.renamer.Invalidate();
				}
			}
		}

		public override string	Filter(string input)
		{
			string	number = Convert.ToString(this.start + NGRenamerWindow.DrawingIndex * this.inc, (int)this.baseNumber);

			if (number.Length < this.pad)
				number = new string('0', this.pad - number.Length) + number;

			if (this.romanCase == RomanNumerals.Lower)
				number = number.ToLower();
			else if (this.romanCase == RomanNumerals.Upper)
				number = number.ToUpper();

			if (this.mode == Mode.Insert)
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

				input = input.Insert(pos, number);
			}
			else if (this.mode == Mode.Prefix)
				input = number + input;
			else if (this.mode == Mode.Suffix)
				input = input + number;
			else if (this.mode == Mode.PrefixAndSuffix)
				input = number + input + number;

			return input;
		}
	}
}