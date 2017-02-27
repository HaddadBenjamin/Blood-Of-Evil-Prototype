using NGTools;
using UnityEditor;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(GSpaceAttribute), true)]
	internal sealed class GSpaceDrawer : DecoratorDrawer
	{
		public override float GetHeight()
		{
			if (GroupDrawer.isMasterDrawing == false)
				return 0;
			return (this.attribute as GSpaceAttribute).space;
		}
	}
}