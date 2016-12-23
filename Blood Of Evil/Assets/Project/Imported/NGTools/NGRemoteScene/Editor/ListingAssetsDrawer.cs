using NGTools;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[CustomPropertyDrawer(typeof(ListingAssets))]
	public class ListingAssetsDrawer : PropertyDrawer
	{
		private class Folder
		{
			public class File
			{
				public readonly string	name;

				public bool		referenced;

				public	File(string name)
				{
					this.name = name;
				}
			}

			public readonly Folder			parent;
			public readonly string			name;
			public readonly List<Folder>	folders;
			public readonly File[]			files;

			public bool	open;
			public bool	referenced;

			public	Folder(Folder parent, string path, string name, HashSet<string> existingList)
			{
				string[]	dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
				string[]	files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

				this.parent = parent;
				this.name = name;
				this.folders = new List<Folder>();

				int	totalFiles = 0;

				for (int j = 0; j < files.Length; j++)
				{
					if (files[j].EndsWith(".meta") == false)
						++totalFiles;
				}

				this.files = new File[totalFiles];

				for (int i = 0; i < dirs.Length; i++)
				{
					Folder	folder = new Folder(this, dirs[i], dirs[i].Substring(path.Length + 1), existingList);

					if (folder.folders.Count > 0 || folder.files.Length > 0)
						this.folders.Add(folder);
				}

				for (int j = 0, i = 0; j < files.Length; j++)
				{
					if (files[j].EndsWith(".meta") == false)
					{
						this.files[i] = new File(files[j]);

						if (existingList.Contains(this.files[i].name) == true)
							this.files[i].referenced = true;

						++i;
					}
				}

				this.Update(false);
			}

			public float	GetHeight()
			{
				float	height = EditorGUIUtility.singleLineHeight;

				if (this.open == true)
				{
					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Length == 0)
							continue;

						height += this.folders[i].GetHeight();
					}

					height += this.files.Length * EditorGUIUtility.singleLineHeight;
				}

				return height;
			}

			public void	Draw(Rect r)
			{
				float x = r.x;
				float w = r.width;

				r.width = 16F;
				EditorGUI.showMixedValue = this.HasMixedRefs();
				EditorGUI.BeginChangeCheck();
				EditorGUI.ToggleLeft(r, string.Empty, this.referenced);
				if (EditorGUI.EndChangeCheck() == true)
				{
					if (this.referenced == true)
						this.Unreference();
					else
						this.Reference();
				}

				EditorGUI.showMixedValue = false;

				r.x += r.width + 10F;
				r.width = w;
				this.open = EditorGUI.Foldout(r, this.open, this.name);

				if (this.open == true)
				{
					r.x = x + 16F;
					r.y += r.height;

					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Length == 0)
							continue;

						this.folders[i].Draw(r);
						r.y += this.folders[i].GetHeight();
					}

					for (int i = 0; i < this.files.Length; i++)
					{
						EditorGUI.BeginChangeCheck();
						this.files[i].referenced = EditorGUI.ToggleLeft(r, this.files[i].name, this.files[i].referenced);
						if (EditorGUI.EndChangeCheck() == true)
						{
							this.Update();
						}

						r.y += r.height;
					}
				}
			}

			public bool	HasMixedRefs()
			{
				bool	referencing = false;
				int		j = 0;

				for (int i = 0; i < this.folders.Count; i++)
				{
					if (this.folders[i].folders.Count == 0 && this.folders[i].files.Length == 0)
						continue;

					if (this.folders[i].referenced == true)
					{
						if (j > 0 && referencing == false)
							return true;

						referencing = true;
					}
					else if (referencing == true)
						return true;

					if (this.folders[i].HasMixedRefs() == true)
						return true;

					 ++j;
				}

				for (int i = 0; i < this.files.Length; i++)
				{
					if (this.files[i].referenced == true)
					{
						if ((j > 0 || i > 0) && referencing == false)
							return true;

						referencing = true;
					}
					else if (referencing == true)
						return true;
				}

				return false;
			}

			public void	Reference()
			{
				this.referenced = true;

				for (int i = 0; i < this.folders.Count; i++)
					this.folders[i].Reference();

				for (int i = 0; i < this.files.Length; i++)
					this.files[i].referenced = true;

				if (this.parent != null)
					this.parent.Update();
			}

			public void	Unreference()
			{
				this.referenced = false;

				for (int i = 0; i < this.folders.Count; i++)
					this.folders[i].Unreference();

				for (int i = 0; i < this.files.Length; i++)
					this.files[i].referenced = false;

				if (this.parent != null)
					this.parent.Update();
			}

			public IEnumerable<string>	EachReference()
			{
				for (int i = 0; i < this.folders.Count; i++)
				{
					IEnumerable<string>	subRefs = this.folders[i].EachReference();

					foreach (var reference in subRefs)
						yield return reference;
				}

				for (int i = 0; i < this.files.Length; i++)
				{
					if (this.files[i].referenced == true)
						yield return this.files[i].name;
				}
			}

			private void	Update(bool ascendentUpdate = true)
			{
				if (this.HasMixedRefs() == false)
				{
					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Length == 0)
							continue;
						this.referenced = this.folders[0].referenced;
						goto skipFile;
					}

					if (this.files.Length > 0)
						this.referenced = this.files[0].referenced;
				}

				skipFile:
				if (ascendentUpdate == true && this.parent != null)
					this.parent.Update();
			}
		}

		private Folder				root;
		private HashSet<string>		existingList;
		private SerializedProperty	assets;

		public override float	GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (this.existingList == null)
				this.Init(property);

			return this.root.GetHeight() + EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight;
		}

		public override void	OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (this.existingList == null)
				this.Init(property);

			float	height = position.height;

			position.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.LabelField(position, "NG Project Resources (" + this.assets.arraySize + ")");

			position.y += position.height;
			this.root.Draw(position);

			position.y += height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.singleLineHeight;
			if (GUI.Button(position, "Reference Resources") == true)
			{
				assets.arraySize = 0;

				foreach (var path in this.root.EachReference())
				{
					if (path.EndsWith(".unity") == true)
						continue;

					Object[]	references = AssetDatabase.LoadAllAssetsAtPath(path);

					if (references.Length == 0)
						references = new Object[] { AssetDatabase.LoadMainAssetAtPath(path) };

					this.assets.InsertArrayElementAtIndex(this.assets.arraySize);
					SerializedProperty prop = this.assets.GetArrayElementAtIndex(this.assets.arraySize - 1);
					prop.FindPropertyRelative("asset").stringValue = path;

					SerializedProperty	refField = prop.FindPropertyRelative("references");
					refField.arraySize = references.Length;

					for (int i = 0; i < references.Length; i++)
						refField.GetArrayElementAtIndex(i).objectReferenceValue = references[i];
				}
			}
		}

		private void	Init(SerializedProperty property)
		{
			this.existingList = new HashSet<string>();
			this.assets = property.FindPropertyRelative("assets");

			for (int i = 0; i < this.assets.arraySize; i++)
				this.existingList.Add(this.assets.GetArrayElementAtIndex(i).FindPropertyRelative("asset").stringValue);

			this.root = new Folder(null, "Assets", "Assets", this.existingList);
		}
	}
}