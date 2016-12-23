//----------------------------------------------
//            NJG MiniMap (NGUI)
// Copyright � 2013 - 2015 Ninjutsu Games LTD.
//----------------------------------------------

using NJG;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class NJGEditorTools : MonoBehaviour
{
    static public string contentFolder = "Ninjutsu Games/NJG MiniMap";

    static public Sprite GetDefaultMask(bool isMiniMap)
    {
        Sprite s = AssetDatabase.LoadAssetAtPath("Assets/" + contentFolder + "/Common/Mask Textures/" + (isMiniMap ? "CircleMask" : "ErodedMask") + ".png", typeof(Sprite)) as Sprite;
        return s;
    }

    static public Material GetMaterial(Map m, bool isMiniMap)
    {
        GetPath();

        //Material mat = (Material)AssetDatabase.LoadAssetAtPath(matFilename, typeof(Material));
        string matPath = isMiniMap ? "Assets/" + contentFolder + "/Common/Materials/MiniMap.mat" : "Assets/" + contentFolder + "/Common/Materials/WorldMap.mat";
        Material mat = (Material)AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
        m.material = mat;

        if (mat == null)
        {
            mat = new Material(Shader.Find("Ninjutsu Games/Map"));

            //string[] arr = EditorSceneManager.GetActiveScene().name.Split('/');
            //string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);

            string matFilename = "Assets/" + contentFolder + "/_Generated Content/" + EditorSceneManager.GetActiveScene().name + "_" + (isMiniMap ? "MiniMap" : "WorldMap") + ".mat";

            AssetDatabase.CreateAsset(mat, matFilename);

            Debug.Log((isMiniMap ? "MiniMap" : "WorldMap") + " material generated at: " + matFilename);

            m.material = mat;
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(mat);
        }

        if (m.maskTexture != null)
        {
            m.material.SetTexture("_Mask", m.maskTexture.texture);
        }
        else
        {
            m.maskTexture = AssetDatabase.LoadAssetAtPath("Assets/" + contentFolder + "/Common/Mask Textures/" + (isMiniMap ? "CircleMask" : "ErodedMask") + ".png", typeof(Sprite)) as Sprite;
            if (m.maskTexture != null) m.material.SetTexture("_Mask", m.maskTexture.texture);
        }

        if (!NJGMap.instance.generateMapTexture)
        {
            if (NJGMap.instance.userMapTexture != null) m.material.SetTexture("_Main", NJGMap.instance.userMapTexture);
        }
        else
        {
            if (NJGMap.instance.mapTexture != null) m.material.SetTexture("_Main", NJGMap.instance.mapTexture);
        }

        //Debug.Log("Material created");

        return mat;
    }

    /// <summary>
    /// Create an undo point for the specified objects.
    /// </summary>

    static public void RegisterUndo(string name, params Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			UnityEditor.Undo.RegisterUndo(objects, name);
#else
            UnityEditor.Undo.RecordObjects(objects, name);
#endif
            foreach (Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text) { return DrawHeader(text, text, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, bool forceOn) { return DrawHeader(text, text, forceOn); }

    /// <summary>
    /// Draw a distinctly different looking header label
    /// </summary>

    static public bool DrawHeader(string text, string key, bool forceOn)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;
#if UNITY_3_5
		if (!GUILayout.Toggle(true, text, "dragtab")) state = !state;
#else
        if (!GUILayout.Toggle(true, "<b><size=11>" + text + "</size></b>", "dragtab")) state = !state;
#endif
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);
        return state;
    }

    /// <summary>
    /// Begin drawing the content area.
    /// </summary>

    static public void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    /// <summary>
    /// End drawing the content area.
    /// </summary>

    static public void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    /*static public UITexture CreateMapTexture(Map m)
	{
		bool isMiniMap = m is UIMiniMap;

		//m.mapMaterial = AssetDatabase.LoadAssetAtPath("Assets/NinjutsuGames/NG GameMap/Materials/WorldMap.mat", typeof(Material)) as Material;

		//m.mapMaterial = GenerateMaterial(m);

		m.uiTexture = NGUITools.AddWidget<UITexture>(m.gameObject);
		m.uiTexture.depth = 0;
		m.uiTexture.name = isMiniMap ? "_MiniMapTexture" : "_WorldMapTexture";
		m.uiTexture.material = GetMaterial(m);

		m.uiTexture.cachedTransform.localPosition = new Vector3(0, 0, 0);
		m.uiTexture.cachedTransform.localScale = isMiniMap ? new Vector3(200, 200, 1) : new Vector3(700, 400, 1);		

		return m.uiTexture;
	}*/

    static public void DrawEditMap()
    {
        EditorGUILayout.Separator();
        if (NJGMap.instance == null)
        {
            GUI.backgroundColor = Color.red;
            EditorGUILayout.HelpBox("No Instance of Map was found.\nPlease create a new one.", MessageType.Error);
            GUI.backgroundColor = Color.white;
        }

        GUI.backgroundColor = Color.cyan;
        GUI.enabled = NJGMap.instance != null;
        if (GUILayout.Button(new GUIContent("Settings", "Click to edit the NJG MiniMap")))
        {
            /*if (NJGMap.instance == null)
				NJGMenu.AddNJGMiniMap();
			else*/
            Selection.activeObject = NJGMap.instance;
        }
        GUI.enabled = true;
        GUI.backgroundColor = Color.white;
        EditorGUILayout.Separator();
    }

    /*static public void CreateMap(bool isMiniMap)
	{
		GameObject parent = NGUIEditorTools.SelectedRoot();
		if (parent == null) parent = NJGMap.instance.GetComponentInChildren<UIPanel>() != null ? NJGMap.instance.GetComponentInChildren<UIPanel>().gameObject : null;
		GameObject go = NGUITools.AddChild(parent);
		go.name = isMiniMap ? "MiniMap" : "WorldMap";

		UITexture ut = null;
		int depth = 0;

		if (isMiniMap)
		{
			// Create MiniMap
			UIMiniMap mp = go.AddComponent<UIMiniMap>();
			ut = NJGEditorTools.CreateMapTexture(mp);
			depth = (int)Mathf.Abs(ut.cachedTransform.localPosition.z) > 0 ? -(int)Mathf.Abs(ut.cachedTransform.localPosition.z) : -1;

			// Add Anchor
			UIAnchor a = go.AddComponent<UIAnchor>();
			a.side = UIAnchor.Side.TopRight;

			// Title
			UILabel title = CreateLabel(go, NJGMap.instance.worldName);
			title.gameObject.AddComponent<UILabelWorldName>();
			title.cachedTransform.localPosition = new Vector3(0, ut.cachedTransform.localScale.y / 2, depth);
		}
		else
		{
			// Create WorldMap
			UIWorldMap mp = go.AddComponent<UIWorldMap>();
			ut = NJGEditorTools.CreateMapTexture(mp);
			depth = (int)Mathf.Abs(ut.cachedTransform.localPosition.z) > 0 ? -(int)Mathf.Abs(ut.cachedTransform.localPosition.z) : -1;

			// Add Anchor
			UIAnchor a = go.AddComponent<UIAnchor>();
			a.side = UIAnchor.Side.Center;

			// Close button
			GameObject button = CreateButton(go, "Close", 100, 34);
			button.transform.parent = go.transform;
			button.transform.localScale = Vector3.one;
			button.transform.localPosition = new Vector3(0, -ut.cachedTransform.localScale.y / 2, depth);
			button.AddComponent<UIButtonCloseMap>();

			// Title
			UILabel title = CreateLabel(go, NJGMap.instance.worldName);
			title.gameObject.AddComponent<UILabelWorldName>();
			title.cachedTransform.localPosition = new Vector3(0, ut.cachedTransform.localScale.y / 2, depth);
		}

		Selection.activeGameObject = go;
	}	
	*/
    /// <summary>
    /// Save the generated texture.
    /// </summary>

    static public Texture2D SaveTexture(Texture2D tex)
    {
        if (tex == null) return null;
        byte[] bytes = tex.EncodeToPNG();
        string path = GetPath();

        //string[] arr = EditorApplication.currentScene.Split('/');
        //string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);

        string fileName = EditorSceneManager.GetActiveScene().name + ".png";
        System.IO.File.WriteAllBytes(path + fileName, bytes);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string newPath = "Assets/" + contentFolder + "/_Generated Content/" + fileName;
        Texture2D tx = (Texture2D)AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));
        if (NJGMap.instance.optimize)
            EditorUtility.CompressTexture(tx, (TextureFormat)NJGMap.instance.textureFormat, NJGMap.instance.compressQuality);

        EditorGUIUtility.PingObject(tx);
        return tx;
    }

    static public Texture2D GetTexture()
    {
        //string[] arr = EditorApplication.currentScene.Split('/');
        //string scene = arr[arr.Length - 1].Replace(".unity", string.Empty);
        string fileName = EditorSceneManager.GetActiveScene().name + ".png";
        return (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/" + contentFolder + "/_Generated Content/" + fileName, typeof(Texture2D));
    }

    static public Vector2 GetGameViewSize()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView == null ? Vector2.zero : GetSizeOfMainGameView.Invoke(null, null);
        return (Vector2)Res;
    }

    static public EditorWindow GetMainGameView()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetMainGameView.Invoke(null, null);
        return (EditorWindow)Res;
    }

    static string GetPath()
    {
        //string[] arr = EditorApplication.currentScene.Split('/');
        string path = Application.dataPath + "/" + contentFolder + "/_Generated Content/";
        // + arr[arr.Length - 1].Replace(".unity", "/")

        //Debug.Log("path " + path);

        //AssetDatabase.CreateFolder("Assets/" + contentFolder, "_Generated Content");

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        return path;
    }

    /// <summary>
    /// Button creation function.
    /// </summary>

    //static string mButton = "Button";
    //static void OnButton(string val) { mButton = val; }

    /*static public GameObject CreateButton(GameObject go, string name, int w, int h)
	{
		if (NJGMap.instance.atlas != null)
		{
			int depth = NGUITools.CalculateNextDepth(go);
			go = NGUITools.AddChild(go);
			go.name = "Button - " + name;

			UISlicedSprite bg = NGUITools.AddWidget<UISlicedSprite>(go);
			//bg.type = UISprite.Type.Sliced;
			bg.name = "Background";
			bg.depth = depth;
			bg.atlas = NJGMap.instance.atlas;
			bg.spriteName = mButton;
			bg.transform.localScale = new Vector3(w, h, 1f);
			bg.MakePixelPerfect();

			if (NGUISettings.bitmapFont != null)
			{
				UILabel lbl = NGUITools.AddWidget<UILabel>(go);
				lbl.font = NGUISettings.bitmapFont;
				lbl.text = name;
				lbl.MakePixelPerfect();
			}

			// Add a collider
			NGUITools.AddWidgetCollider(go);

			// Add the scripts
			go.AddComponent<UIButton>().tweenTarget = bg.gameObject;
			go.AddComponent<UIButtonScale>();
			go.AddComponent<UIButtonOffset>();
			go.AddComponent<UIButtonSound>();
		}

		return go;
	}

	/// <summary>
	/// Label creation function.
	/// </summary>

	static public UILabel CreateLabel(GameObject go, string content)
	{
		if (NGUISettings.ambigiousFont != null)
		{
			UILabel lbl = NGUITools.AddWidget<UILabel>(go);
			lbl.bitmapFont = (UIFont)NGUISettings.ambigiousFont;
			lbl.text = content;
			lbl.color = Color.white;
			lbl.MakePixelPerfect();
			Selection.activeGameObject = lbl.gameObject;
			return lbl;
		}
		return null;
	}*/

    /// <summary>
    /// Convenience function that displays a list of sprites and returns the selected value.
    /// </summary>

    static public string DrawList(string field, string[] list, string selection, params GUILayoutOption[] options)
    {
        if (list != null && list.Length > 0)
        {
            int index = 0;
            if (string.IsNullOrEmpty(selection)) selection = list[0];

            // We need to find the sprite in order to have it selected
            if (!string.IsNullOrEmpty(selection))
            {
                for (int i = 0; i < list.Length; ++i)
                {
                    if (selection.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
                    {
                        index = i;
                        break;
                    }
                }
            }

            // Draw the sprite selection popup
            index = string.IsNullOrEmpty(field) ?
                EditorGUILayout.Popup(index, list, options) :
                EditorGUILayout.Popup(field, index, list, options);

            return list[index];
        }
        return null;
    }

    /// <summary>
    /// Returns a blank usable 1x1 white texture.
    /// </summary>

    static public Texture2D blankTexture
    {
        get
        {
            return EditorGUIUtility.whiteTexture;
        }
    }

    static Texture2D mBackdropTex;

    /// <summary>
    /// Returns a usable texture that looks like a dark checker board.
    /// </summary>

    static public Texture2D backdropTexture
    {
        get
        {
            if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
                new Color(0.1f, 0.1f, 0.1f, 0.5f),
                new Color(0.2f, 0.2f, 0.2f, 0.5f));
            return mBackdropTex;
        }
    }

    /// <summary>
    /// Create a checker-background texture
    /// </summary>

    static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";
        tex.hideFlags = HideFlags.DontSave;

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    /// <summary>
    /// Draw a visible separator in addition to adding some padding.
    /// </summary>

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    static Texture2D mContrastTex;

    /// <summary>
    /// Returns a usable texture that looks like a high-contrast checker board.
    /// </summary>

    static public Texture2D contrastTexture
    {
        get
        {
            if (mContrastTex == null) mContrastTex = CreateCheckerTex(
                new Color(0f, 0.0f, 0f, 0.5f),
                new Color(1f, 1f, 1f, 0.5f));
            return mContrastTex;
        }
    }

    /// <summary>
    /// Convenience function that displays a list of sprites and returns the selected value.
    /// </summary>

    static public int DrawList(string field, string[] list, int selection, params GUILayoutOption[] options)
    {
        if (list != null && list.Length > 0)
        {
            int index = 0;

            if (selection < 0) selection = 0;

            if (selection <= list.Length)
            {
                if (list[selection] != null)
                    index = selection;
            }

            // Draw the sprite selection popup
            index = string.IsNullOrEmpty(field) ?
                EditorGUILayout.Popup(index, list, options) :
                EditorGUILayout.Popup(field, index, list, options);

            return index;
        }
        return -1;
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(null, serializedObject, property, false, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, false, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawPaddedProperty(SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(null, serializedObject, property, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawPaddedProperty(string label, SerializedObject serializedObject, string property, params GUILayoutOption[] options)
    {
        return DrawProperty(label, serializedObject, property, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
    {
        SerializedProperty sp = serializedObject.FindProperty(property);

        if (sp != null)
        {
            if (padding) EditorGUILayout.BeginHorizontal();

            if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
            else EditorGUILayout.PropertyField(sp, options);

            if (padding)
            {
                GUILayout.Space(18f);
                EditorGUILayout.EndHorizontal();
            }
        }
        return sp;
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public void DrawProperty(string label, SerializedProperty sp, params GUILayoutOption[] options)
    {
        DrawProperty(label, sp, true, options);
    }

    /// <summary>
    /// Helper function that draws a serialized property.
    /// </summary>

    static public void DrawProperty(string label, SerializedProperty sp, bool padding, params GUILayoutOption[] options)
    {
        if (sp != null)
        {
            if (padding) EditorGUILayout.BeginHorizontal();

            if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
            else EditorGUILayout.PropertyField(sp, options);

            if (padding)
            {
                GUILayout.Space(18f);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// Draws the tiled texture. Like GUI.DrawTexture() but tiled instead of stretched.
    /// </summary>

    static public void DrawTiledTexture(Rect rect, Texture tex)
    {
        GUI.BeginGroup(rect);
        {
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            for (int y = 0; y < height; y += tex.height)
            {
                for (int x = 0; x < width; x += tex.width)
                {
                    GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                }
            }
        }
        GUI.EndGroup();
    }

    /// <summary>
    /// Draw a single-pixel outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = contrastTexture;
            GUI.color = Color.white;
            DrawTiledTexture(new Rect(rect.xMin, rect.yMax, 1f, -rect.height), tex);
            DrawTiledTexture(new Rect(rect.xMax, rect.yMax, 1f, -rect.height), tex);
            DrawTiledTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            DrawTiledTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
        }
    }

    /// <summary>
    /// Draw a single-pixel outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = blankTexture;
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// Draw a selection outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect relative, Color color)
    {
        if (Event.current.type == EventType.Repaint)
        {
            // Calculate where the outer rectangle would be
            float x = rect.xMin + rect.width * relative.xMin;
            float y = rect.yMax - rect.height * relative.yMin;
            float width = rect.width * relative.width;
            float height = -rect.height * relative.height;
            relative = new Rect(x, y, width, height);

            // Draw the selection
            DrawOutline(relative, color);
        }
    }

    /// <summary>
    /// Draw a selection outline around the specified rectangle.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect relative)
    {
        if (Event.current.type == EventType.Repaint)
        {
            // Calculate where the outer rectangle would be
            float x = rect.xMin + rect.width * relative.xMin;
            float y = rect.yMax - rect.height * relative.yMin;
            float width = rect.width * relative.width;
            float height = -rect.height * relative.height;
            relative = new Rect(x, y, width, height);

            // Draw the selection
            DrawOutline(relative);
        }
    }

    /// <summary>
    /// Draw a 9-sliced outline.
    /// </summary>

    static public void DrawOutline(Rect rect, Rect outer, Rect inner)
    {
        if (Event.current.type == EventType.Repaint)
        {
            Color green = new Color(0.4f, 1f, 0f, 1f);

            DrawOutline(rect, new Rect(outer.x, inner.y, outer.width, inner.height));
            DrawOutline(rect, new Rect(inner.x, outer.y, inner.width, outer.height));
            DrawOutline(rect, outer, green);
        }
    }

    /// <summary>
    /// Draw a checkered background for the specified texture.
    /// </summary>

    static public Rect DrawBackground(Texture2D tex, float ratio)
    {
        Rect rect = GUILayoutUtility.GetRect(0f, 0f);
        rect.width = Screen.width - rect.xMin;
        rect.height = rect.width * ratio;
        GUILayout.Space(rect.height);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D blank = blankTexture;
            Texture2D check = backdropTexture;

            // Lines above and below the texture rectangle
            GUI.color = new Color(0f, 0f, 0f, 0.2f);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin - 1, rect.width, 1f), blank);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), blank);
            GUI.color = Color.white;

            // Checker background
            DrawTiledTexture(rect, check);
        }
        return rect;
    }

    /// <summary>
    /// Helper function that returns the selected root object.
    /// </summary>

    static public GameObject SelectedRoot() { return SelectedRoot(false); }

    /// <summary>
    /// Helper function that returns the selected root object.
    /// </summary>

    static public GameObject SelectedRoot(bool createIfMissing)
    {
        GameObject go = Selection.activeGameObject;

        // Only use active objects
        if (go != null && !NJGTools.GetActive(go)) go = null;

        return go;
    }
}
