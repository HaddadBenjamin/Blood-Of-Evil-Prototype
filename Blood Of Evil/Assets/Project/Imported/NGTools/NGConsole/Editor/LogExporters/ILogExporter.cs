using System.Collections.Generic;

namespace NGToolsEditor.NGConsole
{
	public interface ILogExporter
	{
		/// <summary>Called when the wizard select this exporter.</summary>
		void	OnEnable();

		/// <summary>Called when the wizard focus another exporter.</summary>
		void	OnDestroy();

		/// <summary>Called right after wizard GUI</summary>
		void	OnGUI();

		/// <summary>
		/// Can be called to add an extra column to the actual row.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="attributes"></param>
		void	AddColumn(string key, string value, string[] attributes);

		/// <summary>
		/// Generates the export with <paramref name="rows"/> using the given <paramref name="settings"/>.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		string	Generate(List<Row> rows, ExportRowsEditorWindow.ExportSettings settings);
	}
}