using System;
using UnityEditor;

namespace NGToolsEditor.NGHierarchyEnhancer
{
	using UnityEngine;

	internal sealed class GameObjectMenu : DynamicObjectMenu
	{
		public const float	Spacing = 3F;

		public static Color	AudioSourceBackgroundColor = new Color(45F / 255F, 45F / 255F, 45F / 255F);
		public static Color	RendererBackgroundColor = new Color(200F / 255F, 155F / 255F, 125F / 255F);
		public static Color	ParticleSystemBackgroundColor = new Color(100F / 255F, 125F / 255F, 85F / 255F);

		private static Type	inspectorWindowType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");

		public override float	DrawHierarchy(Rect r,  Object instance)
		{
			GameObject	go = instance as GameObject;

			if (go == null)
				return r.x;

			r.xMin = r.xMax - 20F;
			EditorGUI.BeginChangeCheck();
			EditorGUI.Toggle(r, go.activeSelf);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Preferences.Settings.hierarchy.selectionHoldModifiers > 0 &&
					((int)Event.current.modifiers & ((int)Preferences.Settings.hierarchy.selectionHoldModifiers >> 1)) != 0)
				{
					for (int i = 0; i < Selection.gameObjects.Length; i++)
					{
						if (Selection.gameObjects[i] == go ||
							AssetDatabase.Contains(Selection.gameObjects[i]) == true)
						{
							continue;
						}

						if (Selection.gameObjects[i].activeSelf == go.activeSelf)
						{
							Undo.RecordObject(Selection.gameObjects[i], "Toggle Game Object");
							Selection.gameObjects[i].SetActive(!go.activeSelf);
						}
					}
				}

				Undo.RecordObject(go, "Toggle Game Object");
				go.SetActive(!go.activeSelf);
			}

			EditorGUI.BeginChangeCheck();
			r.xMin = this.HandleAudioSources(r, go);
			r.xMin = this.HandleRenderers(r, go);
			r.xMin = this.HandleParticleSystems(r, go);
			if (EditorGUI.EndChangeCheck() == true)
				Utility.RepaintEditorWindow(GameObjectMenu.inspectorWindowType);

			return r.xMin;
		}

		private float	HandleParticleSystems(Rect r, GameObject gameObject)
		{
			ParticleSystem[]	sources = gameObject.GetComponents<ParticleSystem>();
			float				width = 24F;

			r.width = width;
			r.height = 12F;
			for (int i = 0; i < sources.Length; i++)
			{
				r.x -= r.width + GameObjectMenu.Spacing;

				if (Event.current.type == EventType.Repaint)
				{
					r.width = width;
					r.height = 16F;
					EditorGUI.DrawRect(r, GameObjectMenu.ParticleSystemBackgroundColor);
					r.height = 12F;
				}

				if (sources[i].isPlaying == true)
				{
					r.y += 2F;
					if (GUI.Button(r, "▮▮", GeneralStyles.CenterButton) == true)
					{
						Selection.activeGameObject = gameObject;
						sources[i].Pause(Event.current.control);
					}
					r.y -= 2F;
				}
				else
				{
					r.y += 2F;
					if (GUI.Button(r, "►") == true)
					{
						Selection.activeGameObject = gameObject;
						sources[i].Play(Event.current.control);
					}
					r.y -= 2F;
				}
			}

			return r.xMin;
		}

		private float	HandleRenderers(Rect r, GameObject gameObject)
		{
			Renderer[]	sources = gameObject.GetComponents<Renderer>();
			float		width = 24F;

			r.width = width;
			r.height = 12F;
			for (int i = 0; i < sources.Length; i++)
			{
				r.x -= r.width + GameObjectMenu.Spacing;

				if (Event.current.type == EventType.Repaint)
				{
					r.height = 16F;
					EditorGUI.DrawRect(r, GameObjectMenu.RendererBackgroundColor);
					r.height = 12F;
				}

				r.y += 2F;
				float	h = GeneralStyles.ToolbarButton.fixedHeight;
				GeneralStyles.ToolbarButton.fixedHeight = 12F;
				sources[i].enabled = GUI.Toggle(r, sources[i].enabled, "R", GeneralStyles.ToolbarButton);
				GeneralStyles.ToolbarButton.fixedHeight = h;
				r.y -= 2F;
			}

			return r.xMin;
		}

		private float	HandleAudioSources(Rect r, GameObject gameObject)
		{
			AudioSource[]	sources = gameObject.GetComponents<AudioSource>();
			float			width = 24F;

			r.width = width;
			for (int i = 0; i < sources.Length; i++)
			{
				r.x -= r.width + GameObjectMenu.Spacing;

				if (sources[i].isPlaying == true)
				{
					if (Event.current.type == EventType.Repaint)
					{
						r.width = width * 3;
						r.height = 16F;
						r.x -= width;
						EditorGUI.DrawRect(r, GameObjectMenu.AudioSourceBackgroundColor);
						r.height = 12F;
						r.x += width;
						r.width = width;
					}

					if (GUI.Button(r, "■", GeneralStyles.CenterButton) == true)
						sources[i].Stop();

					r.x -= r.width;

					if (GUI.Button(r, "▮▮", GeneralStyles.CenterButton) == true)
						sources[i].Pause();
				}
				else
				{
					if (Event.current.type == EventType.Repaint)
					{
						r.width = width * 2;
						r.height = 16F;
						r.x -= width;
						EditorGUI.DrawRect(r, GameObjectMenu.AudioSourceBackgroundColor);
						r.height = 12F;
						r.x += width;
						r.width = width;
					}

					GUI.enabled = sources[i].clip != null;
					r.y += 2F;
					if (GUI.Button(r, "►") == true)
						sources[i].Play();
					r.y -= 2F;
					GUI.enabled = true;
				}

				r.x -= r.width;
				r.y += 2F;
				r.height = 12F;
				float	h = GeneralStyles.ToolbarButton.fixedHeight;
				GeneralStyles.ToolbarButton.fixedHeight = 12F;
				sources[i].mute = GUI.Toggle(r, sources[i].mute, "M", GeneralStyles.ToolbarButton);
				GeneralStyles.ToolbarButton.fixedHeight = h;
				r.y -= 2F;
			}

			return r.xMin;
		}
	}
}