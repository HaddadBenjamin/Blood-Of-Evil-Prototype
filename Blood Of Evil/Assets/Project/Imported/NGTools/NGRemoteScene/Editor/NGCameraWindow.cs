using NGTools;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGCameraWindow : NGRemoteWindow, IHasCustomMenu, IReplaySettings
	{
		public enum DisplayMode
		{
			Camera,
			Modules
		}

		public enum RaycastState
		{
			None,
			RequestingRaycast,
			ResultReceived
		}

		public enum RenderTextureDepth
		{
			NoDepth = 0,
			Depth16 = 16,
			Depth24 = 24,
		}

		public const string				NormalTitle = "NG Remote Camera";
		public const string				ShortTitle = "NG R Camera";
		public readonly static Color	DefaultArrowColor = Color.white;
		public readonly static Color	HighlightArrowColor = Color.black;
		public readonly static Color	SpeedHighlightArrowColor = Color.yellow;
		public readonly static Color	PanelBackgroundColor = Color.gray * .8F;
		public readonly static int[]	ClearFlagsValues = new int[] { 1, 2, 3, 4 };
		public readonly static string[]	ClearFlagsLabels = new string[] { "Skybox", "Solid Color", "Depth only", "Don't Clear" };
		public readonly static string[]	ProjectionLabels = new string[] { "Perspective", "Orthographic" };
		public readonly static int[]	RenderingPathValues = new int[] { -1, 1, 3, 0, 2 };
		public readonly static string[]	RenderingPathLabels = new string[] { "Use Player Settings", "Forward", "Deferred", "Legacy Vertex Lit", "Legacy Deferred (light prepass)" };
		public readonly static string[]	DisplayLabels = new string[] { "Display 1", "Display 2", "Display 3", "Display 4", "Display 5", "Display 6", "Display 7", "Display 8" };

		public int					width = 800;
		public int					height = 600;
		public RenderTextureDepth	depth = RenderTextureDepth.Depth24;
		public int					targetRefresh = 24;
		public RenderTextureFormat	renderTextureFormat = RenderTextureFormat.ARGB32;

		public float	xAxisSensitivity = 1F;
		public float	yAxisSensitivity = 1F;
		public float	moveSpeed = 1F;
		public float	zoomSpeed = 1F;

		public int		clearFlags = 1;
		public Color	background = Color.black;
		public int		cullingMask = -1;
		public int		projection = 0;
		public float	fieldOfView = 30F;
		public float	size = 5F;
		public float	clippingPlanesNear = 1F;
		public float	clippingPlanesFar = 1000F;
		public Rect		viewportRect = new Rect(0F, 0F, 1F, 1F);
		public float	cdepth = -1F;
		public int		renderingPath = -1;
		public bool		occlusionCulling = true;
		public bool		HDR = false;
		public int		targetDisplay = 0;

		public float	recordLastSeconds = 30F;

		public DisplayMode	displayMode = DisplayMode.Camera;
		public bool			showOptions;
		public bool			displayGhostPanel = false;
		public bool			displayOverlay = false;
		public bool			displayCameraSettings = true;

		private int		cursorCamera;

		private bool		cameraConnected;

		[NonSerialized]
		private byte[]		modulesAvailable;
		private int[]		cameraIDs;
		private string[]	cameraNames;
		private int			ghostCameraID;
		private int			remoteFPS;
		private int			receivedTexturesCounter;
		private double		nextFPStime;

		private double	lastRaycastClick;
		private Vector2	lastRaycastMousePosition;
		private bool	hasDoubleRaycast;

		private RaycastState	raycastState;
		private Rect			raycastResultRect;
		private int[]			raycastResultIDs;
		private string[]		raycastResultNames;

		private bool	moveForward;
		private bool	moveBackward;
		private bool	moveLeft;
		private bool	moveRight;
		private Vector3	camPosition;
		private Vector2	camRotation;

		private bool	ghostCamFocused = false;
		private bool	hasWindowFocus = false;
		private Vector2	dragPos;
		private Vector3	dragCamPosition;
		private Vector2	dragCamRotation;
		private bool	dragging = false;

		private Rect	textureRect;
		private Rect	panelRect;
		private Rect	togglePanelRect;
		private Rect	overlayInputRect;

		[NonSerialized]
		private GUIStyle	toolbarStyle;

		public float	RecordLastSeconds { get { return this.recordLastSeconds; } }
		public int		TextureWidth { get { return this.width; } }
		public int		TextureHeight { get { return this.height; } }

		[NonSerialized]
		private List<CameraDataModuleEditor>	modules;
		public List<CameraDataModuleEditor>		Modules {
			get
			{
				return this.modules;
			}
		}

		[NonSerialized]
		public CameraDataModuleEditor	textureModule;

		private int	stickyTransformID;

#if !UNITY_4_5 && UNITY_EDITOR_WIN
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetCursorPos(int X, int Y);

		public bool	keepCursorCenter = true;
#endif

		static	NGCameraWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGCameraWindow.NormalTitle);
		}

		[MenuItem(Constants.MenuItemPath + NGCameraWindow.NormalTitle, priority = Constants.MenuItemPriority + 220)]
		private static void	Open()
		{
			EditorWindow.GetWindow<NGCameraWindow>(NGCameraWindow.ShortTitle);
		}

		protected virtual void	OnEnable()
		{
			Utility.LoadEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());

			this.panelRect = new Rect(32F, 0F, 0F, 76F);
			this.togglePanelRect = new Rect(0F, 0F, 32F, 32F);
			this.overlayInputRect = new Rect(100F, 0F, 60F, 60F);

			// Initialize modules.
			this.modules = new List<CameraDataModuleEditor>();

			foreach (Type module in Utility.EachAllSubClassesOf(typeof(CameraDataModuleEditor)))
			{
				CameraDataModuleEditor	instance = Activator.CreateInstance(module) as CameraDataModuleEditor;
				this.modules.Add(instance);

				if (instance.moduleID == ScreenshotModule.ModuleID)
					this.textureModule = instance;
			}

			this.modules.Sort((a, b) => b.priority - b.priority);
		}

		protected override void	OnDisable()
		{
			base.OnDisable();

			this.stickyTransformID = 0;
			this.cameraConnected = false;

			Utility.SaveEditorPref(this, NGEditorPrefs.GetPerProjectPrefix());
		}

		protected override void	OnHierarchyInit()
		{
			base.OnHierarchyInit();

			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerIsInitialized, this.Handle_Scene_ServerIsInitialized);
			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerSendAllCameras, this.Handle_Scene_ServerSendAllCameras);
			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerSendCameraTransform, this.Handle_Scene_ServerSendCameraTransform);
			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerSendRaycastResult, this.Handle_Scene_ServerSendRaycastResult);
			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerSendCameraData, this.Handle_Scene_ServerSendCameraData);
			this.Hierarchy.executer.HandlePacket(PacketId.Camera_ServerStickGhostCamera, this.Handle_Scene_ServerStickGhostCamera);

			this.camPosition = Vector3.zero;
			this.camRotation = Vector2.zero;
		}

		protected override void	OnHierarchyUninit()
		{
			base.OnHierarchyUninit();

			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerIsInitialized);
			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerSendAllCameras);
			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerSendCameraTransform);
			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerSendRaycastResult);
			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerSendCameraData);
			this.Hierarchy.executer.UnhandlePacket(PacketId.Camera_ServerStickGhostCamera);
		}

		protected override void	OnHierarchyConnected()
		{
			base.OnHierarchyConnected();

			this.cameraIDs = null;
			this.cameraNames = null;

			this.raycastState = RaycastState.None;
		}

		protected override void	OnHierarchyDisconnected()
		{
			base.OnHierarchyDisconnected();

			this.stickyTransformID = 0;
			this.cameraConnected = false;
		}

		protected virtual void	OnFocus()
		{
			this.hasWindowFocus = true;
		}

		protected virtual void	OnLostFocus()
		{
			this.hasWindowFocus = false;

			if (this.Hierarchy == null || this.Hierarchy.Client == null || this.cameraConnected == false)
				return;

			this.moveForward = false;
			this.moveBackward = false;
			this.moveLeft = false;
			this.moveRight = false;

			this.Hierarchy.Client.AddPacket(new ClientSendCameraInputPacket(false, false, false, false, 0F));

			this.wantsMouseMove = false;

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && UNITY_EDITOR_WIN
			if (this.keepCursorCenter == true)
				Cursor.visible = true;
#endif

			this.Repaint();
		}

		protected override void	OnGUIHeader()
		{
			bool	clientConnected = this.Hierarchy.IsClientConnected();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button(this.showOptions == true ? "˄" : "˅", GeneralStyles.ToolbarDropDown, GUILayout.Width(24F)) == true)
					this.showOptions = !this.showOptions;

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.displayMode == DisplayMode.Camera, LC.G("NGCamera_Camera"), GeneralStyles.ToolbarToggle);
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.displayMode = DisplayMode.Camera;
				}

				EditorGUI.BeginChangeCheck();
				GUILayout.Toggle(this.displayMode == DisplayMode.Modules, LC.G("NGCamera_Modules"), GeneralStyles.ToolbarToggle);
				if (EditorGUI.EndChangeCheck() == true)
				{
					this.displayMode = DisplayMode.Modules;
				}

#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				this.recordLastSeconds = EditorGUILayout.FloatField("Record duration", this.recordLastSeconds);
#else
				this.recordLastSeconds = EditorGUILayout.DelayedFloatField(LC.G("NGCamera_RecordDuration"), this.recordLastSeconds);
#endif

				EditorGUI.BeginDisabledGroup(clientConnected == false || this.cameraConnected == false);
				{
					if (GUILayout.Button(LC.G("NGCamera_ExportReplay"), GeneralStyles.ToolbarButton) == true)
					{
						NGReplayWindow	replayWindow = EditorWindow.GetWindow<NGReplayWindow>(NGReplayWindow.Title);

						replayWindow.AddReplay(new Replay(this));
					}
				}
				EditorGUI.EndDisabledGroup();

				GUILayout.FlexibleSpace();

				if (this.cameraConnected == false)
				{
					GUI.enabled = clientConnected;
					if (GUILayout.Button(LC.G("NGCamera_Connect"), GeneralStyles.ToolbarButton) == true && GUI.enabled == true)
					{
						if (this.Hierarchy.BlockRequestChannel(this.GetHashCode()) == true)
						{
							List<byte>	modulesAvailable = new List<byte>();
							for (int i = 0; i < this.modules.Count; i++)
								modulesAvailable.Add(this.modules[i].moduleID);

							this.Hierarchy.Client.AddPacket(new ClientConnectPacket(this.width, this.height, (int)this.depth, this.targetRefresh, this.renderTextureFormat, modulesAvailable.ToArray()));
						}
					}
					GUI.enabled = true;
				}
				else
				{
					GUILayout.Label(LC.G("NGCamera_FPS") + " " + this.remoteFPS);

					if (GUILayout.Button(LC.G("NGCamera_Disconnect")) == true)
					{
						this.cameraConnected = false;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			if (this.showOptions == true)
			{
				if (this.toolbarStyle == null)
				{
					this.toolbarStyle = new GUIStyle("Toolbar");
					this.toolbarStyle.fixedHeight = 0F;
					this.toolbarStyle.stretchHeight = true;
				}

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.targetRefresh = EditorGUILayout.IntSlider(LC.G("NGCamera_TargetRefresh"), this.targetRefresh, NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax);
					if (EditorGUI.EndChangeCheck() == true)
					{
						this.targetRefresh = Mathf.Clamp(this.targetRefresh, NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax);
						this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.TargetRefresh, ClientSetSettingPacket.SettingType.Integer, this.targetRefresh));
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					this.displayCameraSettings = EditorGUILayout.Foldout(this.displayCameraSettings, LC.G("NGCamera_CameraSettings"));
				}
				EditorGUILayout.EndHorizontal();

				if (this.displayCameraSettings == true)
				{
					++EditorGUI.indentLevel;
					EditorGUI.BeginDisabledGroup(clientConnected);
					{
						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
							{
								using (LabelWidthRestorer.Get(80F))
								{
									this.width = EditorGUILayout.IntField(LC.G("NGCamera_Resolution"), this.width);

									using (LabelWidthRestorer.Get(16F))
										this.height = EditorGUILayout.IntField("X", this.height);

									this.depth = (RenderTextureDepth)EditorGUILayout.EnumPopup(LC.G("NGCamera_Depth"), this.depth);
								}
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
							{
								this.renderTextureFormat = (RenderTextureFormat)EditorGUILayout.EnumPopup(LC.G("NGCamera_RenderTextureFormat"), this.renderTextureFormat);
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUI.EndDisabledGroup();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.clearFlags = EditorGUILayout.IntPopup(LC.G("NGCamera_ClearFlags"), this.clearFlags, NGCameraWindow.ClearFlagsLabels, NGCameraWindow.ClearFlagsValues);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClearFlags, ClientSetSettingPacket.SettingType.Integer, this.clearFlags));
					}
					EditorGUILayout.EndHorizontal();
							
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.background = EditorGUILayout.ColorField(LC.G("NGCamera_Background"), this.background);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraBackground, ClientSetSettingPacket.SettingType.Color, this.background));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						if (this.Hierarchy == null || this.cameraConnected == true && (this.Hierarchy.ready & HierarchyReady.Layers) != 0)
						{
							EditorGUI.BeginChangeCheck();
							this.cullingMask = EditorGUILayout.MaskField(LC.G("NGCamera_CullingMask"), this.cullingMask, this.Hierarchy.Layers);
							if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
								this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraCullingMask, ClientSetSettingPacket.SettingType.Integer, this.cullingMask));
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.projection = EditorGUILayout.Popup(LC.G("NGCamera_Projection"), this.projection, NGCameraWindow.ProjectionLabels);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraProjection, ClientSetSettingPacket.SettingType.Integer, this.projection));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						if (this.projection == 0)
						{
							EditorGUI.BeginChangeCheck();
							this.fieldOfView = EditorGUILayout.Slider(LC.G("NGCamera_FieldOfView"), this.fieldOfView, 1F, 179F);
							if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
								this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraFieldOfView, ClientSetSettingPacket.SettingType.Single, this.fieldOfView));
						}
						else
						{
							EditorGUI.BeginChangeCheck();
							this.size = EditorGUILayout.FloatField(LC.G("NGCamera_Size"), this.size);
							if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
								this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraSize, ClientSetSettingPacket.SettingType.Single, this.size));
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(toolbarStyle, GUILayout.Height(40F));
					{
						EditorGUILayout.LabelField(LC.G("NGCamera_ClippingPlane"), GUILayout.Width(EditorGUIUtility.labelWidth));

						EditorGUILayout.BeginVertical();
						{
							using (LabelWidthRestorer.Get(50F))
							{
								EditorGUI.BeginChangeCheck();
								this.clippingPlanesNear = EditorGUILayout.FloatField(LC.G("NGCamera_Near"), this.clippingPlanesNear);
								if (EditorGUI.EndChangeCheck() == true)
								{
									if (this.clippingPlanesNear < .01F)
										this.clippingPlanesNear = .01F;

									if (this.cameraConnected == true)
										this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClippingPlanesNear, ClientSetSettingPacket.SettingType.Single, this.clippingPlanesNear));

									if (this.clippingPlanesNear >= this.clippingPlanesFar)
									{
										this.clippingPlanesFar = this.clippingPlanesNear + .01F;

										if (this.cameraConnected == true)
											this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClippingPlanesFar, ClientSetSettingPacket.SettingType.Single, this.clippingPlanesFar));
									}
								}

								EditorGUI.BeginChangeCheck();
								this.clippingPlanesFar = EditorGUILayout.FloatField(LC.G("NGCamera_Far"), this.clippingPlanesFar);
								if (EditorGUI.EndChangeCheck())
								{
									if (this.clippingPlanesFar <= this.clippingPlanesNear)
										this.clippingPlanesFar = this.clippingPlanesNear + .01F;

									if (this.cameraConnected == true)
										this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClippingPlanesFar, ClientSetSettingPacket.SettingType.Single, this.clippingPlanesFar));
								}
							}
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(toolbarStyle, GUILayout.Height(55F));
					{
						EditorGUI.BeginChangeCheck();
						this.viewportRect = EditorGUILayout.RectField(LC.G("NGCamera_ViewportRect"), this.viewportRect);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraViewportRect, ClientSetSettingPacket.SettingType.Rect, this.viewportRect));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.cdepth = EditorGUILayout.FloatField(LC.G("NGCamera_Depth"), this.cdepth);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
						{
							this.cdepth = Mathf.Clamp(this.cdepth, -100F, 100F);
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraDepth, ClientSetSettingPacket.SettingType.Single, this.cdepth));
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.renderingPath = EditorGUILayout.IntPopup(LC.G("NGCamera_RenderingPath"), this.renderingPath, NGCameraWindow.RenderingPathLabels, NGCameraWindow.RenderingPathValues);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraRenderingPath, ClientSetSettingPacket.SettingType.Integer, this.renderingPath));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.occlusionCulling = EditorGUILayout.Toggle(LC.G("NGCamera_OcclusionCulling"), this.occlusionCulling);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraOcclusionCulling, ClientSetSettingPacket.SettingType.Boolean, this.occlusionCulling));
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.HDR = EditorGUILayout.Toggle(LC.G("NGCamera_HDR"), this.HDR);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraHDR, ClientSetSettingPacket.SettingType.Boolean, this.HDR));
					}
					EditorGUILayout.EndHorizontal();

#if UNITY_5
					EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
					{
						EditorGUI.BeginChangeCheck();
						this.targetDisplay = EditorGUILayout.Popup(LC.G("NGCamera_TargetDisplay"), this.targetDisplay, NGCameraWindow.DisplayLabels);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraTargetDisplay, ClientSetSettingPacket.SettingType.Integer, this.targetDisplay));
					}
					EditorGUILayout.EndHorizontal();
#endif

					--EditorGUI.indentLevel;
				}
			}

			// The way to display GUI is ugly, but whatever...
			if (this.displayMode == DisplayMode.Modules)
			{
				for (int i = 0; i < this.modules.Count; i++)
				{
					bool	available = false;

					if (this.cameraConnected == true)
					{
						if (this.modules[i].moduleID != ScreenshotModule.ModuleID)
						{
							for (int j = 0; j < this.modulesAvailable.Length; j++)
							{
								if (this.modulesAvailable[j] == this.modules[i].moduleID)
								{
									available = true;
									break;
								}
							}
						}
					}
					else
						available = true;

					EditorGUI.BeginDisabledGroup(!available);
					{
						EditorGUI.BeginChangeCheck();
						this.modules[i].active = GUILayout.Toggle(this.modules[i].active, this.modules[i].name, GeneralStyles.ToolbarButton);
						if (EditorGUI.EndChangeCheck() == true && this.cameraConnected == true)
							this.Hierarchy.Client.AddPacket(new ClientToggleModulePacket(this.modules[i].moduleID, this.modules[i].active));
					}
					EditorGUI.EndDisabledGroup();

					this.modules[i].OnGUIModule(this.Hierarchy);
				}
			}
		}

		protected override void	OnGUIConnected()
		{
			if (this.cameraConnected == false || this.displayMode == DisplayMode.Modules)
				return;

			EditorGUILayout.BeginHorizontal();
			{
				if (this.cameraIDs != null)
				{
					using (LabelWidthRestorer.Get(70F))
					{
						EditorGUI.BeginChangeCheck();
						this.cursorCamera = EditorGUILayout.Popup(LC.G("NGCamera_Cameras"), this.cursorCamera, this.cameraNames);
						if (EditorGUI.EndChangeCheck() == true)
						{
							this.ghostCamFocused = this.ghostCameraID == this.cameraIDs[this.cursorCamera];
							this.Hierarchy.Client.AddPacket(new ClientPickCameraPacket(this.cameraIDs[this.cursorCamera]));
						}
					}

					EditorGUI.BeginDisabledGroup(this.ghostCameraID == this.cameraIDs[this.cursorCamera]);
					{
						if (GUILayout.Button(LC.G("NGCamera_PickGhostCameraAtCamera")) == true)
						{
							this.Hierarchy.Client.AddPacket(new ClientPickGhostCameraAtCameraPacket(this.cameraIDs[this.cursorCamera]));

							for (int i = 0; i < this.cameraIDs.Length; i++)
							{
								if (this.cameraIDs[i] == this.ghostCameraID)
								{
									this.cursorCamera = i;
									this.ghostCamFocused = true;
									break;
								}
							}
						}

						if (GUILayout.Button(LC.G("NGCamera_PickGhostCamera")) == true)
						{
							for (int i = 0; i < this.cameraIDs.Length; i++)
							{
								if (this.cameraIDs[i] == this.ghostCameraID)
								{
									this.cursorCamera = i;
									this.ghostCamFocused = true;
									this.Hierarchy.Client.AddPacket(new ClientPickCameraPacket(this.ghostCameraID));
									break;
								}
							}
						}
					}
					EditorGUI.EndDisabledGroup();

					GUILayout.Label(" | " + LC.G("NGCamera_Anchor"));

					if (this.stickyTransformID == 0)
						GUILayout.Label(LC.G("NGCamera_NoStickyTransform"));
					else
						GUILayout.Label(this.Hierarchy.GetResourceName(typeof(Transform), this.stickyTransformID));

					if (GUILayout.Button(LC.G("NGCamera_OpenPickerTransform")) == true)
					{
						this.Hierarchy.PickupResource(typeof(Transform), string.Empty, this.StickGhostCamera, this.stickyTransformID);
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			this.textureRect = GUILayoutUtility.GetLastRect();
			this.textureRect.x = 0F;
			this.textureRect.width = this.position.width;
			this.textureRect.y += this.textureRect.height;
			this.textureRect.height = this.position.height - this.textureRect.y - 1F;

			// This has to be executed before DrawCameraGUI, because raycastResultRect is used inside.
			if (this.raycastState == RaycastState.ResultReceived)
			{
				if (this.hasDoubleRaycast == true)
				{
					this.hasDoubleRaycast = false;
					this.raycastState = RaycastState.None;

					if (this.raycastResultIDs.Length > 0)
						this.Hierarchy.SelectGameObject(this.raycastResultIDs[0]);
				}
				else
				{
					this.raycastResultRect.x = this.textureRect.x;
					this.raycastResultRect.y = this.textureRect.y;
					this.raycastResultRect.width = 250F;
					this.raycastResultRect.height = 16F * (1 + this.raycastResultNames.Length);
				}
			}

			for (int i = 0; i < this.modules.Count; i++)
			{
				this.modules[i].OnGUI(this, this.textureRect);
			}

			if (this.ghostCamFocused == true)
				this.DrawCameraGUI();

			if (this.raycastState == RaycastState.RequestingRaycast)
			{
				Rect	r = this.textureRect;

				r.width = 250F;
				r.height = 16F;

				EditorGUI.DrawRect(r, NGCameraWindow.PanelBackgroundColor);
				GUI.Label(r, "Raycasting...");
			}
			else if (this.raycastState == RaycastState.ResultReceived)
			{
				EditorGUI.DrawRect(this.raycastResultRect, NGCameraWindow.PanelBackgroundColor);

				Rect	r = this.raycastResultRect;

				r.width -= 16F;
				r.height = 16F;
				GUI.Label(r, "Raycast result: " + this.raycastResultNames.Length + " hit(s)");

				r.x += r.width;
				r.width = 16F;
				if (GUI.Button(r, "X") == true)
					this.raycastState = RaycastState.None;

				r.y += r.height;
				r.x = this.raycastResultRect.x;
				r.width = this.raycastResultRect.width;

				for (int i = 0; i < this.raycastResultNames.Length; i++)
				{
					if (GUI.Button(r, this.raycastResultNames[i]) == true)
						this.Hierarchy.SelectGameObject(this.raycastResultIDs[i]);

					r.y += r.height;
				}
			}
		}

		protected virtual void	Update()
		{
			if (EditorApplication.timeSinceStartup >= this.nextFPStime)
			{
				this.remoteFPS = this.receivedTexturesCounter;
				this.receivedTexturesCounter = 0;
				this.nextFPStime = EditorApplication.timeSinceStartup + 1D;
			}
		}

		private void	DrawCameraGUI()
		{
			if (Event.current.type == EventType.KeyDown)
			{
				bool	anyUpdate = true;

				if (Event.current.keyCode == KeyCode.UpArrow)
					this.moveForward = true;
				else if (Event.current.keyCode == KeyCode.DownArrow)
					this.moveBackward = true;
				else if (Event.current.keyCode == KeyCode.LeftArrow)
					this.moveLeft = true;
				else if (Event.current.keyCode == KeyCode.RightArrow)
					this.moveRight = true;
				else if (Event.current.keyCode == KeyCode.RightControl)
				{}
				else
					anyUpdate = false;

				if (anyUpdate == true)
				{
					float	moveSpeed = this.moveSpeed * (Event.current.control == true ? 2F : 1F);

					this.Hierarchy.Client.AddPacket(new ClientSendCameraInputPacket(this.moveForward, this.moveBackward, this.moveLeft, this.moveRight, moveSpeed));
				}
			}
			else if (Event.current.type == EventType.KeyUp)
			{
				bool	anyUpdate = true;

				if (Event.current.keyCode == KeyCode.UpArrow)
					this.moveForward = false;
				else if (Event.current.keyCode == KeyCode.DownArrow)
					this.moveBackward = false;
				else if (Event.current.keyCode == KeyCode.LeftArrow)
					this.moveLeft = false;
				else if (Event.current.keyCode == KeyCode.RightArrow)
					this.moveRight = false;
				else if (Event.current.keyCode == KeyCode.RightControl)
				{}
				else
					anyUpdate = false;

				if (anyUpdate == true)
				{
					float	moveSpeed = this.moveSpeed * (Event.current.control == true ? 2F : 1F);

					this.Hierarchy.Client.AddPacket(new ClientSendCameraInputPacket(this.moveForward, this.moveBackward, this.moveLeft, this.moveRight, moveSpeed));
				}
			}
			else if (Event.current.type == EventType.ScrollWheel && this.textureRect.Contains(Event.current.mousePosition) == true)
			{
				this.Hierarchy.Client.AddPacket(new ClientSendCameraZoomPacket(-Event.current.delta.y * this.zoomSpeed));
			}

			if (this.hasWindowFocus == true && this.dragging == true)
			{
				this.textureRect.x += 1F;
				this.textureRect.y += 1F;
				this.textureRect.width -= 2F;
				this.textureRect.height -= 2F;
				Utility.DrawUnfillRect(this.textureRect, Color.white);

				this.wantsMouseMove = true;

				this.Repaint();
			}

			this.panelRect.y = this.position.height - this.panelRect.height;
			this.panelRect.width = this.position.width - 32F;
			this.togglePanelRect.y = this.position.height - this.togglePanelRect.height;

			if (Event.current.type == EventType.MouseMove && this.dragging == true)
			{
				this.dragging = false;
			}
			else if (Event.current.type == EventType.MouseDrag && this.dragging == true)
			{
				if (Event.current.button != 2 && (Event.current.alt == false || Event.current.control == false))
				{
					Vector2	delta = new Vector2((Event.current.mousePosition.y - this.dragPos.y) * this.yAxisSensitivity,
												(Event.current.mousePosition.x - this.dragPos.x) * this.xAxisSensitivity);
					this.camRotation = this.dragCamRotation + delta;

#if !UNITY_4_5 && UNITY_EDITOR_WIN
					if (this.keepCursorCenter == true)
					{
						this.dragPos.x = (int)(this.textureRect.x + this.textureRect.width / 2);
						this.dragPos.y = (int)(this.textureRect.y + this.textureRect.height / 2);
						NGCameraWindow.SetCursorPos((int)(this.position.x + this.textureRect.x + this.textureRect.width / 2),
													(int)(this.position.y + this.textureRect.y + this.textureRect.height / 2));
						this.dragCamRotation += delta;
					}
#endif
					this.Hierarchy.Client.AddPacket(new ClientSendCameraTransformRotationPacket(this.camRotation));
				}
				else
				{
					Vector3	up = Quaternion.Euler(this.camRotation.x, this.camRotation.y, 0F) * Vector3.up;
					// The 2 next lines are equal, but one does not allocate Vector3. Don't know why, but could'nt reproduce the same for up vector, maybe because of PitchRollYaw.
					//Vector3	right = Quaternion.Euler(this.camRotation.x, this.camRotation.y, 0F) * Vector3.right;
					Vector3	right = Quaternion.AngleAxis(this.camRotation.y, Vector3.up) * Vector3.right;
					Vector3	direction = (up * (Event.current.mousePosition.y - this.dragPos.y) * .01F + right * (Event.current.mousePosition.x - this.dragPos.x) * -.01F);
					this.camPosition = this.dragCamPosition + direction;

#if !UNITY_4_5 && UNITY_EDITOR_WIN
					if (this.keepCursorCenter == true)
					{
						this.dragPos.x = (int)(this.textureRect.x + this.textureRect.width / 2);
						this.dragPos.y = (int)(this.textureRect.y + this.textureRect.height / 2);
						NGCameraWindow.SetCursorPos((int)(this.position.x + this.textureRect.x + this.textureRect.width / 2),
													(int)(this.position.y + this.textureRect.y + this.textureRect.height / 2));
						this.dragCamPosition += direction;
					}
#endif
					this.Hierarchy.Client.AddPacket(new ClientSendCameraTransformPositionPacket(this.camPosition));
				}
			}
			else if (Event.current.type == EventType.MouseDown &&
					 this.textureRect.Contains(Event.current.mousePosition) == true &&
					 (this.displayGhostPanel == false || this.panelRect.Contains(Event.current.mousePosition) == false) &&
					 (this.raycastState == RaycastState.None || this.raycastResultRect.Contains(Event.current.mousePosition) == false) &&
					 this.togglePanelRect.Contains(Event.current.mousePosition) == false)
			{
				if (Event.current.button == 1 || Event.current.alt == true)
				{
					this.dragCamPosition = this.camPosition;
					this.dragCamRotation = this.camRotation;
					this.dragging = true;
					this.dragPos = Event.current.mousePosition;
					GUI.FocusControl(null);

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && UNITY_EDITOR_WIN
					if (this.keepCursorCenter == true)
						Cursor.visible = false;
#endif
				}
				else if (Event.current.button == 0)
				{
					if (this.lastRaycastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup &&
						Vector2.Distance(this.lastRaycastMousePosition, Event.current.mousePosition) < 5F)
					{
						this.hasDoubleRaycast = true;
					}
					else
					{
						float	viewportPositionX = (Event.current.mousePosition.x - this.textureRect.x) / this.textureRect.width;
						float	viewportPositionY = 1F - (Event.current.mousePosition.y - this.textureRect.y) / this.textureRect.height;
						float	tRatio = (float)this.width / (float)this.height;
						float	tRRatio = this.textureRect.width / this.textureRect.height;

						if (tRatio < tRRatio)
						{
							float	uncropHeight = (float)this.height * this.textureRect.width / (float)this.width;
							float	yMin = (float)(uncropHeight - this.textureRect.height) / 2F;
							viewportPositionY = (yMin + (viewportPositionY * this.textureRect.height)) / uncropHeight;
						}
						else
						{
							float	uncropWidth = (float)this.width * this.textureRect.height / (float)this.height;
							float	xMin = (float)(uncropWidth - this.textureRect.width) / 2F;
							viewportPositionX = (xMin + (viewportPositionX * this.textureRect.width)) / uncropWidth;
						}

						this.Hierarchy.Client.AddPacket(new ClientRaycastScenePacket(viewportPositionX, viewportPositionY));
						this.lastRaycastClick = EditorApplication.timeSinceStartup;
						this.lastRaycastMousePosition = Event.current.mousePosition;
						this.hasDoubleRaycast = Event.current.control;
						this.raycastState = RaycastState.RequestingRaycast;
					}
				}
				else
				{
					this.dragCamPosition = this.camPosition;
					this.dragging = true;
					this.dragPos = Event.current.mousePosition;
					GUI.FocusControl(null);
				}
			}
			else if (Event.current.type == EventType.MouseUp)
			{
				this.dragging = false;
#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && UNITY_EDITOR_WIN
				if (this.keepCursorCenter == true)
					Cursor.visible = true;
#endif
			}

			if (this.displayOverlay == true)
			{
				this.overlayInputRect.x = 100F;
				this.overlayInputRect.y = this.textureRect.yMax - 160F;

				if (this.displayGhostPanel == true)
					this.overlayInputRect.y -= this.panelRect.height;

				Color	pressedColor = Event.current.control == true ? NGCameraWindow.SpeedHighlightArrowColor : NGCameraWindow.HighlightArrowColor;

				if (this.moveForward == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGCameraWindow.DefaultArrowColor);

				this.overlayInputRect.y += this.overlayInputRect.height + 4F;
				if (this.moveBackward == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGCameraWindow.DefaultArrowColor);

				this.overlayInputRect.x -= this.overlayInputRect.width + 4F;
				if (this.moveLeft == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGCameraWindow.DefaultArrowColor);

				this.overlayInputRect.x += this.overlayInputRect.width + 4F + this.overlayInputRect.width + 4F;
				if (this.moveRight == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGCameraWindow.DefaultArrowColor);
			}

			if (this.displayGhostPanel == true)
			{
				EditorGUI.DrawRect(this.panelRect, NGCameraWindow.PanelBackgroundColor);

				GUILayout.BeginArea(this.panelRect);
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(3F);

						EditorGUILayout.BeginVertical(GUILayout.Width(this.panelRect.width * .5F));
						{
							GUILayout.Space(3F);

							using (LabelWidthRestorer.Get(90F))
							{
								EditorGUILayout.BeginHorizontal();
								{
									this.xAxisSensitivity = EditorGUILayout.FloatField(LC.G("NGCamera_MouseXSensitivity"), this.xAxisSensitivity);
									this.yAxisSensitivity = EditorGUILayout.FloatField(LC.G("NGCamera_MouseYSensitivity"), this.yAxisSensitivity);
								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.BeginHorizontal();
								{
									this.moveSpeed = EditorGUILayout.FloatField(LC.G("NGCamera_MoveSpeed"), this.moveSpeed);
									this.zoomSpeed = EditorGUILayout.FloatField(LC.G("NGCamera_ZoomSpeed"), this.zoomSpeed);
								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.BeginHorizontal();
								{
									this.displayOverlay = EditorGUILayout.Toggle("Display Overlay", this.displayOverlay);
#if !UNITY_4_5 && UNITY_EDITOR_WIN
									this.keepCursorCenter = EditorGUILayout.Toggle("Lock Cursor", this.keepCursorCenter);
#endif
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndVertical();

						EditorGUILayout.BeginVertical();
						{
							GUILayout.Space(3F);

							EditorGUI.BeginChangeCheck();
							this.camPosition = EditorGUILayout.Vector3Field(LC.G("NGCamera_Position"), this.camPosition);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.Client.AddPacket(new ClientSendCameraTransformPositionPacket(this.camPosition));

							EditorGUI.BeginChangeCheck();
							this.camRotation = EditorGUILayout.Vector2Field(LC.G("NGCamera_Rotation"), this.camRotation);
							if (EditorGUI.EndChangeCheck() == true)
								this.Hierarchy.Client.AddPacket(new ClientSendCameraTransformRotationPacket(this.camRotation));
						}
						EditorGUILayout.EndVertical();

						GUILayout.Space(3F);
					}
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
			}

			if (GUI.Button(this.togglePanelRect, this.displayGhostPanel == true ? "<" : ">") == true)
				this.displayGhostPanel = !this.displayGhostPanel;
		}

		private void	Handle_Scene_ServerIsInitialized(Client sender, Packet _packet)
		{
			ServerIsInitializedPacket	packet = _packet as ServerIsInitializedPacket;

			this.Hierarchy.Client.AddPacket(new ClientRequestAllCamerasPacket());
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClearFlags, ClientSetSettingPacket.SettingType.Integer, this.clearFlags));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraBackground, ClientSetSettingPacket.SettingType.Color, this.background));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraCullingMask, ClientSetSettingPacket.SettingType.Integer, this.cullingMask));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraProjection, ClientSetSettingPacket.SettingType.Integer, this.projection));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraFieldOfView, ClientSetSettingPacket.SettingType.Single, this.fieldOfView));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraSize, ClientSetSettingPacket.SettingType.Single, this.size));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClippingPlanesNear, ClientSetSettingPacket.SettingType.Single, this.clippingPlanesNear));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraClippingPlanesFar, ClientSetSettingPacket.SettingType.Single, this.clippingPlanesFar));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraViewportRect, ClientSetSettingPacket.SettingType.Rect, this.viewportRect));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraDepth, ClientSetSettingPacket.SettingType.Single, this.cdepth));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraRenderingPath, ClientSetSettingPacket.SettingType.Integer, this.renderingPath));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraOcclusionCulling, ClientSetSettingPacket.SettingType.Boolean, this.occlusionCulling));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraHDR, ClientSetSettingPacket.SettingType.Boolean, this.HDR));
#if UNITY_5
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(ClientSetSettingPacket.Settings.CameraTargetDisplay, ClientSetSettingPacket.SettingType.Integer, this.targetDisplay));
#endif

			for (int i = 0; i < this.modules.Count; i++)
				this.modules[i].OnServerInitialized(this, this.Hierarchy.Client);

			this.modulesAvailable = packet.modules;

			this.Hierarchy.UnblockRequestChannel(this.GetHashCode());

			this.cameraConnected = true;

			this.Repaint();
		}

		private void	Handle_Scene_ServerSendAllCameras(Client sender, Packet _packet)
		{
			ServerSendAllCamerasPacket	packet = _packet as ServerSendAllCamerasPacket;

			this.cameraIDs = packet.IDs;
			this.cameraNames = packet.names;
			this.ghostCameraID = packet.ghostCameraId;

			for (int i = 0; i < this.cameraIDs.Length; i++)
			{
				if (this.cameraIDs[i] == this.ghostCameraID)
				{
					this.cursorCamera = i;
					this.ghostCamFocused = true;
					break;
				}
			}

			this.Repaint();
		}

		private void	Handle_Scene_ServerSendCameraTransform(Client sender, Packet _packet)
		{
			ServerSendCameraTransformPacket	packet = _packet as ServerSendCameraTransformPacket;

			this.camPosition.x = packet.positionX;
			this.camPosition.y = packet.positionY;
			this.camPosition.z = packet.positionZ;
			this.camRotation.x = packet.rotationX;
			this.camRotation.y = packet.rotationY;
		}

		private void	Handle_Scene_ServerSendRaycastResult(Client sender, Packet _packet)
		{
			ServerSendRaycastResultPacket	packet = _packet as ServerSendRaycastResultPacket;

			this.raycastResultIDs = packet.instanceIDs;
			this.raycastResultNames = packet.names;
			this.raycastState = RaycastState.ResultReceived;
		}

		private void	Handle_Scene_ServerSendCameraData(Client sender, Packet _packet)
		{
			CameraDataPacket	packet = _packet as CameraDataPacket;

			if (ScreenshotModule.ModuleID == packet.moduleID)
				++this.receivedTexturesCounter;

			for (int i = 0; i < this.modules.Count; i++)
			{
				if (this.modules[i].moduleID == packet.moduleID &&
					this.modules[i].active == true)
				{
					this.modules[i].HandlePacket(this, packet.time, packet.data);
					this.Repaint();
					break;
				}
			}
		}

		private void	Handle_Scene_ServerStickGhostCamera(Client sender, Packet _packet)
		{
			ServerStickGhostCameraPacket	packet = _packet as ServerStickGhostCameraPacket;

			this.stickyTransformID = packet.ID;
		}

		private Packet	StickGhostCamera(string valuePath, byte[] data)
		{
			TypeHandler	handler = TypeHandlersManager.GetTypeHandler<UnityObject>();
			UnityObject	unityObject = handler.Deserialize(Utility.GetBBuffer(data), typeof(Transform)) as UnityObject;

			return new ClientStickGhostCameraPacket(unityObject.instanceID);
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			NGHierarchyWindow.AddTabMenus(menu);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGCameraWindow.NormalTitle, Constants.WikiBaseURL + "#markdown-header-134-ng-remote-camera");
		}
	}
}