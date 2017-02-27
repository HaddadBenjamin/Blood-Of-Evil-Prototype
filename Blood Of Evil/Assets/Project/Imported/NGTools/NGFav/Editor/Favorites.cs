using System;
using System.Collections.Generic;

namespace NGToolsEditor.NGFav
{
	[Serializable]
	public sealed class Favorites
	{
		public string					name;
		public List<AssetsSelection>	favorites = new List<AssetsSelection>();
	}
}