#if UNITY_5
using NGTools;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(UnityEventBase), true)]
	[CustomPropertyDrawer(typeof(UnityEvent), true)]
	[CustomPropertyDrawer(typeof(UnityEvent<bool>), true)]
	[CustomPropertyDrawer(typeof(UnityEvent<int>), true)]
	[CustomPropertyDrawer(typeof(UnityEvent<float>), true)]
	[CustomPropertyDrawer(typeof(UnityEvent<string>), true)]
	public partial class GroupUnityEventDrawer : UnityEventDrawer
	{
		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.fieldInfo.IsDefined(typeof(InGroupAttribute), true) == true && GroupDrawer.isMasterDrawing == false)
				return -2;

			return base.GetPropertyHeight(property, label);
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (this.fieldInfo.IsDefined(typeof(InGroupAttribute), true) == true && GroupDrawer.isMasterDrawing == false)
				return;

			base.OnGUI(position, property, label);
		}
	}
}
#endif