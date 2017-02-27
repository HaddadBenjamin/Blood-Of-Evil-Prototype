using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	[InitializeOnLoad]
	public class NGRemoteInspectorWindow : NGRemoteWindow, IHasCustomMenu
	{
		public const string	NormalTitle = "ƝƓ Ʀemote Ɨnspector";
		public const string	ShortTitle = "ƝƓ Ʀ Ɨnspector";
		public const int	ForceRepaintRefreshTick = 10;
		public const float	ComponentSpacing = 2F;
		[SetColor(64F / 255F, 64F / 255F, 64F / 255F, 1F, 210F / 255F, 210F / 255F, 210F / 255F, 1F)]
		public static Color	HeaderBackgroundColor = default(Color);
		[SetColor(62F / 255F, 62F / 255F, 62F / 255F, 1F, 162F / 255F, 162F / 255F, 162F / 255F, 1F)]
		public static Color	MaterialHeaderBackgroundColor = default(Color);

		public Vector2	scrollPosition;
		public Rect		bodyRect;
		public Rect		viewRect;
		public Rect		r;

		private bool				isLock;
		private ClientGameObject	target;
		private Vector2				scrollBatchPosition;
		private int					selectedWindow;
		private int					selectedBatch;
		private List<int>			renderingMaterials = new List<int>();
		private List<bool>			foldoutMaterials = new List<bool>();
		private int					lastMaterialsHash = -1;

		private BgColorContentAnimator	animActive;
		private BgColorContentAnimator	animName;
		private BgColorContentAnimator	animIsStatic;
		private BgColorContentAnimator	animTag;
		private BgColorContentAnimator	animLayer;

		private TypeHandler	booleanHandler;
		private TypeHandler	stringHandler;
		private TypeHandler	intHandler;
		private TypeHandler	floatHandler;
		private TypeHandler	colorHandler;
		private TypeHandler	vector4Handler;
		
		[NonSerialized]
		private Texture2D	gameObjectIcon = null;

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGRemoteInspectorWindow.NormalTitle + ", change GameObject, toggle Component, change any values. Unfortunately exceptions raised here require you to contact the author.");

		static	NGRemoteInspectorWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGRemoteInspectorWindow.NormalTitle);
		}

		[MenuItem(Constants.MenuItemPath + NGRemoteInspectorWindow.NormalTitle, priority = Constants.MenuItemPriority + 215)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGRemoteInspectorWindow>(NGRemoteInspectorWindow.ShortTitle);
		}

		protected virtual void	OnEnable()
		{
			this.SetTitle(NGRemoteInspectorWindow.ShortTitle);
			this.animActive = new BgColorContentAnimator(this.Repaint, 1F, 0F);
			this.animName = new BgColorContentAnimator(this.Repaint, 1F, 0F);
			this.animIsStatic = new BgColorContentAnimator(this.Repaint, 1F, 0F);
			this.animTag = new BgColorContentAnimator(this.Repaint, 1F, 0F);
			this.animLayer = new BgColorContentAnimator(this.Repaint, 1F, 0F);

			this.booleanHandler = TypeHandlersManager.GetTypeHandler<bool>();
			this.stringHandler = TypeHandlersManager.GetTypeHandler<string>();
			this.intHandler = TypeHandlersManager.GetTypeHandler<int>();
			this.floatHandler = TypeHandlersManager.GetTypeHandler<float>();
			this.colorHandler = TypeHandlersManager.GetTypeHandler<Color>();
			this.vector4Handler = TypeHandlersManager.GetTypeHandler<Vector4>();

			this.minSize = new Vector2(275F, this.minSize.y);

			this.bodyRect = new Rect();
			this.viewRect = new Rect();
			this.r = new Rect();

			this.selectedWindow = 0;

			this.lastMaterialsHash = -1;

			this.gameObjectIcon = AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));

			Utility.RegisterIntervalCallback(this.Repaint, NGRemoteInspectorWindow.ForceRepaintRefreshTick);
			Utility.LoadEditorPref(this);
		}

		protected override void	OnDisable()
		{
			base.OnDisable();

			Utility.UnregisterIntervalCallback(this.Repaint);
			Utility.SaveEditorPref(this);
		}

		protected override void	OnGUIHeader()
		{
			this.errorPopup.OnGUILayout();

			EditorGUIUtility.wideMode = true;

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Toggle(this.selectedWindow == 0, LC.G("NGInspector_Inspector"), GeneralStyles.ToolbarToggle) == true)
					this.selectedWindow = 0;

				if (this.Hierarchy.Client != null)
				{
					if (this.Hierarchy.Client.batchMode == Client.BatchMode.On)
					{
						if (GUILayout.Toggle(this.selectedWindow == 1, LC.G("NGInspector_Batch"), GeneralStyles.ToolbarToggle) == true)
							this.selectedWindow = 1;
					}
				}

				GUILayout.FlexibleSpace();

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.selectedWindow == 2, LC.G("NGInspector_Historic"), GeneralStyles.ToolbarToggle);
				if (EditorGUI.EndChangeCheck() == true)
					this.selectedWindow = 2;

				if (this.Hierarchy.Client != null)
				{
					EditorGUI.BeginChangeCheck();
					GUILayout.Toggle(this.Hierarchy.Client.batchMode == Client.BatchMode.On, LC.G("NGInspector_Batch"), GeneralStyles.ToolbarToggle);
					if (EditorGUI.EndChangeCheck() == true)
					{
						if (this.Hierarchy.Client.batchMode == Client.BatchMode.On && this.Hierarchy.Client.batchedPackets.Count > 0)
						{
							if ((Event.current.modifiers & Constants.ByPassPromptModifier) != 0 || EditorUtility.DisplayDialog(LC.G("NGInspector_Batch"), LC.G("NGHierarchy_RequesttoSendCurrentBatch"), LC.G("Yes"), LC.G("No")) == true)
								this.Hierarchy.Client.ExecuteBatch();
						}

						this.Hierarchy.Client.batchMode = this.Hierarchy.Client.batchMode == Client.BatchMode.On ? Client.BatchMode.Off : Client.BatchMode.On;

						if (this.Hierarchy.Client.batchMode == Client.BatchMode.Off)
							this.selectedWindow = 0;
					}

					if (GUILayout.Button(LC.G("NGInspector_Refresh"), GeneralStyles.ToolbarButton) == true)
						this.Hierarchy.Client.AddPacket(new ClientRequestGameObjectDataPacket(this.target.instanceID));
				}
				else
				{
					EditorGUI.BeginDisabledGroup(true);
					GUILayout.Toggle(false, LC.G("NGInspector_Batch"), GeneralStyles.ToolbarToggle);
					GUILayout.Button(LC.G("NGInspector_Refresh"), GeneralStyles.ToolbarButton);
					EditorGUI.EndDisabledGroup();
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		protected override void	OnGUIConnected()
		{
			if (this.Hierarchy.Client.batchMode == Client.BatchMode.On &&
				this.selectedWindow == 1)
			{
				this.DrawBatch();
				return;
			}
			if (this.selectedWindow == 2)
			{
				this.DrawPacketsHistoric();
				return;
			}

			if (this.isLock == false)
			{
				ClientGameObject[]	selection = this.Hierarchy.GetSelectedGameObjects();

				if (selection.Length > 0)
				{
					if (this.target != selection[0])
					{
						this.target = selection[0];
						this.target.RequestBehaviours(this.Hierarchy.Client);
						this.Hierarchy.WatchGameObject(this, this.target);
					}
				}
				else
				{
					if (this.target != null)
					{
						this.target = null;
						this.Hierarchy.WatchGameObject(this, null);
					}
				}
			}

#if NGTOOLS_FREE
			EditorGUILayout.HelpBox("NG Remote Inspector is read-only. You can only toggle the GameObject's active state below.", MessageType.Info);
#endif

			if (this.target == null)
				return;

			this.DrawHeader();

			if (this.target.components == null)
			{
				EditorGUILayout.LabelField(LC.G("NGInspector_NoComponentAvailable"));
				return;
			}

			this.bodyRect.y = GUILayoutUtility.GetLastRect().yMax + NGRemoteInspectorWindow.ComponentSpacing;
			this.bodyRect.width = this.position.width;
			this.bodyRect.height = this.position.height - this.bodyRect.y;
			this.viewRect.height = 0F;

			for (int i = 0; i < this.target.components.Count; i++)
			{
				try
				{
					this.viewRect.height += this.target.components[i].GetHeight(this) + NGRemoteInspectorWindow.ComponentSpacing;
				}
				catch
				{
					this.viewRect.height += 16F + NGRemoteInspectorWindow.ComponentSpacing;
				}
			}

			this.PopulateMaterials(this.renderingMaterials);

			for (int i = 0; i < this.renderingMaterials.Count; i++)
			{
				NetMaterial	material = this.Hierarchy.GetMaterial(this.renderingMaterials[i]);

				this.viewRect.height += 16F + 2F; // Material title + Top/Bottom line separator.

				if (material != null)
				{
					int	 n = 0;

					for (int j = 0; j < material.properties.Length; j++)
					{
						if (material.properties[j].hidden == true)
							continue;

						if (material.properties[j].type == NGShader.ShaderPropertyType.TexEnv)
							n += 3;
						else
							++n;
					}

					this.viewRect.height += (1 + n) * EditorGUIUtility.singleLineHeight + ClientComponent.Spacing; // Shader header + Properties + Title spacing.
				}
			}

			this.scrollPosition = GUI.BeginScrollView(this.bodyRect, this.scrollPosition, this.viewRect);
			{
				this.r = this.bodyRect;
				this.r.y = 0F;

				if (this.viewRect.height >= this.bodyRect.height)
					this.r.width -= 16F;

				for (int i = 0; i < this.target.components.Count; i++)
				{
					try
					{
						float	height = this.target.components[i].GetHeight(this);

						this.r.height = height;
						if (this.r.y + height <= this.scrollPosition.y)
						{
							continue;
						}

						this.target.components[i].OnGUI(this.r, this);
					}
					catch (Exception ex)
					{
						if (Event.current.type == EventType.Repaint)
							EditorGUI.DrawRect(this.r, Color.red * .5F);

						this.errorPopup.exception = ex;
						this.errorPopup.customMessage = "Component " + this.target.components[i].name + " (" + i + ") failed to render.";
					}
					finally
					{
						this.r.y += this.r.height + NGRemoteInspectorWindow.ComponentSpacing;
					}

					if (this.r.y - this.scrollPosition.y > this.bodyRect.height)
						break;
				}

				if (this.renderingMaterials.Count > 0 && this.r.y - this.scrollPosition.y < this.bodyRect.height)
					this.DrawMaterials(this.r, this.renderingMaterials);
			}
			GUI.EndScrollView();

			int	hash = 0;

			for (int i = 0; i < this.renderingMaterials.Count; i++)
				hash += this.renderingMaterials[i];

			if (hash != this.lastMaterialsHash)
			{
				this.lastMaterialsHash = hash;
				this.Hierarchy.WatchMaterials(this, this.renderingMaterials);
			}
		}

		private void	PopulateMaterials(List<int> materials)
		{
			materials.Clear();

			for (int i = 0; i < this.target.components.Count; i++)
			{
				if (this.target.components[i].type != null &&
					(this.target.components[i].type.IsSubclassOf(typeof(Renderer)) == true || // Handle all renderers.
					 (this.target.components[i].type.IsSubclassOf(typeof(Behaviour)) == true && // And those bastards like Projector.
					  this.target.components[i].type.IsSubclassOf(typeof(MonoBehaviour)) == false)))
				{
					for (int j = 0; j < this.target.components[i].fields.Length; j++)
					{
						if (this.target.components[i].fields[j].name.Equals("sharedMaterials") == true)
						{
							ArrayData		array = this.target.components[i].fields[j].value as ArrayData;
							UnityObject[]	sharedMaterials = array.array as UnityObject[];

							for (int k = 0; k < sharedMaterials.Length; k++)
							{
								if (sharedMaterials[k].instanceID != 0 &&
									materials.Contains(sharedMaterials[k].instanceID) == false)
								{
									materials.Add(sharedMaterials[k].instanceID);
								}
							}
						}
						else if (this.target.components[i].fields[j].name.Equals("material") == true)
						{
							UnityObject	material = this.target.components[i].fields[j].value as UnityObject;

							if (material.instanceID != 0 &&
								materials.Contains(material.instanceID) == false)
							{
								materials.Add(material.instanceID);
							}
						}
					}
				}
			}
		}

		private void	DrawMaterials(Rect r, List<int> materials)
		{
			string[]	shaderNames;
			int[]		shaderInstanceIDs;
			float		width = r.width;
			Utility.content.text = LC.G("Change");
			float		changeWidth = GUI.skin.button.CalcSize(Utility.content).x;

			this.Hierarchy.LoadResources(typeof(Material));
			this.Hierarchy.GetResources(typeof(Shader), out shaderNames, out shaderInstanceIDs);

			r.height = 16F;

			for (int i = 0; i < materials.Count; i++)
			{
				NetMaterial	material = this.Hierarchy.GetMaterial(materials[i]);

				if (material != null)
				{
					while (this.foldoutMaterials.Count <= i)
						this.foldoutMaterials.Add(true);

					r.height = 1F;
					r.y += 1F;
					EditorGUI.DrawRect(r, Color.black);
					r.y -= 1F;
					r.height = 16F;

					r.y += ClientComponent.Spacing;

					if (Event.current.type == EventType.Repaint)
						EditorGUI.DrawRect(r, NGRemoteInspectorWindow.MaterialHeaderBackgroundColor);
					this.foldoutMaterials[i] = EditorGUI.Foldout(r, this.foldoutMaterials[i], "Material " + material.name, true);
					r.y += r.height;

					if (this.foldoutMaterials[i] == false)
						continue;

					++EditorGUI.indentLevel;

					if (Event.current.type == EventType.Repaint)
						EditorGUI.DrawRect(r, NGRemoteInspectorWindow.MaterialHeaderBackgroundColor);

					if (shaderNames != null)
					{
						if (material.selectedShader == -1)
						{
							for (int j = 0; j < shaderNames.Length; j++)
							{
								if (shaderNames[j].Equals(material.shader) == true)
								{
									material.selectedShader = j;
									break;
								}
							}
						}

						r.width -= changeWidth;
						material.selectedShader = EditorGUI.Popup(r, "Shader", material.selectedShader, shaderNames);
						r.x += r.width;

						r.width = changeWidth;
						if (GUI.Button(r, LC.G("Change")) == true)
							this.Hierarchy.AddPacket(new ClientChangeMaterialShaderPacket(materials[i], shaderInstanceIDs[material.selectedShader]));

						r.x = 0F;
						r.width = width;
					}
					else
						EditorGUI.LabelField(r, "Shader", LC.G("NGInspector_NotAvailableYet"));
					r.y += r.height;

					r.height = 1F;
					r.y += 1F;
					EditorGUI.DrawRect(r, Color.gray);
					r.y += 1F;
					r.height = 16F;

					for (int j = 0; j < material.properties.Length; j++)
					{
						if (material.properties[j].hidden == true)
							continue;

						if (material.properties[j].type == NGShader.ShaderPropertyType.Color)
						{
							EditorGUI.BeginChangeCheck();
							Color	newValue = EditorGUI.ColorField(r, material.properties[j].displayName, material.properties[j].colorValue);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materials[i], material.properties[j].name, this.colorHandler.Serialize(newValue)));
						}
						else if (material.properties[j].type == NGShader.ShaderPropertyType.Float)
						{
							EditorGUI.BeginChangeCheck();
							float	newValue = EditorGUI.FloatField(r, material.properties[j].displayName, material.properties[j].floatValue);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materials[i], material.properties[j].name, this.floatHandler.Serialize(newValue)));
						}
						else if (material.properties[j].type == NGShader.ShaderPropertyType.Range)
						{
							EditorGUI.BeginChangeCheck();
							float	newValue = EditorGUI.Slider(r, material.properties[j].displayName, material.properties[j].floatValue, material.properties[j].rangeMin, material.properties[j].rangeMax);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materials[i], material.properties[j].name, this.floatHandler.Serialize(newValue)));
						}
						else if (material.properties[j].type == NGShader.ShaderPropertyType.TexEnv)
						{
							UnityObject	unityObject = material.properties[j].textureValue;

							r.width -= 20F;
							EditorGUI.LabelField(r, Utility.NicifyVariableName(material.properties[j].displayName), unityObject.instanceID + " " + unityObject.name);
							r.x += r.width;

							r.width = 20F;
							if (GUI.Button(r, "P") == true)
								this.Hierarchy.PickupResource(typeof(Texture), materials[i] + "." + material.properties[j].name, this.UpdateMaterialTexture, unityObject.instanceID);

							++EditorGUI.indentLevel;
							r.x = 0F;
							r.width = width;

							r.y += r.height;
							EditorGUI.BeginChangeCheck();
							Vector2	newValue = EditorGUI.Vector2Field(r, "Tiling", material.properties[j].textureScale);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.AddPacket(new ClientUpdateMaterialVector2Packet(materials[i], material.properties[j].name, newValue, MaterialVector2Type.Scale));

							r.y += r.height;
							EditorGUI.BeginChangeCheck();
							newValue = EditorGUI.Vector2Field(r, "Offset", material.properties[j].textureOffset);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.AddPacket(new ClientUpdateMaterialVector2Packet(materials[i], material.properties[j].name, newValue, MaterialVector2Type.Offset));
							--EditorGUI.indentLevel;

							r.x = 0F;
							r.width = width;
						}
						else if (material.properties[j].type == NGShader.ShaderPropertyType.Vector)
							this.DrawVector4(r, materials[i], material.properties[j]);

						r.y += r.height;
					}

					--EditorGUI.indentLevel;
				}
				else
					EditorGUI.LabelField(r, "Material " + (i + 1).ToString(), "Not loaded yet.");
			}
		}

		private void	DrawVector4(Rect r, int materialInstanceID, NetMaterialProperty property)
		{
			Vector4	vector = property.vectorValue;
			float	labelWidth;
			float	controlWidth;

			Utility.CalculSubFieldsWidth(r.width, 44F, 4, out labelWidth, out controlWidth);

			r.width = labelWidth;
			EditorGUI.LabelField(r, Utility.NicifyVariableName(property.name));
			r.x += r.width;

			int	iL = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			using (LabelWidthRestorer.Get(14F))
			{
				r.width = controlWidth;

				EditorGUI.BeginChangeCheck();
				Single	v = EditorGUI.FloatField(r, "X", vector.x);
				r.x += r.width;
				if (EditorGUI.EndChangeCheck() == true)
				{
					property.vectorValue.x = v;
					this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materialInstanceID, property.name, this.vector4Handler.Serialize(property.vectorValue)));
				}

				EditorGUI.BeginChangeCheck();
				v = EditorGUI.FloatField(r, "Y", vector.y);
				r.x += r.width;
				if (EditorGUI.EndChangeCheck() == true)
				{
					property.vectorValue.y = v;
					this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materialInstanceID, property.name, this.vector4Handler.Serialize(property.vectorValue)));
				}

				EditorGUI.BeginChangeCheck();
				v = EditorGUI.FloatField(r, "Z", vector.z);
				r.x += r.width;
				if (EditorGUI.EndChangeCheck() == true)
				{
					property.vectorValue.z = v;
					this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materialInstanceID, property.name, this.vector4Handler.Serialize(property.vectorValue)));
				}

				EditorGUI.BeginChangeCheck();
				v = EditorGUI.FloatField(r, "W", vector.w);
				if (EditorGUI.EndChangeCheck() == true)
				{
					property.vectorValue.w = v;
					this.Hierarchy.AddPacket(new ClientUpdateMaterialPropertyPacket(materialInstanceID, property.name, this.vector4Handler.Serialize(property.vectorValue)));
				}
			}
			EditorGUI.indentLevel = iL;
		}

		private Packet	UpdateMaterialTexture(string valuePath, byte[] rawValue)
		{
			string[]	data = valuePath.Split('.');

			return new ClientUpdateMaterialPropertyPacket(int.Parse(data[0]), data[1], rawValue);
		}

		private void	DrawHeader()
		{
			EditorGUILayout.BeginHorizontal();
			{
				this.r = EditorGUILayout.GetControlRect();

				Rect	r2 = this.r;
				r2.x = 0F;
				r2.width = this.position.width;
				r2.height += 3F;
				EditorGUI.DrawRect(r2, NGRemoteInspectorWindow.HeaderBackgroundColor);

				this.r.x += 32F;

				float	w = this.r.width -= 50F + 32F;
				this.r.width = 16F;

				if (this.Hierarchy.GetUpdateNotification(this.target.instanceID + ".active") == true)
					this.animActive.Start();

				using (this.animActive.Restorer(0F, .8F + this.animActive.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					bool	active = GUI.Toggle(this.r, this.target.active, GUIContent.none);
					if (EditorGUI.EndChangeCheck() == true)
						this.Hierarchy.Client.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".active", this.booleanHandler.Serialize(active), this.booleanHandler));
				}

				if (this.Hierarchy.GetUpdateNotification(this.target.instanceID + ".name") == true)
					this.animName.Start();

				this.r.x += 16F;
				this.r.width = w - 16F;

				using (this.animName.Restorer(0F, .8F + this.animName.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					string	name = EditorGUI.TextField(this.r, this.target.name);
					if (EditorGUI.EndChangeCheck() == true)
						this.Hierarchy.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".name", this.stringHandler.Serialize(name), this.stringHandler));
				}

				if (this.Hierarchy.GetUpdateNotification(this.target.instanceID + ".isStatic") == true)
					this.animIsStatic.Start();

				this.r.x += this.r.width;
				this.r.width = 50F;

				using (this.animIsStatic.Restorer(0F, .8F + this.animIsStatic.Value, 0F, 1F))
				{
					EditorGUI.BeginChangeCheck();
					bool	isStatic = GUI.Toggle(this.r, this.target.isStatic, "Static");
					if (EditorGUI.EndChangeCheck() == true)
						this.Hierarchy.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".isStatic", this.booleanHandler.Serialize(isStatic), this.booleanHandler));
				}
			}
			EditorGUILayout.EndHorizontal();

			if ((this.target.ready & GameObjectReady.TagAndLayer) != 0)
			{
				EditorGUILayout.BeginHorizontal();
				{
					this.r = EditorGUILayout.GetControlRect();

					Rect	r2 = this.r;
					r2.x = 0F;
					r2.width = this.position.width;
					r2.height += 4F;
					EditorGUI.DrawRect(r2, NGRemoteInspectorWindow.HeaderBackgroundColor);

					r2.x += 2F;
					r2.y -= 20F;
					r2.width = 24F;
					r2.height = 24F;
					GUI.DrawTexture(r2, this.gameObjectIcon);

					if (this.Hierarchy.GetUpdateNotification(this.target.instanceID + ".tag") == true)
						this.animTag.Start();

					this.r.x += 18F;
					this.r.width -= 18F;

					using (this.animTag.Restorer(0F, .8F + this.animTag.Value, 0F, 1F))
					{
						using (LabelWidthRestorer.Get(30F))
						{
							if (this.Hierarchy.syncTags == true)
							{
								EditorGUI.BeginChangeCheck();
								string	tag = EditorGUI.TagField(this.r, "Tag", this.target.tag);
								if (EditorGUI.EndChangeCheck() == true)
									this.Hierarchy.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".tag", this.stringHandler.Serialize(tag), this.stringHandler));
							}
							else
							{
								EditorGUI.BeginChangeCheck();
								string	tag = EditorGUI.TextField(this.r, "Tag", this.target.tag);
								if (EditorGUI.EndChangeCheck() == true)
									this.Hierarchy.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".tag", this.stringHandler.Serialize(tag), this.stringHandler));
							}
						}
					}

					this.r = EditorGUILayout.GetControlRect();

					if (this.Hierarchy.GetUpdateNotification(this.target.instanceID + ".layer") == true)
						this.animLayer.Start();

					using (this.animLayer.Restorer(0F, .8F + this.animLayer.Value, 0F, 1F))
					{
						using (LabelWidthRestorer.Get(40F))
						{
							if ((this.Hierarchy.ready & HierarchyReady.Layers) != 0)
							{
								EditorGUI.BeginChangeCheck();
								int	layer = EditorGUI.Popup(this.r, "Layer", this.target.layer, this.Hierarchy.Layers);
								if (EditorGUI.EndChangeCheck() == true)
									this.Hierarchy.AddPacket(new ClientUpdateFieldValuePacket(this.target.instanceID + ".layer", this.intHandler.Serialize(layer), this.intHandler));
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}

		public void	DrawPacketsHistoric()
		{
			this.bodyRect = new Rect(0F, EditorGUIUtility.singleLineHeight + NGRemoteInspectorWindow.ComponentSpacing, this.position.width, this.position.height - EditorGUIUtility.singleLineHeight);
			this.viewRect.height = this.Hierarchy.Client.sentPacketsHistoric.Count * EditorGUIUtility.singleLineHeight;
			this.r.y = 0F;
			this.r.height = EditorGUIUtility.singleLineHeight;

			this.scrollBatchPosition = GUI.BeginScrollView(this.bodyRect, this.scrollBatchPosition, this.viewRect);
			{
				if (this.viewRect.height >= this.bodyRect.height)
					this.bodyRect.width -= 16F;

				for (int i = this.Hierarchy.Client.sentPacketsHistoric.Count - 1; i >= 0; --i)
				{
					if (this.r.y + this.r.height <= this.scrollBatchPosition.y)
					{
						this.r.y += this.r.height;
						continue;
					}

					this.r.x = 0F;

					if (i >= 9999)
						this.r.width = 50F;
					else if (i >= 999)
						this.r.width = 42F;
					else if (i >= 99)
						this.r.width = 34F;
					else if (i >= 9)
						this.r.width = 26F;
					else
						this.r.width = 18F;

					EditorGUI.LabelField(this.r, (i + 1).ToString());
					this.r.x += this.r.width;

					this.r.width = 90F;
					GUI.Label(this.r, this.Hierarchy.Client.sentPacketsHistoric[i].time);
					this.r.x += this.r.width;

					this.r.width = this.bodyRect.width - this.r.x;
					GUILayout.BeginArea(r);
					{
						GUILayout.BeginHorizontal();
						{
							this.Hierarchy.Client.sentPacketsHistoric[i].packet.OnGUI(this.Hierarchy);

							GUILayout.FlexibleSpace();

							if (Conf.DebugMode != Conf.DebugModes.None)
								this.DebugDrawResent(this.Hierarchy.Client.sentPacketsHistoric[i].packet);
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();

					this.r.y += this.r.height;

					if (this.r.y - this.scrollBatchPosition.y > this.bodyRect.height)
						break;
				}
			}
			GUI.EndScrollView();
		}

		private void	DebugDrawResent(Packet packet)
		{
			if (GUILayout.Button(LC.G("NGInspector_Resend"), GUILayout.Width(60F)) == true)
				this.Hierarchy.Client.AddPacket(packet);
		}

		public void	DrawBatch()
		{
			this.bodyRect.y = EditorGUIUtility.singleLineHeight; // Header
			this.bodyRect.width = this.position.width;
			this.bodyRect.height = this.position.height;
			this.viewRect.height = this.Hierarchy.Client.batchedPackets.Count * EditorGUIUtility.singleLineHeight;
			this.r.x = 0F;
			this.r.y = EditorGUIUtility.singleLineHeight;
			this.r.height = EditorGUIUtility.singleLineHeight;
			this.r.width = this.position.width;

			GUILayout.BeginArea(this.r);
			{
				EditorGUILayout.BeginHorizontal();
				{
					if (GUILayout.Button(LC.G("Execute")) == true)
						this.Hierarchy.Client.ExecuteBatch();

					if (GUILayout.Button(LC.G("Save"), GUILayout.MaxWidth(100F)) == true)
						PromptWindow.Start("Noname", this.PromptSaveBatch, null);
				}
				EditorGUILayout.EndHorizontal();
				this.bodyRect.y += EditorGUIUtility.singleLineHeight;
			}
			GUILayout.EndArea();

			if (this.Hierarchy.Client.batchNames.Length > 0)
			{
				this.r.y += this.r.height;
				GUILayout.BeginArea(this.r);
				{
					EditorGUILayout.BeginHorizontal();
					{
						this.selectedBatch = EditorGUILayout.Popup(this.selectedBatch, this.Hierarchy.Client.batchNames);
						if (GUILayout.Button(LC.G("Load"), GUILayout.MaxWidth(100F)) == true)
							this.Hierarchy.Client.LoadBatch(this.selectedBatch);
					}
					EditorGUILayout.EndHorizontal();
					this.bodyRect.y += EditorGUIUtility.singleLineHeight;
					this.bodyRect.height -= this.bodyRect.y;
				}
				GUILayout.EndArea();
			}

			this.scrollBatchPosition = GUI.BeginScrollView(this.bodyRect, this.scrollBatchPosition, this.viewRect);
			{
				if (this.viewRect.height >= this.bodyRect.height)
					this.r.width -= 16F;

				this.r.y = 0F;
				this.r.height = EditorGUIUtility.singleLineHeight;

				for (int i = 0; i < this.Hierarchy.Client.batchedPackets.Count; i++)
				{
					GUILayout.BeginArea(r);
					{
						GUILayout.BeginHorizontal();
						{
							this.Hierarchy.Client.batchedPackets[i].OnGUI(this.Hierarchy);

							if (GUILayout.Button("X", GUILayout.Width(20F)) == true)
							{
								this.Hierarchy.Client.batchedPackets.RemoveAt(i);
								return;
							}
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();

					this.r.y += this.r.height;
				}
			}
			GUI.EndScrollView();
		}

		private void	PromptSaveBatch(object data, string name)
		{
			if (this.Hierarchy != null &&
				this.Hierarchy.Client != null &&
				string.IsNullOrEmpty(name) == false)
			{
				this.Hierarchy.Client.SaveBatch(name);
			}
		}

		protected virtual void	ShowButton(Rect r)
		{
			this.isLock = GUI.Toggle(r, this.isLock, GUIContent.none, GeneralStyles.LockButton);
		}

		private void	ToggleLocker()
		{
			this.isLock = !this.isLock;
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			NGRemoteHierarchyWindow.AddTabMenus(menu);
			menu.AddItem(new GUIContent("Lock"), this.isLock, new GenericMenu.MenuFunction(this.ToggleLocker));
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGRemoteInspectorWindow.NormalTitle, Constants.WikiBaseURL + "#markdown-header-132-ng-remote-inspector");
		}
	}
}