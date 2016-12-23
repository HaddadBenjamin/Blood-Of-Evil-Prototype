using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(GSpaceAttribute), true)]
	public class GSpaceDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			if (GroupDrawer.isMasterDrawing == false)
				return 0;
			return (this.attribute as GSpaceAttribute).space;
		}
	}
}