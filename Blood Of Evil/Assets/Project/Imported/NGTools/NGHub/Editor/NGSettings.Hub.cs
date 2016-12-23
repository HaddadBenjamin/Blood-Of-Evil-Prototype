using UnityEngine;

namespace NGToolsEditor
{
	public partial class NGSettings : ScriptableObject
	{
		[HideInInspector]
		public MultiDataStorage	hubData = new MultiDataStorage();
	}
}