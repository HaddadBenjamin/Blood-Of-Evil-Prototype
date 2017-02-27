using UnityEngine;

namespace NGToolsEditor.NGHierarchyEnhancer
{
	public abstract class DynamicObjectMenu
	{
		public int	priority;

		/// <summary>
		/// Draws custom GUI.
		/// </summary>
		/// <param name="r">Area available.</param>
		/// <param name="instance">Working Object.</param>
		/// <returns>End position in X axis of the next area.</returns>
		public abstract float	DrawHierarchy(Rect r, Object instance);
	}
}