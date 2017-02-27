using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	[Exportable(ExportableAttribute.ArrayOptions.Overwrite)]
	public interface ILogFilter
	{
		bool	Enabled { get; set; }

		/// <summary>
		/// Is called when the filter is enabled or disabled.
		/// </summary>
		event Action	ToggleEnable;

		/// <summary>
		/// Defines whether the Row is accepted, refused or unhandled.
		/// </summary>
		/// <param name="row"></param>
		/// <returns>Returns -1</returns>
		FilterResult	CanDisplay(Row row);

		/// <summary>
		/// Draws filter in a single line.
		/// </summary>
		/// <param name="rect"></param>
		/// <returns></returns>
		void	OnGUI();

		/// <summary>
		/// <para>Adds an item to the given menu.</para>
		/// <para>Use the position to differentiate identical filters in the menu.</para>
		/// </summary>
		/// <param name="menu"></param>
		/// <param name="row"></param>
		/// <param name="position"></param>
		void	ContextMenu(GenericMenu menu, Row row, int position);
	}
}