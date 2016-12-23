//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright © 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace NJG
{
    [CustomEditor(typeof(Map))]
	public class MapInspector : Editor
	{
		protected Map m;
		//Texture mMask;
		Color mColor;
		//Material mMat;
		LeanTweenType mEasing;
		//bool usePanelForIcons;
		bool limitBounds;
		bool rotateWithPlayer;
		bool mouseWheel;
		Transform targetObj;
		string targetTag;
		float zoom;
		float zoomAmount;
		float minZoom;
		float maxZoom;
		float zoomSpeed;
		float mapBorderRadius;
		GameObject northIcon;
		//int northIconOffset;
		//KeyCode mapKey;
		//KeyCode zoomInKey;
		//KeyCode zoomOutKey;
		bool calculateBorder;
		//NJGMap.ShaderType mShader;
		Map.Pivot mPivot;
		Vector2 mMargin;
		//Vector2 mMapScale;
		bool panning;
		float panningSpeed;
		float panningSensitivity;
        LeanTweenType panningEase;
		bool panningMoveBack;
        int mBorderOffset;
        float lastRender;

        void DrawNotFound()
        {
            Map mp = (Map)m;
            if (mp != null)
            {
                if (mp.mapRenderer == null)
                {
                    mp.mapRenderer = mp.GetComponentInChildren<RawImage>();

                    if (mp.mapRenderer == null)
                    {
                        GUI.backgroundColor = Color.red;
                        EditorGUILayout.HelpBox("No UITexture found.", MessageType.Error);
                        GUI.backgroundColor = Color.white;

                        EditorGUILayout.Separator();
                        return;
                    }
                }
                else
                {
                    if (mp.material == null) mp.material = NJGEditorTools.GetMaterial(m, true);
                }
            }
        }

		/// <summary>
		/// Draw the inspector.
		/// </summary>

		public override void OnInspectorGUI()
		{       

			EditorGUIUtility.labelWidth = 120f;
			m = target as Map;

            
            //if (GUI.changed)
            //    EditorUtility.SetDirty(mp);

			//PrefabType type = PrefabUtility.GetPrefabType(m.gameObject);

			if (m.material == null) m.material = NJGEditorTools.GetMaterial(m, true);

            NJGEditorTools.DrawEditMap();

			DrawNotFound();

            Map.Type mapType = (Map.Type)EditorGUILayout.EnumPopup("Map Type", m.mapType);
            if (m.mapType != mapType)
            {
                m.mapType = mapType;
                NJGEditorTools.RegisterUndo("Map Type", m);
            }

			targetObj = (Transform)EditorGUILayout.ObjectField(new GUIContent("Map Target", "The object that this map is going to follow"), m.target, typeof(Transform), true);

			if (m.target == null)
			{
				if (Application.isPlaying)
					EditorGUILayout.HelpBox("No target has been found, assign the tag to your target or drag it manually to the target field.", MessageType.Warning);
				else
					EditorGUILayout.HelpBox("No target has been assigned, the target can be set automatically by using a tag or drag it manually to the target field.", MessageType.Warning);
			}

			targetTag = EditorGUILayout.TagField(new GUIContent("Target Tag", "Assign a tag to auto search for the Map Target"), m.targetTag);

            GUILayout.BeginHorizontal();
            bool isVisible = EditorGUILayout.Toggle("Visible", m.isVisible, GUILayout.Width(140f));
            GUI.contentColor = isVisible ? Color.cyan : Color.gray;
            EditorGUILayout.LabelField("Toggle visbility.");
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			limitBounds = EditorGUILayout.Toggle("Map Bounds", m.limitBounds, GUILayout.Width(140f));
			GUI.contentColor = limitBounds ? Color.cyan : Color.gray;
			EditorGUILayout.LabelField("Prevent map to display beyond borders.");
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			rotateWithPlayer = EditorGUILayout.Toggle("Lock Rotation", m.rotateWithPlayer, GUILayout.Width(140f));
			GUI.contentColor = rotateWithPlayer ? Color.cyan : Color.gray;
			EditorGUILayout.LabelField("Makes the map follow target rotation.");
			GUI.contentColor = Color.white;
			GUILayout.EndHorizontal();

            ShowSelector("Toggle Key", m.toggleKey, delegate(KeyCode k) { NJGMap.instance.keysInUse[0] = m.toggleKey = k; EditorUtility.SetDirty(m); });
			ShowSelector("Lock Key", m.lockKey, delegate(KeyCode k) { NJGMap.instance.keysInUse[1] = m.lockKey = k; EditorUtility.SetDirty(m); });

            

            /*mShader = (NJGMap.ShaderType)EditorGUILayout.EnumPopup("Shader Type", m.shaderType);
            if (mShader == NJGMap.ShaderType.ColorMask)
            {
                EditorGUILayout.HelpBox("Use the camera background color for masking", MessageType.Info);
                //m.colorMask = EditorGUILayout.ColorField("Color Mask", m.colorMask);
            }

            if (m.shaderType != mShader)
            {
                m.shaderType = mShader;
                Shader s = Shader.Find("Ninjutsu Games/Map TextureMask");
                switch (mShader)
                {
                    case NJGMap.ShaderType.TextureMask:
                        s = Shader.Find("Ninjutsu Games/Map TextureMask");
                        NJGMap.instance.fow.enabled = false;
                        break;
                    case NJGMap.ShaderType.ColorMask:
                        s = Shader.Find("Ninjutsu Games/Map ColorMask");
                        m.material.SetColor("_MaskColor", NJGMap.instance.cameraBackgroundColor);
                        NJGMap.instance.fow.enabled = false;
                        break;
                    case NJGMap.ShaderType.FOW:
                        s = Shader.Find("Ninjutsu Games/Map FOW");
                        NJGMap.instance.fow.enabled = true;
                        break;
                }
                m.material.shader = s;
                if (m.mapRenderer != null)
                {
                    m.mapRenderer.material.shader = s;
                }
                m.mapRenderer.enabled = false;
                m.mapRenderer.enabled = true;
                NJGEditorTools.RegisterUndo("Map Shader Type", m);
            }*/



            bool shouldBeOn = NJGEditorTools.DrawHeader("UI Settings");
			if (shouldBeOn)
			{
				Color c = Color.grey;
				c.a = 0.5f;
				GUI.backgroundColor = c;
				NJGEditorTools.BeginContents();
				GUI.backgroundColor = Color.white;

                /*mPivot = (Map.Pivot)EditorGUILayout.EnumPopup("Alignment", m.pivot);
				if (m.pivot != mPivot)
				{
					m.pivot = mPivot;
					EditorUtility.SetDirty(m);
					m.UpdateAlignment();
				}*/

                Sprite mMask = (Sprite)EditorGUILayout.ObjectField("Mask", m.maskTexture, typeof(Sprite), false);

                if (mMask != m.maskTexture)
                {
                    m.maskTexture = mMask;
                    m.material.SetTexture("_Mask", m.maskTexture.texture);
                    m.mapRenderer.material.SetTexture("_Mask", m.maskTexture.texture);
                    NJGEditorTools.RegisterUndo("Map Setting", m);
                }

                EditorGUILayout.Space();

                m.mapColor = EditorGUILayout.ColorField("Color", m.mapColor);

				if (mColor != m.mapColor)
				{
					mColor = m.mapColor;
					if (m.material != null)
						m.material.color = m.mapColor;

                    m.mapRenderer.enabled = false;
                    m.mapRenderer.enabled = true;
					NJGEditorTools.RegisterUndo("Map Setting", m);
				}

				//mMapScale = EditorGUILayout.Vector2Field("Dimensions", m.mapScale);

				/*if (m.mapScale != mMapScale)
				{
					mMapScale.x = (int)mMapScale.x;
					mMapScale.y = (int)mMapScale.y;
					m.mapScale = mMapScale;
					NJGEditorTools.RegisterUndo("Map Setting", m);
					//m.UpdateAlignment();
				}*/

				/*mMargin = EditorGUILayout.Vector2Field("Margin", m.margin);

				if (m.margin != mMargin)
				{
					m.margin.x = (int)mMargin.x;
					m.margin.y = (int)mMargin.y;
					NJGEditorTools.RegisterUndo("Map Setting", m);
					m.UpdateAlignment();
				}*/

				GUILayout.BeginHorizontal();
				//EditorGUILayout.LabelField(new GUIContent("Culling Radius", "If icons get farther this radius they will dissapear"), GUILayout.Width(116f));
				//GUI.enabled = !m.calculateBorder;

				//mapBorderRadius = EditorGUILayout.FloatField(m.mapBorderRadius, GUILayout.Width(158f));

				//GUI.enabled = true;
				//calculateBorder = EditorGUILayout.Toggle(new GUIContent("Automatic", "Check this option if you want this value to be autmatically calculated at start."), m.calculateBorder);

				EditorGUIUtility.labelWidth = 120f;
				GUILayout.EndHorizontal();

				northIcon = (GameObject)EditorGUILayout.ObjectField(new GUIContent("North Icon", "Optional north icon. Will be automatically placed if its assigned."), m.northIcon, typeof(GameObject), true);
				//if (northIcon != null)
				//	northIconOffset = EditorGUILayout.IntField(new GUIContent("North Icon Offset", "Adjust the north icon distance from map border"), m.northIconOffset);
				NJGEditorTools.EndContents();
				EditorGUILayout.Separator();
			}
			shouldBeOn = NJGEditorTools.DrawHeader("Zoom Settings");
			if (shouldBeOn)
			{
				Color c = Color.grey;
				c.a = 0.5f;
				GUI.backgroundColor = c;
				NJGEditorTools.BeginContents();
				GUI.backgroundColor = Color.white;

				//EditorGUILayout.LabelField("Zoom Settings", EditorStyles.boldLabel);

				GUILayout.BeginHorizontal();
				mouseWheel = EditorGUILayout.Toggle("Mouse Wheel", m.mouseWheelEnabled, GUILayout.Width(140f));
				GUI.contentColor = mouseWheel ? Color.cyan : Color.gray;
				EditorGUILayout.LabelField("Enable Mouse Wheel zoom.");
				GUI.contentColor = Color.white;
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				zoom = EditorGUILayout.Slider(new GUIContent("Zoom", "Current zoom level"), m.zoom, m.minZoom, m.maxZoom);
				if (m.zoom != zoom)
				{
                    if (!LeanTween.isTweening(m.gameObject)) m.zoom = Mathf.Clamp(zoom, m.minZoom, m.maxZoom);
					NJGEditorTools.RegisterUndo("Map Setting", m);
				}
				GUILayout.EndHorizontal();

				zoomAmount = EditorGUILayout.Slider(new GUIContent("Amount", "How much should zoom in/out when zoom function is called."), m.zoomAmount, 0.01f, 5);

				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Range", "Min and Max level of zoom"), GUILayout.Width(116.0f));
				minZoom = EditorGUILayout.FloatField(m.minZoom, GUILayout.Width(25.0f));
				EditorGUILayout.MinMaxSlider(ref minZoom, ref maxZoom, 1, 30);
				maxZoom = EditorGUILayout.FloatField(m.maxZoom, GUILayout.Width(25.0f));
				//minZoom = Mathf.Round(minZoom);
				//maxZoom = Mathf.Round(maxZoom);
				GUILayout.EndHorizontal();

                mEasing = (LeanTweenType)EditorGUILayout.EnumPopup("Easing", m.zoomEasing);
				if (m.zoomEasing != mEasing)
				{
					m.zoomEasing = mEasing;
					NJGEditorTools.RegisterUndo("Map Setting", m);
				}

				zoomSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "Zoom animation speed"), m.zoomSpeed, 0f, 2f);

				ShowSelector("Zoom In Key", m.zoomInKey, delegate(KeyCode k) { NJGMap.instance.keysInUse[2] = m.zoomInKey = k; NJGEditorTools.RegisterUndo("Map Setting", m); });
				ShowSelector("Zoom Out Key", m.zoomOutKey, delegate(KeyCode k) { NJGMap.instance.keysInUse[3] = m.zoomOutKey = k; NJGEditorTools.RegisterUndo("Map Setting", m); });

				NJGEditorTools.EndContents();
				EditorGUILayout.Separator();
			}

			shouldBeOn = NJGEditorTools.DrawHeader("Panning Settings");
			if (shouldBeOn)
			{
				Color c = Color.grey;
				c.a = 0.5f;
				GUI.backgroundColor = c;
				NJGEditorTools.BeginContents();
				GUI.backgroundColor = Color.white;

				panning = EditorGUILayout.Toggle("Enabled", m.panning, GUILayout.Width(140f));
				GUI.enabled = panning;
                panningEase = (LeanTweenType)EditorGUILayout.EnumPopup("Easing", m.panningEasing);
				panningSpeed = EditorGUILayout.Slider(new GUIContent("Speed", "How fast the panning should move"), m.panningSpeed, 0, 5);
				panningSensitivity = EditorGUILayout.Slider(new GUIContent("Sensitivy", "How fast the panning should respond on mouse move"), m.panningSensitivity, 0.1f, 10f);
				panningMoveBack = EditorGUILayout.Toggle(new GUIContent("Return on Release", "Moves back the panning to its original position"), m.panningMoveBack, GUILayout.Width(140f));
				GUI.enabled = true;
				NJGEditorTools.EndContents();
				EditorGUILayout.Separator();
			}

			if (m.limitBounds != limitBounds ||
				m.rotateWithPlayer != rotateWithPlayer ||
				m.target != targetObj ||
				m.targetTag != targetTag ||
				m.minZoom != minZoom ||
				m.maxZoom != maxZoom ||
				m.zoomSpeed != zoomSpeed ||
				m.northIcon != northIcon ||
				//m.northIconOffset != northIconOffset ||
				m.mouseWheelEnabled != mouseWheel ||
				m.panning != panning ||
				m.panningSpeed != panningSpeed ||
				m.panningEasing != panningEase ||
				m.panningMoveBack != panningMoveBack ||
				m.panningSensitivity != panningSensitivity ||
				m.zoomAmount != zoomAmount ||
                m.isVisible != isVisible)
			{
				m.zoomAmount = zoomAmount;
				m.panningSensitivity = panningSensitivity;
				m.panningMoveBack = panningMoveBack;
				m.panning = panning;
				m.panningSpeed = panningSpeed;
				m.panningEasing = panningEase;
				m.limitBounds = limitBounds;
				m.rotateWithPlayer = rotateWithPlayer;
				m.target = targetObj;
				m.targetTag = targetTag;
				m.minZoom = minZoom;
				m.maxZoom = maxZoom;
				m.zoomSpeed = zoomSpeed;
				m.northIcon = northIcon;
				//m.northIconOffset = northIconOffset;
                m.mouseWheelEnabled = mouseWheel;
                m.isVisible = isVisible;
				NJGEditorTools.RegisterUndo("Changed Minimap Settings", m);
			}

            if (m.mapRenderer.texture == null && !Application.isPlaying)
            {
                MapRenderer.instance.cam.targetTexture = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Ninjutsu Games/NJG MiniMap/Common/Materials/MapPreview.renderTexture", typeof(RenderTexture)) as RenderTexture;
                RenderTexture.active = MapRenderer.instance.cam.targetTexture;

                m.mapRenderer.texture = MapRenderer.instance.cam.targetTexture;

                MapRenderer.instance.cam.Render();
                MapRenderer.instance.ConfigCamera();
                MapRenderer.instance.cam.Render();
            }
            else if (Application.isPlaying && MapRenderer.instance.cam.targetTexture != null)
            {
                MapRenderer.instance.cam.targetTexture = RenderTexture.active = null;
            }

            if (!Application.isPlaying && Time.time > lastRender)
            {
                lastRender = Time.time + 1f;
                MapRenderer.instance.cam.Render();
                MapRenderer.instance.ConfigCamera();
                MapRenderer.instance.cam.Render();
                Debug.Log("ReRender");
            }

            if (GUI.changed)
			{
				EditorUtility.SetDirty(m);
			}
		}

		void ShowSelector(string fieldName, KeyCode key, KeySelector.CallbackKeyCode callback)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(fieldName, GUILayout.Width(117f));

			if (GUILayout.Button(key.ToString(), "MiniPullDown"))
			{
                KeySelector.Show(fieldName, key, NJGMap.instance.keysInUse, callback);
			}
			GUILayout.EndHorizontal();
		}
	}
}
