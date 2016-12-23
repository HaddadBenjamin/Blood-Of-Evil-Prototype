using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
class MyHierarchyIcons
{
    class EditorHierarchyIconInfo
    {
        public System.Type type;
        public string path;
        Texture2D m_icon;
        bool notFound = false;
        public Texture2D icon
        {
            get
            {
                if (m_icon == null && !notFound)
                {
                    m_icon = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                    if (m_icon != null)
                    {
                        try
                        {

                            m_icon.width = 12;
                            m_icon.height = 12;
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                    else
                        notFound = true;
                }
                return m_icon;
            }
        }

        public EditorHierarchyIconInfo(System.Type _type, string _path)
        {
            type = _type;
            path = _path;
            m_icon = null;
        }
    }
    static List<EditorHierarchyIconInfo> iconInfoList;

    static MyHierarchyIcons()
    {
        // Init
        string scriptGuid = AssetDatabase.FindAssets(typeof(MyHierarchyIcons).Name + " t:script").FirstOrDefault();

        if (scriptGuid != null)
        {
            string relativePath = System.IO.Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(scriptGuid));
            iconInfoList = new List<EditorHierarchyIconInfo>()
            {
                new EditorHierarchyIconInfo(typeof(Camera), relativePath + "/Icons/iconCamera.png"),
                new EditorHierarchyIconInfo(typeof(UnityEngine.UI.Text), relativePath + "/Icons/TextIcon.png"),
                new EditorHierarchyIconInfo(typeof(UnityEngine.UI.Button), relativePath + "/Icons/ButtonIcon.png"),
                new EditorHierarchyIconInfo(typeof(UnityEngine.UI.Image), relativePath + "/Icons/ImageIcon.png"),
                new EditorHierarchyIconInfo(typeof(AudioSource), relativePath + "/Icons/AudioSourceIcon.png"),
				//new EditorHierarchyIconInfo(typeof(Dummy), relativePath + "/Icons/iconDummy.png"),
				//new EditorHierarchyIconInfo(typeof(Replica.IModule), relativePath + "/Icons/iconModule.png"),
            };
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
        }
    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        // place the icon to the right of the list:

        GameObject go = EditorUtility.InstanceIDToObject (instanceID) as GameObject;

        if (go != null)
        {
            Rect r = new Rect (selectionRect);
            float width, height;
            GUI.skin.label.CalcMinMaxWidth(new GUIContent(go.name), out width, out height);
            r.x += width;
            r.y += 2;
            r.width = 16;
            r.height = 16;
            iconInfoList.ForEach(info =>
            {
                if (info.icon != null && go.GetComponent(info.type))
                {
                    GUI.Label(r, info.icon);
                    r.x += r.width + 2;
                }
            });
        }
    }
}