using System;
using UnityEditor;
using UnityEditorInternal;

namespace NGToolsEditor.NGHub
{
	using UnityEngine;

	[Serializable, Category("Scene")]
	internal sealed class LoadSceneComponent : HubComponent
	{
		[Exportable]
		public string	scene;
		[Exportable]
		public string	alias;

		[NonSerialized]
		private GUIContent	content;
		[NonSerialized]
		private Texture		image;
		[NonSerialized]
		private GUIStyle	button;

		public	LoadSceneComponent() : base("Load Scene",  true, true)
		{
		}

		public override void	Init(NGHubWindow hub)
		{
			base.Init(hub);

			this.image = InternalEditorUtility.GetIconForFile(".unity");
			this.content = new GUIContent(string.IsNullOrEmpty(this.alias) == true ? this.scene : this.alias, this.scene);
		}

		public override void	OnPreviewGUI(Rect r)
		{
			GUI.Label(r, "Scene \"" + this.scene + "\"");
		}

		public override void	OnEditionGUI()
		{
			EditorGUI.BeginChangeCheck();
			this.scene = EditorGUILayout.TextField("Scene Path", this.scene);
			this.alias = EditorGUILayout.TextField("Alias", this.alias);
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.content.text = string.IsNullOrEmpty(this.alias) == true ? this.scene : this.alias;
				this.content.tooltip = this.scene;
			}
		}

		public override void	OnGUI()
		{
			if (this.button == null)
				this.button = new GUIStyle(GUI.skin.button);

			float	w = this.button.CalcSize(this.content).x;

			if (this.image == null)
				this.button.padding.left = GUI.skin.button.padding.left;
			else
			{
				this.button.padding.left = (int)this.hub.height;
				w += 12F; // Remove texture width, because Button calculates using the whole height.
			}

			Rect	r = GUILayoutUtility.GetRect(w, this.hub.height, GUI.skin.button);

			if (Event.current.type == EventType.MouseDrag &&
				Utility.position2D != Vector2.zero &&
				DragAndDrop.GetGenericData(Utility.DragObjectDataName) != null &&
				(Utility.position2D - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance)
			{
				Utility.position2D = Vector2.zero;
				DragAndDrop.StartDrag("Drag Scene");
				Event.current.Use();
			}
			else if (Event.current.type == EventType.MouseUp && r.Contains(Event.current.mousePosition) == true)
			{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				EditorApplication.OpenScene(this.scene);
#else
				UnityEditor.SceneManagement.EditorSceneManager.OpenScene(this.scene);
#endif
			}
			else if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition) == true)
			{
				Utility.position2D = Event.current.mousePosition;
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[] { AssetDatabase.LoadMainAssetAtPath(this.scene) };
				DragAndDrop.SetGenericData(Utility.DragObjectDataName, 1);
			}

			GUI.Button(r, this.content, this.button);
			if (this.image != null)
			{
				r = GUILayoutUtility.GetLastRect();

				r.x += 4F;
				r.width = r.height;
				GUI.DrawTexture(r, this.image);
			}
		}

		public override void	InitDrop(NGHubWindow hub)
		{
			base.InitDrop(hub);

			this.scene = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
			this.alias = DragAndDrop.objectReferences[0].name;
			this.image = InternalEditorUtility.GetIconForFile(".unity");
			this.content = new GUIContent(string.IsNullOrEmpty(this.alias) == true ? this.scene : this.alias, this.scene);
		}

		private static bool		CanDrop()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (DragAndDrop.objectReferences.Length > 0)
				return AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]).EndsWith(".unity", true, System.Globalization.CultureInfo.CurrentCulture);
			return false;
#else
			return DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is SceneAsset;
#endif
		}
	}
}