using NGTools;
using NGTools.NGRemoteScene;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[CustomPropertyDrawer(typeof(ListingAssets))]
	internal sealed class ListingAssetsDrawer : PropertyDrawer
	{
		private class Folder
		{
			public class File
			{
				public readonly string	path;
				public readonly string	name;

				public bool		referenced;

				public	File(string path, string name)
				{
					this.path = path;
					this.name = name;
				}
			}

			public readonly Folder			parent;
			public readonly string			name;
			public readonly List<Folder>	folders = new List<Folder>();
			public readonly List<File>		files = new List<File>();

			public bool	open;
			public bool	referenced;

			public	Folder(Folder parent, string name)
			{
				this.parent = parent;
				this.name = name;
				this.open = EditorPrefs.GetBool(this.GetHierarchyPath());
			}

			public float	GetHeight()
			{
				float	height = EditorGUIUtility.singleLineHeight;

				if (this.open == true)
				{
					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Count == 0)
							continue;

						height += this.folders[i].GetHeight();
					}

					height += this.files.Count * EditorGUIUtility.singleLineHeight;
				}

				return height;
			}

			public void	Draw(Rect r)
			{
				float	x = r.x;
				float	w = r.width;

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
				EditorGUI.BeginChangeCheck();
				this.open = EditorGUI.Foldout(r, this.open, this.name);
				if (EditorGUI.EndChangeCheck() == true)
					EditorPrefs.SetBool(this.GetHierarchyPath(), this.open);

				if (this.open == true)
				{
					r.x = x + 16F;
					r.y += r.height;

					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Count == 0)
							continue;

						this.folders[i].Draw(r);
						r.y += this.folders[i].GetHeight();
					}

					for (int i = 0; i < this.files.Count; i++)
					{
						EditorGUI.BeginChangeCheck();
						this.files[i].referenced = EditorGUI.ToggleLeft(r, this.files[i].name, this.files[i].referenced);
						if (EditorGUI.EndChangeCheck() == true)
							this.Update();

						r.y += r.height;
					}
				}
			}

			private string	GetHierarchyPath()
			{
				StringBuilder	buffer = Utility.GetBuffer();

				Folder	f = this;

				while (f != null)
				{
					buffer.Insert(0, f.name);
					buffer.Insert(0, '/');

					f = f.parent;
				}

				return Utility.ReturnBuffer(buffer);
			}

			public bool	HasMixedRefs()
			{
				bool	referencing = false;
				int		j = 0;

				for (int i = 0; i < this.folders.Count; i++)
				{
					if (this.folders[i].folders.Count == 0 && this.folders[i].files.Count == 0)
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

				for (int i = 0; i < this.files.Count; i++)
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

				for (int i = 0; i < this.files.Count; i++)
					this.files[i].referenced = true;

				if (this.parent != null)
					this.parent.Update();
			}

			public void	Unreference()
			{
				this.referenced = false;

				for (int i = 0; i < this.folders.Count; i++)
					this.folders[i].Unreference();

				for (int i = 0; i < this.files.Count; i++)
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

				for (int i = 0; i < this.files.Count; i++)
				{
					if (this.files[i].referenced == true)
						yield return this.files[i].path;
				}
			}

			public void	Update(bool ascendentUpdate = true)
			{
				if (this.HasMixedRefs() == false)
				{
					for (int i = 0; i < this.folders.Count; i++)
					{
						if (this.folders[i].folders.Count == 0 && this.folders[i].files.Count == 0)
							continue;
						this.referenced = this.folders[0].referenced;
						goto skipFile;
					}

					if (this.files.Count > 0)
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
			EditorGUI.LabelField(position, "NG Remote Project Assets (" + this.assets.arraySize + ")");

			position.y += position.height;
			this.root.Draw(position);

			using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
			{
				position.y += height - EditorGUIUtility.singleLineHeight - EditorGUIUtility.singleLineHeight;
				if (GUI.Button(position, "Reference Resources") == true)
				{
					assets.arraySize = 0;

					foreach (var path in this.root.EachReference())
					{
						if (path.EndsWith(".unity") == true)
							continue;

						Object[]	references = AssetDatabase.LoadAllAssetsAtPath("Assets/" + path);

						if (references.Length == 0)
						{
							Object	mainAsset = AssetDatabase.LoadMainAssetAtPath(path);
							if (mainAsset != null)
								references = new Object[] { mainAsset };
							else
							{
								InternalNGDebug.LogWarning("Assets at \"" + path + "\" can not be embedded.");
								continue;
							}
						}

						this.assets.InsertArrayElementAtIndex(this.assets.arraySize);
						SerializedProperty	prop = this.assets.GetArrayElementAtIndex(this.assets.arraySize - 1);
						prop.FindPropertyRelative("asset").stringValue = path;

						SerializedProperty	refField = prop.FindPropertyRelative("references");
						refField.arraySize = references.Length;

						for (int i = 0; i < references.Length; i++)
							refField.GetArrayElementAtIndex(i).objectReferenceValue = references[i];
					}
				}
			}
		}

		private void	Init(SerializedProperty property)
		{
			this.existingList = new HashSet<string>();
			this.assets = property.FindPropertyRelative("assets");

			for (int i = 0; i < this.assets.arraySize; i++)
				this.existingList.Add(this.assets.GetArrayElementAtIndex(i).FindPropertyRelative("asset").stringValue);

			string[]	assets = AssetDatabase.GetAllAssetPaths();

			this.root = new Folder(null, "Assets");

			for (int i = 0; i < assets.Length; i++)
			{
				if (assets[i].StartsWith("Assets/") == true &&
					File.Exists(assets[i]) == true)
				{
					this.Generate(assets[i]);
				}
			}
		}

		private void	Generate(string path)
		{
			string[]	paths = path.Split('/');
			Folder		folder = this.root;

			for (int i = 1; i < paths.Length - 1; i++)
			{
				int	j = 0;
				int	max = folder.folders.Count;

				for (; j < max; j++)
				{
					if (folder.folders[j].name == paths[i])
					{
						folder = folder.folders[j];
						break;
					}
				}

				if (j >= max)
				{
					folder.folders.Add(new Folder(folder, paths[i]));
					folder = folder.folders[folder.folders.Count - 1];
				}
			}

			Folder.File	f = new Folder.File(path, paths[paths.Length - 1]);
			if (existingList.Contains(f.path) == true)
				f.referenced = true;
			folder.files.Add(f);

			folder.Update(true);
		}
	}
}