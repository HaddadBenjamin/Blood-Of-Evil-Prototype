using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public abstract class AssetReader
	{
		public abstract bool	CanRead(int instanceID);
		public abstract void	ReadAsset(int instanceID);
	}

	[Serializable]
	[ExcludeFromExport]
	internal sealed class DropModule : Module
	{
		public List<int>	drops;
		//private int			workingDrop;

		public	DropModule()
		{
			this.name = "Drop";
			this.drops = new List<int>();
			//this.workingDrop = 0;
		}

		public override void	OnEnable(NGConsoleWindow editor, int id)
		{
			base.OnEnable(editor, id);

			this.console.wantsMouseMove = true;
		}

		public override void	OnDisable()
		{
			this.console.wantsMouseMove = false;
		}

		public override void	OnGUI(Rect r)
		{
			r.height = 100F;
			GUI.Box(r, GUIContent.none);
			if (r.Contains(Event.current.mousePosition))
			{
				GUI.Box(r, GUIContent.none);

				if (Event.current.type == EventType.DragUpdated)
					Event.current.Use();
				else if (Event.current.type == EventType.DragPerform)
				{
					Debug.Log("DragPerform");
					Event.current.Use();
				}

				DragAndDrop.visualMode = DragAndDropVisualMode.Move;
			}
			if (Event.current.type == EventType.DragExited)
				Debug.Log("DragExit");
		}

		public void	Clear()
		{
			this.drops.Clear();
		}
	}
}