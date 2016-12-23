using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[Serializable]
	public class TimeEndMode : EndMode
	{
		[Exportable]
		private float	time;

		public override bool	CheckEnd(Row row)
		{
			return Time.time >= this.time;
		}

		public override void	OnGUI()
		{
			this.time = EditorGUILayout.FloatField(LC.G("TimeStartMode_Time"), this.time);
			if (this.time < 0F)
				this.time = 0F;
		}
	}
}