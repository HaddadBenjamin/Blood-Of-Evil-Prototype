using System;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	internal sealed class MaxLogEndMode : EndMode
	{
		[Exportable]
		private int	endMaxLog;

		/// <summary>
		/// Checks if rows has reached the maximum count.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public override bool	CheckEnd(Row row)
		{
			return (this.sampleLog.rowsDrawer.Count >= this.endMaxLog - 1);
		}

		public override void	OnGUI()
		{
			this.endMaxLog = EditorGUILayout.IntField(LC.G("MaxSamples"), this.endMaxLog);
			if (this.endMaxLog < 1)
				this.endMaxLog = 1;
		}
	}
}