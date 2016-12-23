namespace NGToolsEditor
{
	public interface ISettingExportable
	{
		void	PreExport();
		void	PreImport();
		void	PostImport();
	}
}