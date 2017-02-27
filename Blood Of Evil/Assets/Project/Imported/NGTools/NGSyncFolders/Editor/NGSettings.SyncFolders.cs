using NGToolsEditor.NGSyncFolders;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		public List<Profile>	syncProfiles = new List<Profile>();
	}
}