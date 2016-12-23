using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NJG
{

    static class MapCreator
    {
        /*public struct Resources
        {
            public Sprite standard;
            public Sprite background;
            public Sprite inputField;
            public Sprite knob;
            public Sprite checkmark;
            public Sprite dropdown;
            public Sprite mask;
        }*/

        //private const float kWidth = 160f;
        //private const float kThickHeight = 30f;
        //private const float kThinHeight = 20f;

        //private static Vector2 s_ThickElementSize = new Vector2(kWidth, kThickHeight);
        //private static Vector2 s_ThinElementSize = new Vector2(kWidth, kThinHeight);
        //private static Vector2 s_ImageElementSize = new Vector2(100f, 100f);
        //private static Color s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        //private static Color s_PanelColor = new Color(1f, 1f, 1f, 0.392f);
        //private static Color s_TextColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        [MenuItem("GameObject/UI/Minimap", false, 2000)]
        static void Create()
        {
            GameObject go = CreateUIElementRoot("Minimap", new Vector2(250, 250));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.one;
            rt.anchorMax = Vector2.one;
            rt.pivot = Vector2.one;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - 10, rt.anchoredPosition.y - 10);

            Map map = go.AddComponent<Map>();
            map.material = NJGEditorTools.GetMaterial(map, true);

            Image img = map.iconRoot.gameObject.GetComponent<Image>();
            img.sprite = NJGEditorTools.GetDefaultMask(map == Map.miniMap);

            GameObject bg = new GameObject("Background");
            bg.AddComponent<Image>().color = new Color(80f / 255f, 80f / 255f, 80f / 255f, 1f);
            bg.transform.SetParent(go.transform, false);
            bg.transform.SetAsFirstSibling();
            rt = bg.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            Canvas canvas = Selection.activeGameObject ? Selection.activeGameObject.GetComponent<Canvas>() : null;

            if (!canvas) canvas = Object.FindObjectOfType<Canvas>();

            if (!canvas)
            {
                canvas = new GameObject("Canvas", typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler)).GetComponent<Canvas>();
                canvas.gameObject.layer = LayerMask.NameToLayer("UI");
                canvas.renderMode = RenderMode.ScreenSpaceCamera;

                Undo.RegisterCreatedObjectUndo(canvas, "Create " + canvas.name);
            }

            if(canvas.worldCamera == null)
            {
                Camera cam = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
                cam.cullingMask = 1 << LayerMask.NameToLayer("UI");
                cam.clearFlags = CameraClearFlags.Depth;
                cam.orthographic = true;
                canvas.worldCamera = cam;
            }

            if (canvas) go.transform.SetParent(canvas.transform, false);

            EventSystem esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, null);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                //eventSystem.AddComponent<TouchInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            Button zoomIn = CreateButton("Button - ZoomIn", "+", new Vector2(30, 30));
            zoomIn.gameObject.AddComponent<ButtonZoom>().zoomIn = true;
            zoomIn.gameObject.GetComponent<ButtonZoom>().map = map;
            zoomIn.transform.SetParent(go.transform, false);

            rt = zoomIn.gameObject.GetComponent<RectTransform>();
            rt.pivot = Vector2.one;
            rt.anchorMin = Vector2.one;
            rt.anchorMax = Vector2.one;

            Button zoomOut = CreateButton("Button - ZoomOut", "-", new Vector2(30, 30));
            zoomOut.gameObject.AddComponent<ButtonZoom>().zoomIn = false;
            zoomOut.gameObject.GetComponent<ButtonZoom>().map = map;
            zoomOut.transform.SetParent(go.transform, false);

            rt = zoomOut.gameObject.GetComponent<RectTransform>();
            rt.pivot = Vector2.one;
            rt.anchorMin = Vector2.one;
            rt.anchorMax = Vector2.one;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - 32);

            Button lockMap = CreateButton("Button - Lock", "L", new Vector2(30, 30));
            lockMap.gameObject.AddComponent<ButtonLockMap>().map = map;
            lockMap.transform.SetParent(go.transform, false);

            rt = lockMap.gameObject.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);

            Text text = new GameObject("WorldName", typeof(Text), typeof(WorldName)).GetComponent<Text>();
            text.transform.SetParent(go.transform, false);
            SetDefaultTextValues(text);
            text.text = "World Name";
            text.gameObject.AddComponent<Outline>();
            text.fontSize = 18;
            text.alignment = TextAnchor.MiddleCenter;

            rt = text.gameObject.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);            
            rt.sizeDelta = new Vector2(-80f, 60f);

            text = new GameObject("MapCoords", typeof(Text), typeof(MapCoords)).GetComponent<Text>();
            text.transform.SetParent(go.transform, false);
            SetDefaultTextValues(text);
            text.text = "x:0 y:0";
            text.gameObject.AddComponent<Outline>();
            text.fontSize = 14;
            text.alignment = TextAnchor.LowerCenter;

            rt = text.gameObject.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchorMin = new Vector2(0.5f, 0);
            rt.anchorMax = new Vector2(0.5f, 0);
            rt.sizeDelta = new Vector2(100f, 20f);
            rt.anchoredPosition = new Vector2(0, 25);

            SetLayerRecursively(go, LayerMask.NameToLayer("UI"));

            Undo.RegisterCreatedObjectUndo(go, "Create Minimap");

            Selection.activeGameObject = go;
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            child.transform.SetParent(parent.transform, false);
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        private static GameObject CreateUIElementRoot(string name, Vector2 size)
        {
            GameObject child = new GameObject(name);
            RectTransform rectTransform = child.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }


        static GameObject CreateUIObject(string name, GameObject parent, params System.Type[] types)
        {
            GameObject go = new GameObject(name, types);
            go.AddComponent<RectTransform>();
            SetParentAndAlign(go, parent);
            return go;
        }

        public static Button CreateButton(string name, string textContent, Vector2 size)
        {
            GameObject buttonRoot = CreateUIElementRoot(name, size);

            GameObject childText = new GameObject("Text");
            SetParentAndAlign(childText, buttonRoot);

            Image image = buttonRoot.AddComponent<Image>();
            image.sprite = new DefaultControls.Resources().standard;
            image.type = Image.Type.Sliced;
            image.color = new Color(0f / 255f, 132f / 255f, 176f / 255f, 1f); //s_DefaultSelectableColor;

            Button bt = buttonRoot.AddComponent<Button>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.AddComponent<Text>();
            text.text = textContent;
            text.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return bt;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = Color.white;
            lbl.fontStyle = FontStyle.Bold;
            lbl.fontSize = 24;
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor = new Color(0.521f, 0.521f, 0.521f);
        }
    }
}
 