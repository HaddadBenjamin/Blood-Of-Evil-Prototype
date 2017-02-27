using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGHub
{
	[Serializable, Category("Misc")]
	internal sealed class MenuCallerComponent : HubComponent, ISerializable
	{
		[Exportable]
		public string	menuItem;
		[Exportable]
		public Texture	image;
		[Exportable]
		public string	alias;

		private GUIContent	content = new GUIContent();

		private	MenuCallerComponent(SerializationInfo info, StreamingContext context) : this()
		{
			this.menuItem = info.GetString("menuItem");
			this.image = AssetDatabase.LoadAssetAtPath(info.GetString("image"), typeof(Texture)) as Texture;
			this.alias = info.GetString("alias");
		}

		public	MenuCallerComponent() : base("Call MenuItem", true)
		{
		}

		public override void	OnPreviewGUI(Rect r)
		{
			GUI.Label(r, "Call MenuItem \"" + this.menuItem + "\"");
		}

		public override void	OnEditionGUI()
		{
			if (GUILayout.Button("Pick") == true)
			{
				GenericMenu	menu = new GenericMenu();
				string[]	menuItems = Utility.GetAllMenuItems();

				for (int i = 0; i < menuItems.Length; i++)
					menu.AddItem(new GUIContent(menuItems[i]), false, this.PickMenuItem, menuItems[i]);

				menu.ShowAsContext();
			}

			this.menuItem = EditorGUILayout.TextField("Menu Item Path", this.menuItem);
			this.image = EditorGUILayout.ObjectField("Image", this.image, typeof(Texture), false) as Texture;
			this.alias = EditorGUILayout.TextField("Alias", this.alias);
		}

		public override void	OnGUI()
		{
			this.content.text = (string.IsNullOrEmpty(this.alias) == true ? this.menuItem : this.alias);
			this.content.image = this.image;
			if (GUILayout.Button(this.content, GUILayout.Height(this.hub.height)) == true)
				EditorApplication.ExecuteMenuItem(this.menuItem);
		}

		private void	PickMenuItem(object data)
		{
			string	path = (string)data;

			this.menuItem = path;
			this.alias = path.Substring(path.LastIndexOf("/") + 1);
		}

		void	ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("menuItem", this.menuItem);
			info.AddValue("image", AssetDatabase.GetAssetPath(this.image));
			info.AddValue("alias", this.alias);
		}
	}
}