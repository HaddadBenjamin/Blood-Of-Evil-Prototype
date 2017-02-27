using NGTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGSyncFolders
{
	public sealed class File
	{
		private static Stack<List<File>>	cachedModels = new Stack<List<File>>();

		public bool	CanDisplay { get { return this.canDisplay; } }

		public readonly File	parent;
		public readonly string	path;
		public readonly string	name;
		public readonly bool	isDir;
		public InitialStates	initialState;
		public MasterStates		masterState;
		public SlaveStates		slaveState;
		public string			initialHash;
		public string			targetHash;
		public bool				isMaster;
		public List<File>		children = new List<File>();

		public bool	open = true;

		private bool		isHeightInvalidated = true;
		private float		height = 0F;
		private bool		canDisplay = false;
		private File		model;
		private Exception	actionException;

		public	File(File parent, string path, string name, bool isMaster, bool isDir, InitialStates initialState, MasterStates masterState)
		{
			this.parent = parent;
			this.path = path;
			this.name = Path.GetFileName(path);
			this.isDir = isDir;
			this.initialState = initialState;
			this.masterState = masterState;
			this.isMaster = isMaster;
			this.initialHash = isDir == false ? NGSyncFoldersWindow.TryGetCachedHash(this.path) : string.Empty;
		}

		public	File(File parent, string path, string name, bool isMaster, bool isDir, InitialStates state, SlaveStates slaveState)
		{
			this.parent = parent;
			this.path = path;
			this.name = name;
			this.isDir = isDir;
			this.initialState = state;
			this.slaveState = slaveState;
			this.isMaster = isMaster;
			this.initialHash = isDir == false ? NGSyncFoldersWindow.TryGetCachedHash(this.path) : string.Empty;
		}

		public float	GetHeight(File model)
		{
			if (this.isDir == true)
			{
				if (this.isHeightInvalidated == false)
					return this.height;

				this.isHeightInvalidated = false;

				this.CheckDisplay(model);

				if (this.canDisplay == false)
					return this.height = 0F;

				if (this.open == false)
					return this.height = 16F;

				this.height = 16F;

				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i].isDir == true)
						this.height += this.children[i].GetHeight(this.children[i].GetCacheModel(model, this.children[i].name));
					else
					{
						if (this.isMaster == true)
						{
							if (this.children[i].masterState != MasterStates.Same)
								this.height += 16F;
						}
						else
						{
							Action	action = this.children[i].ChooseAction(this.children[i].GetCacheModel(model, this.children[i].name));

							if (action != Action.Nothing)
								this.height += 16F;
						}
					}
				}

				return this.height;
			}

			return 0F;
		}

		public void	OnGUI(Rect r, float minY, float maxY, File model)
		{
			if (this.isDir == false)
				return;

			if (this.canDisplay == false)
			{
				GUI.Label(r, "No change detected.");
				return;
			}

			r.height = 16F;
			EditorGUI.BeginChangeCheck();
			this.open = EditorGUI.Foldout(r, this.open, this.name);
			if (EditorGUI.EndChangeCheck() == true)
				this.InvalidateHeight();

			if (this.open == false)
				return;

			r.y += r.height;

			if (r.y > maxY)
				return;

			float	w = r.width;

			EditorGUI.indentLevel++;
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].isDir == true)
				{
					if (this.children[i].canDisplay == true)
					{
						File	m = this.children[i].GetCacheModel(model, this.children[i].name);
						r.height = this.children[i].GetHeight(m);

						if (r.yMax >= minY)
							this.children[i].OnGUI(r, minY, maxY, m);
						r.y += r.height;

						if (r.y > maxY)
							break;
					}
				}
				else
				{
					if (this.isMaster == true)
					{
						if (this.children[i].masterState != MasterStates.Same)
						{
							r.height = 16F;

							if (r.yMax >= minY)
							{
								if (this.children[i].masterState == MasterStates.Created)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.CreateColor), GeneralStyles.RichLabel);
								else if (this.children[i].masterState == MasterStates.Altered)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.RestoreColor), GeneralStyles.RichLabel);
								else if (this.children[i].masterState == MasterStates.Deleted)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.DeleteColor), GeneralStyles.RichLabel);
							}

							r.y += r.height;

							if (r.y > maxY)
								break;
						}
					}
					else
					{
						File	m = this.children[i].GetCacheModel(model, this.children[i].name);
						Action	action = this.children[i].ChooseAction(m);

						if (action != Action.Nothing)
						{
							r.height = 16F;

							if (r.yMax >= minY)
							{
								r.width -= 75F;
								if (this.children[i].actionException != null)
									r.width -= 75F;

								if (action == Action.Create)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.CreateColor), GeneralStyles.RichLabel);
								else if (action == Action.Change)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.RestoreColor), GeneralStyles.RichLabel);
								else if (action == Action.Delete)
									EditorGUI.LabelField(r, Utility.Color(this.children[i].name, NGSyncFoldersWindow.DeleteColor), GeneralStyles.RichLabel);
								r.x += r.width;

								if (this.children[i].actionException != null)
								{
									r.width = 75F;
									using (BgColorContentRestorer.Get(Color.red))
									{
										if (GUI.Button(r, "Error") == true)
										{
											ErrorPopup	e = new ErrorPopup();
											e.exception = this.children[i].actionException;
											e.OpenError(r);
										}
									}
									r.x += r.width;
								}

								using (BgColorContentRestorer.Get(action == Action.Create ? NGSyncFoldersWindow.CreateColor : (action == Action.Change ? NGSyncFoldersWindow.RestoreColor : NGSyncFoldersWindow.DeleteColor)))
								{
									r.width = 75F;
									if (GUI.Button(r, action.ToString()) == true)
										this.children[i].Sync(m);
									r.x -= w - 75F;
									if (this.actionException != null)
										r.x -= 75F;
									r.width = w;
								}
							}

							r.y += r.height;

							if (r.y > maxY)
								break;
						}
					}
				}
			}
			EditorGUI.indentLevel--;
		}

		public void	ResetState()
		{
			this.masterState = MasterStates.Same;

			for (int i = 0; i < this.children.Count; i++)
				this.children[i].ResetState();
		}

		public void	SyncAll(File model)
		{
			File[]	children = this.children.ToArray();

			for (int i = 0; i < children.Length; i++)
			{
				File	match = null;

				if (model != null)
				{
					for (int j = 0; j < model.children.Count; j++)
					{
						if (model.children[j].name == children[i].name)
						{
							match = model.children[j];
							break;
						}
					}
				}

				if (children[i].isDir == false)
					children[i].Sync(match);
				else
					children[i].SyncAll(match);
			}
		}

		public File	Generate(string path)
		{
			this.InvalidateHeight();

			if (this.path == path)
			{
				string	hash = NGSyncFoldersWindow.TryGetCachedHash(path);

				if (this.isMaster == true)
				{
					if (hash == this.initialHash)
						this.masterState = MasterStates.Same;
					else
						this.masterState = MasterStates.Altered;
				}
				else
				{
					this.initialHash = hash;
					this.slaveState = SlaveStates.Exist;
				}

				this.targetHash = hash;

				return this;
			}

			string		oldPath = path.Substring(0, this.path.Length + 1);
			string[]	newPath = path.Substring(this.path.Length + 1).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			File		parent = this;

			path = this.path;

			for (int i = 0; i < newPath.Length; i++)
			{
				oldPath = Path.Combine(oldPath, newPath[i]);
				path = Path.Combine(path, newPath[i]);

				File	child = null;

				for (int j = 0; j < parent.children.Count; j++)
				{
					if (parent.children[j].path == path)
					{
						child = parent.children[j];
						break;
					}
				}

				if (child == null)
				{
					if (Conf.DebugMode == Conf.DebugModes.Verbose)
						Debug.Log("Generating " + path + " in " + parent + "(" + oldPath + ")");
					if (this.isMaster == true)
						child = new File(parent, path, Path.GetFileName(path), true, Directory.Exists(oldPath), InitialStates.New, MasterStates.Created);
					else
						child = new File(parent, path, Path.GetFileName(path), false, Directory.Exists(oldPath), InitialStates.New, System.IO.File.Exists(path) ? SlaveStates.Exist : SlaveStates.NonExist);
					parent.children.Add(child);
					parent.children.Sort((a, b) => string.CompareOrdinal(a.path, b.path));
				}

				parent = child;
			}

			return parent;
		}

		public void	ScanDiff(File model, string path, List<string> inclusiveFilters, List<string> exclusiveFilters)
		{
			string[]	dirs = null;
			string[]	files = null;

			if (Directory.Exists(path) == true)
			{
				dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
				files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
			}

			List<File>	currentModels;

			if (cachedModels.Count > 0)
			{
				currentModels = cachedModels.Pop();
				currentModels.Clear();
			}
			else
				currentModels = new List<File>();

			if (model != null)
				currentModels.AddRange(model.children);

			if (dirs != null)
			{
				for (int i = 0; i < dirs.Length; i++)
				{
					if ((inclusiveFilters.Count > 0 && this.Filter(dirs[i], inclusiveFilters) == false) || this.Filter(dirs[i], exclusiveFilters) == true)
						continue;

					File	match = null;
					string	name = Path.GetFileName(dirs[i]);

					if (model != null)
					{
						int	n = this.Find(name, currentModels);

						if (n != -1)
						{
							match = currentModels[n];
							currentModels.RemoveAt(n);
						}
					}

					if (Conf.DebugMode == Conf.DebugModes.Verbose)
						Debug.Log("D " + dirs[i] + ", with " + match);

					File	f = new File(this, dirs[i], name, false, true, InitialStates.Origin, SlaveStates.Exist);
					f.ScanDiff(match, dirs[i], inclusiveFilters, exclusiveFilters);
					this.children.Add(f);
				}
			}

			if (files != null)
			{
				for (int i = 0; i < files.Length; i++)
				{
					if ((inclusiveFilters.Count > 0 && this.Filter(files[i], inclusiveFilters) == false) || this.Filter(files[i], exclusiveFilters) == true)
						continue;

					File	match = null;
					string	name = Path.GetFileName(files[i]);

					if (model != null)
					{
						int	n = this.Find(name, currentModels);

						if (n != -1)
						{
							match = currentModels[n];
							currentModels.RemoveAt(n);
						}
					}

					if (Conf.DebugMode == Conf.DebugModes.Verbose)
						Debug.Log("F " + files[i] + ", with " + match);

					File	f = new File(this, files[i], name, false, false, match != null ? InitialStates.Origin : InitialStates.New, SlaveStates.Exist);
					f.targetHash = f.initialHash;
					this.children.Add(f);
				}
			}

			for (int i = 0; i < currentModels.Count; i++)
			{
				if (currentModels[i].isDir == true)
				{
					File	f = new File(this, Path.Combine(path, currentModels[i].name), currentModels[i].name, false, true, InitialStates.Origin, SlaveStates.Exist);
					f.ScanDiff(currentModels[i], currentModels[i].path.Replace(model.path, this.path), inclusiveFilters, exclusiveFilters);
					this.children.Add(f);
				}
				else
					this.children.Add(new File(this, Path.Combine(path, currentModels[i].name), currentModels[i].name, false, false, InitialStates.Missing, SlaveStates.NonExist));
			}

			this.children.Sort((a, b) => string.CompareOrdinal(a.path, b.path));

			cachedModels.Push(currentModels);
		}

		public void	ScanFolder(string path, List<string> inclusiveFilters, List<string> exclusiveFilters)
		{
			string[]	dirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
			string[]	files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);

			for (int i = 0; i < dirs.Length; i++)
			{
				if ((inclusiveFilters.Count > 0 && this.Filter(dirs[i], inclusiveFilters) == false) || this.Filter(dirs[i], exclusiveFilters) == true)
					continue;

				if (Conf.DebugMode == Conf.DebugModes.Verbose)
					Debug.Log("D " + dirs[i]);

				File	f = new File(this, dirs[i], Path.GetFileName(dirs[i]), true, true, InitialStates.Origin, MasterStates.Same);
				f.ScanFolder(dirs[i], inclusiveFilters, exclusiveFilters);
				this.children.Add(f);
			}

			for (int i = 0; i < files.Length; i++)
			{
				if ((inclusiveFilters.Count > 0 && this.Filter(files[i], inclusiveFilters) == false) || this.Filter(files[i], exclusiveFilters) == true)
					continue;

				if (Conf.DebugMode == Conf.DebugModes.Verbose)
					Debug.Log("F " + files[i]);

				File	f = new File(this, files[i], Path.GetFileName(files[i]), true, false, InitialStates.Origin, MasterStates.Same);
				this.children.Add(f);
			}

			this.children.Sort((a, b) => string.CompareOrdinal(a.path, b.path));
		}

		public int	Find(string name, List<File> list = null)
		{
			if (list == null)
				list = this.children;

			// Children are alphabetically sorted.
			// Get the first child matching the first letter.
			int	n = this.MatchFirstLetter(name[0], list);

			if (n == -1)
				return -1;

			// It continues if the first letters are matching.
			// If the current occurence is matching less than the previous, it means there is none. (It is sorted, remember)
			int	highestLengthMatch = 1;

			for (; n < list.Count; n++)
			{
				if (list[n].name[0] != name[0])
					break;

				if (name.Length != list[n].name.Length)
					continue;

				int	i = 1;
				for (; i < name.Length; i++)
				{
					if (list[n].name[i] != name[i])
						break;
				}

				if (i >= name.Length)
					return n;

				if (highestLengthMatch <= i)
					highestLengthMatch = i;
				else
					break;
			}

			return -1;
		}

		public File	FindClosest(string path)
		{
			File	result = this;

			for (int i = 0; i < this.children.Count; i++)
			{
				if ((this.children[i].isDir == true &&
					 path.StartsWith(this.children[i].path) == true &&
					 path[this.children[i].path.Length] == '\\') ||
					(this.children[i].isDir == false &&
					 path.StartsWith(this.children[i].path) == true))
				{
					result = this.children[i].FindClosest(path);
					if (result == null)
					{
						result = this.children[i];
						break;
					}
				}
			}

			return result;
		}

		public void	Delete()
		{
			if (Conf.DebugMode == Conf.DebugModes.Verbose)
				Debug.Log("Deleting " + this.path + " " + this.initialState);

			if (this.isMaster == true)
				this.masterState = MasterStates.Deleted;
			else
				this.slaveState = SlaveStates.NonExist;

			if (this.initialState == InitialStates.New)
				this.parent.children.Remove(this);

			this.InvalidateHeight();
		}

		private void	InvalidateHeight()
		{
			this.isHeightInvalidated = true;

			if (this.parent != null)
				this.parent.InvalidateHeight();
		}

		private void	CheckDisplay(File model)
		{
			this.canDisplay = false;

			for (int j = 0; j < this.children.Count && this.canDisplay == false; j++)
			{
				if (this.children[j].isDir == true)
					continue;

				if (this.isMaster == true)
				{
					if (this.children[j].masterState != MasterStates.Same)
						this.canDisplay = true;
				}
				else
				{
					Action	action = this.children[j].ChooseAction(this.children[j].GetCacheModel(model, this.children[j].name));

					if (action != Action.Nothing)
						this.canDisplay = true;
				}
			}

			for (int i = 0; i < this.children.Count && this.canDisplay == false; i++)
			{
				if (this.children[i].isDir == true)
				{
					this.children[i].CheckDisplay(this.children[i].GetCacheModel(model, this.children[i].name));
					this.canDisplay = this.children[i].canDisplay;
				}
			}
		}

		private File	GetCacheModel(File model, string name)
		{
			if (this.model != null)
				return this.model;

			if (model == null)
				return null;

			int	n = this.Find(name, model.children);

			if (n != -1)
				this.model = model.children[n];

			return this.model;
		}

		private int	MatchFirstLetter(char c, List<File> list)
		{
			int	n = 0;
			int	m = list.Count - 1;
			int	i = m >> 1;

			while (n <= m)
			{
				if (list[i].name[0] == c)
				{
					m = i;
					i = n + ((m - n) >> 1);

					// Find the first entry matching the letter.
					while (n < m)
					{
						if (list[i].name[0] != c)
							n = i + 1;
						else
							m = i;

						i = n + ((m - n) >> 1);
					}

					return i;
				}
				else if (list[i].name[0] < c)
					n = i + 1;
				else
					m = i - 1;

				i = n + ((m - n) >> 1);
			}

			return -1;
		}

		private void	Sync(File model, bool force = false)
		{
			Action	action = this.ChooseAction(model);

			if (action != Action.Nothing || force == true)
			{
				try
				{
					this.actionException = null;

					if (action == Action.Delete)
					{
						System.IO.File.Delete(this.path);
						EditorApplication.delayCall += () => this.parent.children.Remove(this);

						if (Directory.GetFiles(Path.GetDirectoryName(this.path)).Length == 0)
							Directory.Delete(Path.GetDirectoryName(this.path));

						if (Conf.DebugMode == Conf.DebugModes.Verbose)
							Debug.Log("Sync " + this.path + "\": Deleted.");
					}
					else if (action == Action.Create)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(this.path));
						if (System.IO.File.Exists(this.path) == false)
							System.IO.File.Copy(model.path, this.path);
						else
							System.IO.File.WriteAllBytes(this.path, System.IO.File.ReadAllBytes(model.path));
						this.initialHash = NGSyncFoldersWindow.TryGetCachedHash(path);
						this.slaveState = SlaveStates.Exist;

						if (Conf.DebugMode == Conf.DebugModes.Verbose)
							Debug.Log("Sync " + this.path + "\": Created.");
					}
					else if (action == Action.Change || force == true)
					{
						Directory.CreateDirectory(Path.GetDirectoryName(this.path));
						System.IO.File.WriteAllBytes(this.path, System.IO.File.ReadAllBytes(model.path));
						this.initialHash = NGSyncFoldersWindow.TryGetCachedHash(path);

						if (Conf.DebugMode == Conf.DebugModes.Verbose)
							Debug.Log("Sync " + this.path + "\": Restored.");
					}

					this.initialState = InitialStates.Origin;

					this.InvalidateHeight();
				}
				catch (Exception ex)
				{
					this.actionException = ex;
					if (model != null)
						InternalNGDebug.LogException("Sync'ing file " + this.path + " with " + model.path + " failed.", ex);
					else
						InternalNGDebug.LogException("Sync'ing file " + this.path + " failed.", ex);
				}
			}
		}

		private string	ProcessHash()
		{
			if (System.IO.File.Exists(this.path) == false)
				return string.Empty;

			using (var md5 = MD5.Create())
			using (var stream = System.IO.File.OpenRead(this.path))
				return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","‌​").ToLower();
		}

		private Action	ChooseAction(File m)
		{
			if (m != null)
			{
				if (m.masterState == MasterStates.Same)
				{
					if (this.slaveState == SlaveStates.NonExist)
						return Action.Create;
					else if (this.initialHash == m.initialHash)
						return Action.Nothing;
					else
						return Action.Change;
				}
				else if (m.masterState == MasterStates.Created)
				{
					if (this.slaveState == SlaveStates.NonExist)
						return Action.Create;
					else if (this.initialHash == m.initialHash)
						return Action.Nothing;
					else
						return Action.Change;
				}
				else if (m.masterState == MasterStates.Altered)
				{
					if (this.slaveState == SlaveStates.NonExist)
						return Action.Create;
					else if (this.initialHash == m.targetHash)
						return Action.Nothing;
					else
						return Action.Change;
				}
				else if (m.masterState == MasterStates.Deleted)
				{
					if (this.slaveState == SlaveStates.NonExist)
						return Action.Nothing;
					else
						return Action.Delete;
				}

				this.actionException = new NotImplementedException("MasterState " + m.masterState + " is unknown.");
				throw this.actionException;
			}
			else
				return Action.Delete;
		}

		private bool	Filter(string needle, List<string> filters)
		{
			int	j = 0;

			for (; j < filters.Count; j++)
			{
				if (needle.Contains(filters[j]) == true)
					return true;
			}

			return false;
		}

		public override string	ToString()
		{
			return this.path;
		}
	}
}