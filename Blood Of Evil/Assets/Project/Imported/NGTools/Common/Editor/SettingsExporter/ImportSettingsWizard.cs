using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public class ImportSettingsWizard : ScriptableWizard
	{
		public enum ArrayImportOptions
		{
			/// <summary>Adds a new element in the array.</summary>
			Add,
			/// <summary>Overwrites an existing element when available, otherwise do nothing.</summary>
			Overwrite,
		}

		private class ImportNode
		{
			public SettingsExporter.Node	node;
			public ImportNode				parent;
			public ImportNode[]				children;

			public Type								instanceType;
			public object							instance;
			public Type								arrayElementType;
			public ExportableAttribute.ArrayOptions	arrayExportOptions;
			public IFieldModifier					fieldInfo;

			public bool					isImported = true;
			public ArrayImportOptions	arrayImportOption;

			public void	UpdateInstance()
			{
				if (this.parent == null)
					return;

				//if (this.HasParentAddImportOption() == true)
				//{
				//	this.instance = null;
				//	return;
				//}

				if (typeof(IEnumerable).IsAssignableFrom(this.parent.instanceType) == true)
				{
					this.instance = null;

					if (this.parent.instance != null)
					{
						IEnumerable	array = this.parent.instance as IEnumerable;

						foreach (var item in array)
						{
							if (item.GetType() == this.arrayElementType &&
								ImportSettingsWizard.trackObjects.Contains(item) == false)
							{
								this.instance = item;
								ImportSettingsWizard.trackObjects.Add(item);
								break;
							}
						}
					}

					//if (this.instance == null)
					//	this.instance = Activator.CreateInstance(this.arrayElementType);
				}
				// Look for field.
				else if (this.fieldInfo != null)
				{
					if (this.parent.instance != null)
						this.instance = this.fieldInfo.GetValue(this.parent.instance);
					else
						this.instance = null;
				}
			}

			private	ImportNode(ImportNode parent, SettingsExporter.Node node, Type workingType, object workingInstance)
			{
				this.parent = parent;
				this.node = node;

				// Instance is null only when the parent is an array.
				this.instanceType = workingType;
				// Look for class.
				//if (this.instanceType.FullName == node.name)
				//	this.instance = instance;
				// Look for array element.
				if (typeof(IEnumerable).IsAssignableFrom(this.instanceType) == true)
				{
					this.arrayElementType = Type.GetType(node.name);

					if (this.arrayElementType == null)
						Debug.LogWarning("Type \"" + node.name + "\" was not recognized.");
					else
					{
						foreach (ExportableAttribute attribute in this.arrayElementType.GetCustomAttributesIncludingBaseInterfaces<ExportableAttribute>())
						{
							this.arrayExportOptions = attribute.options;

							if ((attribute.options & ExportableAttribute.ArrayOptions.Add) != 0)
								this.arrayImportOption = ArrayImportOptions.Add;
							else if ((attribute.options & ExportableAttribute.ArrayOptions.Overwrite) != 0)
								this.arrayImportOption = ArrayImportOptions.Overwrite;
						}
					}

					this.instance = null;
					this.instanceType = this.arrayElementType;

					if (workingInstance != null)
					{
						IEnumerable	array = workingInstance as IEnumerable;

						foreach (var item in array)
						{
							if (item.GetType() == this.arrayElementType &&
								ImportSettingsWizard.trackObjects.Contains(item) == false)
							{
								this.instance = item;
								ImportSettingsWizard.trackObjects.Add(item);
								break;
							}
						}
					}

					//if (this.instance == null)
					//	this.instance = Activator.CreateInstance(this.arrayElementType);
				}
				// Look for field.
				else
				{
					FieldInfo	fieldInfo = this.instanceType.GetField(node.name, SettingsExporter.SearchFlags);

					if (fieldInfo != null)
					{
						if (fieldInfo.IsDefined(typeof(HideFromExportAttribute), true) == true)
							node.options = SettingsExporter.Node.Options.Hidden;

						this.fieldInfo = new FieldModifier(fieldInfo);

						if (this.fieldInfo.IsDefined(typeof(ExportableAttribute), true) == true)
						{
							this.arrayExportOptions = (this.fieldInfo.GetCustomAttributes(typeof(ExportableAttribute), true)[0] as ExportableAttribute).options;

							this.instanceType = this.fieldInfo.Type;

							if (workingInstance != null)
								this.instance = this.fieldInfo.GetValue(workingInstance);
						}
					}
					else
					{
						PropertyInfo	propertyInfo = this.instanceType.GetProperty(node.name, SettingsExporter.SearchFlags);

						if (propertyInfo == null)
							Debug.LogWarning("Name \"" + node.name + "\" was not found in " + this.instanceType.FullName + ".");
						else
						{
							if (propertyInfo.IsDefined(typeof(HideFromExportAttribute), true) == true)
								node.options = SettingsExporter.Node.Options.Hidden;

							this.fieldInfo = new PropertyModifier(propertyInfo);

							if (this.fieldInfo.IsDefined(typeof(ExportableAttribute), true) == true)
							{
								this.arrayExportOptions = (this.fieldInfo.GetCustomAttributes(typeof(ExportableAttribute), true)[0] as ExportableAttribute).options;

								this.instanceType = this.fieldInfo.Type;

								if (workingInstance != null)
									this.instance = this.fieldInfo.GetValue(workingInstance);
							}
						}
					}
					//else
					//	Debug.LogWarning("Field \"" + node.name + "\" is not decorated with the attribute \"" + typeof(ExportableAttribute) + "\" in " + this.instanceType.FullName + ".");
				}

				//if (this.instance != null)
				//{
					this.children = new ImportNode[node.children.Count];
					for (int i = 0; i < this.children.Length; i++)
						this.children[i] = new ImportNode(this, node.children[i], this.instanceType, this.instance);
				//}
				//else
				//	this.children = new ImportNode[0];
			}

			private	ImportNode(ImportNode parent, SettingsExporter.Node node)
			{
				this.parent = parent;
				this.node = node;

				this.arrayExportOptions = ExportableAttribute.ArrayOptions.Immutable | ExportableAttribute.ArrayOptions.Overwrite;
				this.arrayImportOption = ArrayImportOptions.Overwrite;

				this.instanceType = Type.GetType(node.name);

				if (this.instanceType != null)
				{
					UnityEngine.Object[]	instances = Resources.FindObjectsOfTypeAll(this.instanceType);

					if (instances.Length > 0)
					{
						this.instance = instances[0];
						this.children = new ImportNode[node.children.Count];

						for (int i = 0; i < this.children.Length; i++)
							this.children[i] = new ImportNode(this, node.children[i], this.instanceType, this.instance);
					}
				}
			}

			public	ImportNode(SettingsExporter.Node node)
			{
				this.parent = null;
				this.node = node;

				this.instance = null;
				this.instanceType = null;

				this.children = new ImportNode[node.children.Count];
				for (int i = 0; i < this.children.Length; i++)
					this.children[i] = new ImportNode(this, node.children[i]);
			}

			/// <summary>
			/// Checks if a parent is being added, excluding root, fields and immutables nodes.
			/// </summary>
			/// <returns></returns>
			public bool	HasParentAddImportOption()
			{
				ImportNode	n = this.parent;

				while (n != null)
				{
					if (n.parent != null && // Remove options from root and fields,
						n.fieldInfo == null && // they can only be overwritten.
						(n.arrayExportOptions & ExportableAttribute.ArrayOptions.Immutable) == 0 &&
						n.arrayImportOption == ArrayImportOptions.Add)
					{
						return true;
					}

					n = n.parent;
				}

				return false;
			}
		}

		private static List<object>	trackObjects = new List<object>();

		private ImportNode	root;
		private Vector2		scrollPosition;
		private GUIStyle	richTextField;

		public void	Init(SettingsExporter.Node root)
		{
			this.root = new ImportNode(root);
		}

		protected virtual void	Update()
		{
			if (EditorApplication.isCompiling == true)
			{
				this.Close();
				return;
			}
		}

		protected virtual void	OnGUI()
		{
			if (this.richTextField == null)
			{
				this.richTextField = new GUIStyle(GUI.skin.textField);
				this.richTextField.richText = true;
			}

			using (BgColorContentRestorer.Get(GeneralStyles.HighlightActionButton))
			{
				if (GUILayout.Button(LC.G("ImportSettings_Import")) == true)
				{
					ImportSettingsWizard.trackObjects.Clear();
					try
					{
						this.Import(this.root);
						InternalNGDebug.Log(LC.G("ImportSettings_ImportCompleted"));
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException(LC.G("ImportSettings_ImportFailed"), ex);
					}
				}
			}

			ImportSettingsWizard.trackObjects.Clear();
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				this.DrawNode(this.root);
			}
			EditorGUILayout.EndScrollView();
		}

		private void	DrawNode(ImportNode node)
		{
			node.UpdateInstance();

			if (node.parent != null && node.node.options == SettingsExporter.Node.Options.Normal)
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space((GUI.depth - 1) * 16F);

					EditorGUILayout.BeginVertical();
					{
						bool	hasParentAddImportOption = node.HasParentAddImportOption();

						EditorGUILayout.BeginHorizontal();
						{
							node.isImported = GUILayout.Toggle(node.isImported, Utility.NicifyVariableName(node.node.name));

							if (node.instanceType == null)
							{
								GUILayout.Label("<color=yellow>" + string.Format(LC.G("ImportSettings_TypeIsUnknown"), node.node.name) + "</color>", this.richTextField);
								node.isImported = false;
							}
							else if (node.instance == null && node.parent.parent == null) // Root instance is missing.
							{
								GUILayout.Label("<color=yellow>" + string.Format(LC.G("ImportSettings_NoInstanceOfTypeFound"), node.node.name) + "</color>", this.richTextField);
								node.isImported = false;
							}
							else
							{
								// Do not show array, but since string is an array...
								if (typeof(IEnumerable).IsAssignableFrom(node.instanceType) == false || node.instanceType == typeof(string))
								{
									// Overwrite is available only if parents are not adding.
									if (hasParentAddImportOption == false)
									{
										if (string.IsNullOrEmpty(node.node.value) == false)
										{
											if (typeof(Object).IsAssignableFrom(node.instanceType) == true)
											{
												Object	o = node.instance as Object;
												string	GUID = AssetDatabase.GetAssetPath(o);

												GUID = AssetDatabase.AssetPathToGUID(GUID);

												// New value will overwrite.
												if (node.node.value != GUID)
													GUILayout.Label("<color=grey>" + node.instance + "</color> < <color=green>" + node.node.value + "</color>", this.richTextField);
												// Values are equal.
												else
													GUILayout.Label("<color=grey>" + node.node.value + "</color>", this.richTextField);
											}
											else if (node.instance != null)
											{
												// New value will overwrite.
												if (node.node.value != node.instance.ToString())
													GUILayout.Label("<color=grey>" + node.instance + "</color> < <color=green>" + node.node.value + "</color>", this.richTextField);
												// Values are equal.
												else
													GUILayout.Label("<color=grey>" + node.node.value + "</color>", this.richTextField);
											}
											// New value.
											else if (string.IsNullOrEmpty(node.node.value) == false)
												GUILayout.Label("<color=cyan>" + node.node.value + "</color>", this.richTextField);
										}
									}
									// New value.
									else if (string.IsNullOrEmpty(node.node.value) == false)
										GUILayout.Label("<color=cyan>" + node.node.value + "</color>", this.richTextField);
								}

								if (node.parent != null && // Remove options from root and fields,
									node.fieldInfo == null && // they can only be overwritten.
									(node.arrayExportOptions & ExportableAttribute.ArrayOptions.Immutable) == 0) // Hide it from Immutable classes and arrays. e.g. MainLog can only be overwritten.
								{
									// When a parent is adding, children can only be added (Without changing the import option).
									if (hasParentAddImportOption == true)
									{
										GUI.enabled = false;
										EditorGUILayout.EnumPopup(string.Empty, ArrayImportOptions.Add); // LC.G("Options")
										GUI.enabled = true;
									}
									else
									{
										node.arrayImportOption = (ArrayImportOptions)EditorGUILayout.EnumPopup(string.Empty, node.arrayImportOption);
									}
								}

								GUILayout.FlexibleSpace();
							}
						}
						EditorGUILayout.EndHorizontal();

						// Check if the node has an instance to overwrite. Of course, when a parent is adding, there is no overwrite anymore.
						if (hasParentAddImportOption == false &&
							node.arrayImportOption == ArrayImportOptions.Overwrite && node.instance == null)
						{
							GUILayout.Label("<color=yellow>" + LC.G("ImportSettings_CantOverwriteInstance") + "</color>", "CN EntryWarn");

							if (node.isImported == true)
								node.arrayImportOption = ArrayImportOptions.Add;
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}

			if (node.isImported == false || node.children == null)
				return;

			if (node.parent != null)
				++GUI.depth;
			for (int i = 0; i < node.children.Length; i++)
				this.DrawNode(node.children[i]);
			if (node.parent != null)
				--GUI.depth;
		}

		private void	Import(ImportNode node)
		{
			InternalNGDebug.Log(node.node.name + " " + node.instanceType + " | " + node.instance + " # " + node.isImported + " ~ " + node.parent + " & " + node.fieldInfo);

			if (node.isImported == false || (node.instance == null && node.parent != null && node.parent.parent == null))
				return;

			if (node.parent != null)
			{
				// Overwrite field.
				if (node.fieldInfo != null)
				{
					// Adding classes may have filled their fields in the constructor, therefore we need to update the instance, to look for actual value.
					node.UpdateInstance();

					// Apply per type.
					if (node.fieldInfo.Type.IsClass == false &&
						node.fieldInfo.Type.IsStruct() == false)
					{
						if (node.fieldInfo.Type == typeof(int))
						{
							InternalNGDebug.Log("fieldInfo.int(" + node.parent.instance + ", " + node.node.value + ")");
							node.fieldInfo.SetValue(node.parent.instance, int.Parse(node.node.value));
						}
						else if (node.fieldInfo.Type == typeof(bool))
						{
							InternalNGDebug.Log("fieldInfo.bool(" + node.parent.instance + ", " + node.node.value + ")");
							node.fieldInfo.SetValue(node.parent.instance, node.node.value[0] == 'T');
						}
						else if (node.fieldInfo.Type == typeof(float))
						{
							InternalNGDebug.Log("fieldInfo.float(" + node.parent.instance + ", " + node.node.value + ")");
							node.fieldInfo.SetValue(node.parent.instance, float.Parse(node.node.value));
						}
					}
					else if ((node.arrayExportOptions & ExportableAttribute.ArrayOptions.Immutable) == 0 ||
							 node.instance == null)
					{
						if (typeof(Object).IsAssignableFrom(node.fieldInfo.Type))
						{
							string	assetPath = AssetDatabase.GUIDToAssetPath(node.node.value);

							InternalNGDebug.Log("fieldInfo.object(" + node.node.value + " > " + assetPath + ")");
							if (string.IsNullOrEmpty(assetPath) == false)
								node.fieldInfo.SetValue(node.parent.instance, AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
						}
						else if (node.fieldInfo.Type == typeof(string))
						{
							InternalNGDebug.Log("fieldInfo.string(" + node.parent.instance + ", " + node.node.value + ")");
							node.fieldInfo.SetValue(node.parent.instance, node.node.value);
						}
						else
						{
							if (node.fieldInfo.Type.IsArray == true)
							{
								InternalNGDebug.Log("Array.CreateInstance(" + node.instanceType + ") " + node.arrayExportOptions + "	" + node.instance + "	" + node.node.value);
								node.instance = Array.CreateInstance(node.fieldInfo.Type.GetElementType(), 0);
							}
							else
							{
								InternalNGDebug.Log("Activator.CreateInstance(" + node.instanceType + ") " + node.arrayExportOptions + "	" + node.instance + "	" + node.node.value);
								node.instance = Activator.CreateInstance(node.instanceType);
							}
							node.fieldInfo.SetValue(node.parent.instance, node.instance);
						}
					}
				}
				// Overwrite, add element in array.
				else if (node.arrayElementType != null)
				{
					if (node.HasParentAddImportOption() == true && node.parent.instance == null)
					{
						InternalNGDebug.Log("Generate new array[" + node.arrayElementType + "] in " + node.parent.parent.instance + " :: " + node.parent.fieldInfo);
						node.parent.instance = Array.CreateInstance(node.arrayElementType, 0);
						node.parent.fieldInfo.SetValue(node.parent.parent.instance, node.parent.instance);
					}
					if ((node.HasParentAddImportOption() == true || node.arrayImportOption == ArrayImportOptions.Add) &&
						(node.arrayExportOptions & ExportableAttribute.ArrayOptions.Immutable) == 0)
					{
						InternalNGDebug.Log("Array[" + node.arrayElementType + "].Add(" + node.parent.instance + ")" + GetElementTypeOfEnumerable(node.parent.instance));
						object	element = Activator.CreateInstance(node.arrayElementType);

						// Look for an existing element in array.
						IEnumerable	array = node.parent.instance as IEnumerable;
						IEnumerator	it = array.GetEnumerator();
						int			count = 0;

						while (it.MoveNext())
							++count;

						if (node.parent.instanceType.IsArray == true)
						{
							Array	copy = Array.CreateInstance(node.parent.instanceType.GetElementType(), count + 1);
							count = 0;
							foreach (var item in array)
							{
								copy.SetValue(item, count);
								++count;
							}

							copy.SetValue(element, count);
							node.parent.fieldInfo.SetValue(node.parent.parent.instance, copy);
						}
						else if (node.parent.instance is IList)
						{
							IList	list = node.parent.instance as IList;

							list.Add(element);
						}

						ImportSettingsWizard.trackObjects.Add(element);

						node.instance = element;
					}
					else if (node.arrayImportOption == ArrayImportOptions.Overwrite ||
							 ((node.arrayExportOptions & (ExportableAttribute.ArrayOptions.Overwrite | ExportableAttribute.ArrayOptions.Immutable)) != 0))
					{
						InternalNGDebug.Log("Array[" + node.arrayElementType + "].Overwrite(" + node.instance + ")");
						IEnumerable	array = node.parent.instance as IEnumerable;

						node.instance = null;

						foreach (var item in array)
						{
							if (item.GetType().FullName == node.node.name &&
								ImportSettingsWizard.trackObjects.Contains(item) == false)
							{
								InternalNGDebug.Log("	Found " + item);
								node.instance = item;
								ImportSettingsWizard.trackObjects.Add(item);
								break;
							}
						}
					}
				}
			}

			if (node.instance is ISettingExportable)
				(node.instance as ISettingExportable).PreImport();

			for (int i = 0; i < node.children.Length; i++)
			{
				this.Import(node.children[i]);

				if (node.fieldInfo != null && node.parent != null)
					node.fieldInfo.SetValue(node.parent.instance, node.instance);
			}

			if (node.instance is ISettingExportable)
				(node.instance as ISettingExportable).PostImport();
		}

		private static Type GetElementTypeOfEnumerable(object o)
		{
			var	enumerable = o as IEnumerable;
			// if it's not an enumerable why do you call this method all ?
			if (enumerable == null)
				return null;

			Type[]	interfaces = enumerable.GetType().GetInterfaces();

			return (from i in interfaces
					where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
					select i.GetGenericArguments()[0]).FirstOrDefault();
		}
	}
}