using UnityEditor.AnimatedValues;
using UnityEngine.Events;

namespace NGToolsEditor
{
	public class GUITimer
	{
		public float	Value { get { return this.af.value; } }

		public AnimFloat	af;
		private float		value;
		private float		target;

		public	GUITimer(UnityAction update, float value, float target)
		{
			this.af = new AnimFloat(0F, update);
			this.af.speed = 1F;
			this.af.target = target;

			this.value = value;
			this.target = target;
		}

		/// <summary>
		/// Initializes the animation. Value will now be updated.
		/// </summary>
		public void	Start()
		{
			this.af.value = this.value;
			this.af.target = this.target;
		}
	}
}