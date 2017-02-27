using System;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
	public abstract class EndMode
	{
		[NonSerialized]
		protected SampleStream	sampleLog;

		public virtual void	Init(SampleStream sampleLog)
		{
			this.sampleLog = sampleLog;
		}

		/// <summary>
		/// <para>Checks whether the sample should continue to be feeded.</para>
		/// <para>Returns false if the sample can still be feeded.</para>
		/// <para>Returns true if the sample is closed.</para>
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public abstract bool	CheckEnd(Row row);

		public virtual void	OnGUI()
		{
		}
	}
}