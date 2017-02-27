using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NGToolsEditor.NGInspectorGadget
{
	internal sealed class CloneComponent
	{
		[MenuItem("CONTEXT/Component/Clone Component", priority = 501)]
		private static void	Clone(MenuCommand menuCommand)
		{
			Component	component = menuCommand.context as Component;

			if (ComponentUtility.CopyComponent(component) == true)
			{
				if (ComponentUtility.PasteComponentAsNew(component.gameObject) == true)
				{
					Component[]	c = component.gameObject.GetComponents<Component>();

					for (int i = 0; i < c.Length; i++)
					{
						if (c[i] == component)
						{
							while (i++ < c.Length - 2 && ComponentUtility.MoveComponentUp(c[c.Length - 1]) == true);
							break;
						}
					}
				}
				else
					EditorUtility.DisplayDialog(LC.G("CloneComponent_Error_Title"), LC.G("CloneComponent_Error_Message"), LC.G("Yes"));
			}
		}
	}
}