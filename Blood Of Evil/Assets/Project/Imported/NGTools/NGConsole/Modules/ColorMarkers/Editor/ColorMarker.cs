using System;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Overwrite)]
	public sealed class ColorMarker
	{
		[Exportable("r", "g", "b", "a")]
		public Color		backgroundColor;
		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public GroupFilters	groupFilters = new GroupFilters();

		public	ColorMarker()
		{
			this.backgroundColor.a = 1F;
		}
	}
}