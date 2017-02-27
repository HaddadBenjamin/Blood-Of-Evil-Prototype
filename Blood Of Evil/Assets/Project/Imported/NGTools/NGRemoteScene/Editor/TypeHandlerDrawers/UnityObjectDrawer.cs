using NGTools.Network;
using NGTools.NGRemoteScene;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[TypeHandlerDrawerFor(typeof(UnityObjectHandler))]
	internal sealed class UnityObjectDrawer : TypeHandlerDrawer
	{
		private const float	PickerButtonWidth = 20F;

		private static Vector2	dragOriginPosition;

		private BgColorContentAnimator	anim;
		private double					lastClick;

		public	UnityObjectDrawer(TypeHandler typeHandler) : base(typeHandler)
		{
		}

		public override void Draw(Rect r, DataDrawer data)
		{
			string		dataPath = data.GetPath();
			UnityObject	unityObject = data.value as UnityObject;

			if (Event.current.type == EventType.KeyDown &&
				Event.current.keyCode == KeyCode.Delete &&
				GUI.GetNameOfFocusedControl() == dataPath)
			{
				UnityObject	nullObject = new UnityObject(unityObject.type, 0);

				data.unityData.AddPacket(new ClientUpdateFieldValuePacket(dataPath, this.typeHandler.Serialize(nullObject.type, nullObject), this.typeHandler));

				Event.current.Use();
			}

			if (r.Contains(Event.current.mousePosition) == true)
			{
				if (Event.current.type == EventType.MouseDown)
				{
					UnityObjectDrawer.dragOriginPosition = Event.current.mousePosition;

					// Initialize drag data.
					DragAndDrop.PrepareStartDrag();

					DragAndDrop.objectReferences = new Object[0];
					DragAndDrop.SetGenericData("r", unityObject);
				}
				else if (Event.current.type == EventType.MouseDrag && (UnityObjectDrawer.dragOriginPosition - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
				{
					DragAndDrop.StartDrag("Dragging Game Object");
					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragUpdated)
				{
					UnityObject	dragItem = DragAndDrop.GetGenericData("r") as UnityObject;

					if (dragItem != null && dragItem.instanceID != unityObject.instanceID &&
						(dragItem.type == null || unityObject.type == null || unityObject.type.IsAssignableFrom(dragItem.type) == true))
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					}
					else
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

					Event.current.Use();
				}
				else if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					UnityObject	dragItem = DragAndDrop.GetGenericData("r") as UnityObject;

					data.unityData.AddPacket(new ClientUpdateFieldValuePacket(dataPath, this.typeHandler.Serialize(dragItem.type, dragItem), this.typeHandler));
				}
				else if (Event.current.type == EventType.Repaint &&
						 DragAndDrop.visualMode == DragAndDropVisualMode.Move)
				{
					Rect	r2 = r;

					r2.width += r2.x;
					r2.x = 0F;

					EditorGUI.DrawRect(r2, Color.yellow);
				}
			}

			float	x = r.x;
			float	width = r.width;

			r.width = UnityObjectDrawer.PickerButtonWidth;
			r.x = width - UnityObjectDrawer.PickerButtonWidth;

			if (Event.current.type == EventType.MouseDown &&
				r.Contains(Event.current.mousePosition) == true)
			{
				data.inspector.Hierarchy.PickupResource(unityObject.type, dataPath, UnityObjectDrawer.CreatePacket, unityObject.instanceID);
				Event.current.Use();
			}

			r.width = width;
			r.x = x;

			if (this.anim == null)
				this.anim = new BgColorContentAnimator(data.inspector.Repaint, 1F, 0F);

			if (data.inspector.Hierarchy.GetUpdateNotification(dataPath) == true)
				this.anim.Start();

			using (this.anim.Restorer(0F, .8F + this.anim.Value, 0F, 1F))
			{
				string	content;
				Rect	prefixRect = EditorGUI.PrefixLabel(r, new GUIContent(Utility.NicifyVariableName(data.name)));

				if (unityObject.instanceID != 0)
					content = unityObject.name + " (" + unityObject.type.Name + ")";
				else
					content = "None (" + unityObject.type.Name + ")";

				GUI.SetNextControlName(dataPath);
				if (GUI.Button(prefixRect, content, GeneralStyles.UnityObjectPicker) == true)
				{
					GUI.FocusControl(dataPath);

					if (unityObject.instanceID != 0 &&
						typeof(Object).IsAssignableFrom(unityObject.type) == true)
					{
						if (this.lastClick + Constants.DoubleClickTime < Time.realtimeSinceStartup)
							data.inspector.Hierarchy.PingObject(unityObject.gameObjectInstanceID);
						else
						{
							data.inspector.Hierarchy.SelectGameObject(unityObject.gameObjectInstanceID);
						}

						this.lastClick = Time.realtimeSinceStartup;
					}
				}
			}
		}

		private static Packet	CreatePacket(string valuePath, byte[] rawValue)
		{
			return new ClientUpdateFieldValuePacket(valuePath, rawValue, TypeHandlersManager.GetTypeHandler<UnityObject>());
		}
	}
}