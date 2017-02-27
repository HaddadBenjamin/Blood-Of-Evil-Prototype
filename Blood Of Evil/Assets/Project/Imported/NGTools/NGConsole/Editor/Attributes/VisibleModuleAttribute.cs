using System;

namespace NGToolsEditor.NGConsole
{
	/// <summary>
	/// Exposes the module in a tab in the main menu bar.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class VisibleModuleAttribute : Attribute
	{
		public readonly int	position;

		/// <summary>
		/// </summary>
		/// <param name="position">A position in the bar. The lesser the more on the left.</param>
		public	VisibleModuleAttribute(int position)
		{
			this.position = position;
		}
	}
}