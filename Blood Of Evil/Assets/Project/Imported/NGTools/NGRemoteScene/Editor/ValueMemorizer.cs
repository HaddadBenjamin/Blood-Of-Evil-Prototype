using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public class ValueMemorizer<T>
	{
		public T		serverValue;
		public T		realTimeValue;
		public string	serverValueStringified;
		public bool		isPending;
		public float	labelWidth = EditorGUIUtility.labelWidth;

		public void	NewValue(T value)
		{
			if (this.isPending == true &&
				this.realTimeValue.Equals(value) == true)
			{
				this.isPending = false;
			}
		}

		public void	Set(T value)
		{
			this.isPending = true;

			this.realTimeValue = value;
			this.serverValueStringified = this.serverValue.ToString();
		}

		public T	Get(T @default)
		{
			this.serverValue = @default;
			return this.isPending == true ? this.realTimeValue : @default;
		}

		public virtual void	Draw(Rect r)
		{
			if (this.isPending == true)
			{
				r.x += this.labelWidth;
				r.y += 3F;
				r.width -= this.labelWidth;
				EditorGUI.LabelField(r, this.serverValueStringified, GeneralStyles.SmallLabel);
			}
		}
	}
}