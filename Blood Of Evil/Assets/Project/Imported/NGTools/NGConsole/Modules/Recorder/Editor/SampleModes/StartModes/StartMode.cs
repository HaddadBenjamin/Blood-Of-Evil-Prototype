using System;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
	public abstract class StartMode
	{
		[NonSerialized]
		protected SampleStream	sampleLog;

		public virtual void	Init(SampleStream sampleLog)
		{
			this.sampleLog = sampleLog;
		}

		/// <summary>
		/// <para>Checks whether the sample can be feeded.</para>
		/// <para>Returns true if the sample is opened, the row is included.</para>
		/// <para>Returns false if the sample is closed.</para>
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public abstract bool	CheckStart(Row row);

		public virtual void	OnGUI()
		{
		}
	}
}