//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NJG;

/// <summary>
/// Inspector class used to edit MapItems.
/// </summary>

[CustomEditor(typeof(MapItem))]
public class MapItemInspector : Editor 
{
	//UISprite mSprite;
	MapItem m;
    //float extraSpace = 0;

    Material uiMat;

    /// <summary>
    /// Draw the Map Marker inspector.
    /// </summary>

    public override void OnInspectorGUI()
	{
#if UNITY_4_3
		EditorGUIUtility.LookLikeControls(120f);
#else 
		EditorGUIUtility.labelWidth = 120f;
#endif

		m = target as MapItem;		

		NJGEditorTools.DrawEditMap();

		if (Map.miniMap != null)
		{
			string info = "Map Position: " + Map.miniMap.WorldToMap(m.transform.position).ToString();

			GUI.color = Color.cyan;
			EditorGUILayout.LabelField(info, EditorStyles.boldLabel);
			GUI.color = Color.white;
		}

		NJGEditorTools.DrawSeparator();

		int type = 0;
		

        Rect rect = GUILayoutUtility.GetRect(0, 0);
        rect.width = 16;
        rect.height = 16;
        rect.y += 5;
        rect.x = 110;

        if (uiMat == null) uiMat = new Material(Shader.Find("UI/Default"));

        EditorGUILayout.BeginHorizontal();
        {
            if (NJGMap.instance != null) type = NJGEditorTools.DrawList("Marker Type", NJGMap.instance.mapItemTypes, m.type);

            NJGMap.MapItemType m2 = NJGMap.instance.mapItems[type];
            GUI.color = m2.color;
            EditorGUI.DrawPreviewTexture(rect, m2.icon.texture, uiMat);
            GUI.color = Color.white;
        }
        EditorGUILayout.EndHorizontal();

        string tooltip = "You can use to display name + anything else you want.\nFor example: Ore [FF0000]+100 Mineral[-]";
		string content = "";

		if (type != 0)
		{
			EditorGUILayout.LabelField(new GUIContent("Tooltip Content", tooltip));
			content = EditorGUILayout.TextArea(m.content);

			GUI.backgroundColor = Color.gray;
			EditorGUILayout.HelpBox(tooltip, MessageType.Info);
			GUI.backgroundColor = Color.white;
		}		
		
		//m.drawDirection = EditorGUILayout.Toggle("Draw Direction Line", m.drawDirection);
		bool revealFOW = EditorGUILayout.Toggle("Reveal FOW", m.revealFOW);
		GUI.enabled = m.revealFOW;
		int revealDistance = (int)EditorGUILayout.Slider("Reveal Distance", m.revealDistance, 0, 100);
		GUI.enabled = true;
		GUI.backgroundColor = Color.gray;
		EditorGUILayout.HelpBox("Overrides Global Reveal Distance, if value = 0 it will use the global reveal distance.", MessageType.Info);
		GUI.backgroundColor = Color.white;

            /*if (NJGMap.instance != null)
            {
                if (NJGMap.instance.atlas != null)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Icon Sprite", GUILayout.Width(100.0f));

                    // Draw sprite preview.		
                    Material mat = NJGMap.instance.atlas.spriteMaterial;

                    if (mat != null)
                    {
                        Texture2D tex = mat.mainTexture as Texture2D;

                        if (tex != null)
                        {
                            UIAtlas.Sprite sprite = m.sprite;
                            Rect rect = sprite.outer;
                            if (NJGMap.instance.atlas.coordinates == UIAtlas.Coordinates.Pixels)
                            {
                                rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);
                            }

                            GUILayout.Space(4f);
                            GUILayout.Label("", GUILayout.Height(NJGMap.instance.iconSize));
                            GUI.color = m.color;
                            DrawSprite(tex, rect, null, false, NJGMap.instance.iconSize);
                            GUI.color = Color.white;

                            extraSpace = NJGMap.instance.iconSize * (float)sprite.outer.height / sprite.outer.width;
                        }

                    }
                    GUILayout.EndHorizontal();

                    extraSpace = Mathf.Max(0f, extraSpace - 30f);
                    //GUILayout.Space(extraSpace);
                }
                EditorGUILayout.Separator();			
            }*/

            EditorGUILayout.Separator();

		if (m.type != type ||
			m.content != content ||
			m.revealFOW != revealFOW ||
			m.revealDistance != revealDistance)
		{
            m.mColorSet = false;
            m.type = type;
			m.content = content;
			m.revealFOW = revealFOW;
            //m.enabled = m.revealFOW;
			m.revealDistance = revealDistance;
            m.enabled = false;
            m.enabled = true;
			NJGEditorTools.RegisterUndo("Map Item Properties", m);
		}
	}

	#region Draw Sprite Preview

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat) { return DrawSprite(tex, sprite, mat, true, 0); }

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat, bool addPadding)
	{
		return DrawSprite(tex, sprite, mat, addPadding, 0);
	}

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite(Texture2D tex, Rect sprite, Material mat, bool addPadding, int maxSize)
	{
		float paddingX = addPadding ? 4f / tex.width : 0f;
		float paddingY = addPadding ? 4f / tex.height : 0f;
		float ratio = (sprite.height + paddingY) / (sprite.width + paddingX);

		ratio *= (float)tex.height / tex.width;

		// Draw the checkered background
		Color c = GUI.color;

		Rect rect = GUILayoutUtility.GetRect(0f, 0f);
		rect.width = Screen.width - rect.xMin;
		rect.height = rect.width * ratio;

		rect = new Rect(85, rect.yMin + 0, NJGMap.instance.iconSize, NJGMap.instance.iconSize);

		GUI.color = c;

		if (maxSize > 0)
		{
			float dim = maxSize / Mathf.Max(rect.width, rect.height);
			rect.width *= dim;
			rect.height *= dim;
		}

		// We only want to draw into this rectangle
		if (Event.current.type == EventType.Repaint)
		{
			if (mat == null)
			{
				GUI.DrawTextureWithTexCoords(rect, tex, sprite);
			}
			else
			{
				// NOTE: DrawPreviewTexture doesn't seem to support BeginGroup-based clipping
				// when a custom material is specified. It seems to be a bug in Unity.
				// Passing 'null' for the material or omitting the parameter clips as expected.
				UnityEditor.EditorGUI.DrawPreviewTexture(sprite, tex, mat);
				//UnityEditor.EditorGUI.DrawPreviewTexture(drawRect, tex);
				//GUI.DrawTexture(drawRect, tex);
			}
			rect = new Rect(sprite.x + rect.x, sprite.y + rect.y, sprite.width, sprite.height);
		}
		return rect;
	}
	#endregion
}
