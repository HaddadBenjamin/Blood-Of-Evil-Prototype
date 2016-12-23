using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public class MissingScriptRecover : EditorWindow
	{
		public class RawComponent
		{
			public string		guid = string.Empty;
			public string		name;
			public Object		asset;
			public List<string>	fields = new List<string>();
			public List<KeyValuePair<Type, int>>	types = new List<KeyValuePair<Type, int>>();

			public bool	open;
		}

		public const string	Title = "Missing Script Recover";

		private GameObject		target;
		private List<RawComponent>	components = new List<RawComponent>();

		//[MenuItem("CONTEXT/Component/Diagnose Missing Script (BETA)")]
		private static void	Diagnose(MenuCommand command)
		{
			MissingScriptRecover	instance = EditorWindow.GetWindow<MissingScriptRecover>(true, MissingScriptRecover.Title);
			instance.Diagnose(Selection.activeGameObject);
			instance.Show();
		}

		
		[MenuItem("CONTEXT/Component/Diagnose Missing Script (BETA)", true)]
		private static bool	ValidateDiagnose(MenuCommand command)
		{
			return false;
		}

		protected virtual void	OnGUI()
		{
			EditorGUILayout.LabelField(this.target.name);

			for (int i = 0; i < this.components.Count; i++)
			{
				GUI.enabled = this.components[i].fields.Count > 0;
				this.components[i].open = EditorGUILayout.Foldout(this.components[i].open, this.components[i].name);
				GUI.enabled = true;

				if (this.components[i].open == true)
				{
					++EditorGUI.indentLevel;
					EditorGUILayout.LabelField("Fields:");
					++EditorGUI.indentLevel;
					for (int j = 0; j < this.components[i].fields.Count; j++)
						EditorGUILayout.LabelField(this.components[i].fields[j]);
					--EditorGUI.indentLevel;

					EditorGUILayout.LabelField("Potential types:");
					++EditorGUI.indentLevel;
					for (int l = 0; l < this.components[i].types.Count; l++)
					{
						if (this.components[i].types[l].Value == this.components[i].fields.Count)
							EditorGUILayout.LabelField(this.components[i].types[l].Key.FullName + " (Full match)");
						else
							EditorGUILayout.LabelField(this.components[i].types[l].Key.FullName + " (" + this.components[i].types[l].Value + " matching fields)");
					}
					--EditorGUI.indentLevel;
					--EditorGUI.indentLevel;
				}
			}
		}

		protected virtual void	Diagnose(GameObject	gameObject)
		{
			this.target = gameObject;
			this.components.Clear();

			PrefabType	prefabType = PrefabUtility.GetPrefabType(this.target);

			Object	prefab = null;

			if (prefabType == PrefabType.Prefab)
				prefab = this.target;
			else if (prefabType == PrefabType.PrefabInstance)
				prefab = PrefabUtility.GetPrefabParent(this.target);

			// Look into a prefab.
			if (prefab != null)
			{
				string	prefabPath = AssetDatabase.GetAssetPath(prefab);

				GameObject	prefabGameObject = prefab as GameObject;

				Component[]	components = null;

				if (prefabGameObject != null)
					components = prefabGameObject.GetComponents<Component>();

				if (EditorSettings.serializationMode == SerializationMode.ForceText)
				{
					string[]		lines = File.ReadAllLines(prefabPath);
					bool			inFields = false;

					List<string>	componentIDs = new List<string>();

					// Step 1: Detect GameObject and its Component
					for (int i = 0; i < lines.Length; i++)
					{
						if (lines[i].StartsWith("GameObject") == true)
						{
							inFields = true;
						}
						else if (inFields == true && lines[i].StartsWith("  m_Component") == true)
						{
							++i;
							for (; i < lines.Length; i++)
							{
								if (lines[i].StartsWith("  -") == false)
									break;

								string	id = lines[i].Substring(lines[i].IndexOf("fileID: ") + 8);
								componentIDs.Add(id.Remove(id.Length - 1));
							}
							break;
						}
					}

					inFields = false;

					for (int k = 0; k < componentIDs.Count; k++)
					{
						RawComponent	rawComponent = new RawComponent();

						this.components.Add(rawComponent);

						for (int i = 0; i < lines.Length; i++)
						{
							if (lines[i].StartsWith("---") == true && lines[i].EndsWith(componentIDs[k]) == true)
							{
								++i;
								rawComponent.name = lines[i].Remove(lines[i].Length - 1);

								if (lines[i] == "MonoBehaviour:")
								{
									++i;
									for (; i < lines.Length; i++)
									{
										if (rawComponent.guid == string.Empty && lines[i].StartsWith("  m_Script") == true)
										{
											int	n = lines[i].IndexOf("guid");

											rawComponent.guid = lines[i].Substring(n + 6, 32);
											string	path = AssetDatabase.GUIDToAssetPath(rawComponent.guid);

											if (components != null)
											{
												if (components[k] != null)
												{
													rawComponent.asset = components[k];
													rawComponent.name = rawComponent.asset.GetType().Name;
												}
											}

											if (string.IsNullOrEmpty(path) == true)
											{
												rawComponent.guid = string.Empty;
												rawComponent.asset = null;
											}
										}
										else if (inFields == true && lines[i].StartsWith("---") == true)
										{
											inFields = false;
											rawComponent.guid = string.Empty;

											foreach (Type type in Utility.EachAllSubClassesOf(typeof(MonoBehaviour)))
											{
												int	matchingFieldsCount = 0;

												for (int l = 0; l < rawComponent.fields.Count; l++)
												{
													if (type.GetField(rawComponent.fields[l]) != null)
														++matchingFieldsCount;
												}

												if (matchingFieldsCount > 0)
												{
													int	l = 0;

													for (; l < rawComponent.types.Count; l++)
													{
														if (rawComponent.types[l].Value < matchingFieldsCount)
														{
															rawComponent.types.Insert(l, new KeyValuePair<Type, int>(type, matchingFieldsCount));
															break;
														}
													}

													if (l >= rawComponent.types.Count)
														rawComponent.types.Add(new KeyValuePair<Type, int>(type, matchingFieldsCount));
												}
											}

											break;
										}
										else if (rawComponent.guid == string.Empty && lines[i].StartsWith("  m_EditorClassIdentifier") == true)
										{
											inFields = true;
										}
										else if (inFields == true)
										{
											if (lines[i][3] != ' ' && lines[i][3] != '-')
												rawComponent.fields.Add(lines[i].Substring(2, lines[i].Length - 3));
										}
									}
								}

								break;
							}

						}
					}
				}
				else
				{
					Debug.Log("Asset serialization mode must be set on ForceText to try recovering from the plain file.");
				}
			}
			// Look into a scene.
			else
			{
				// TODO Implemented looking into scene.
			}
		}
	}
}