using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGRemoteCameraWindow : NGRemoteWindow, IHasCustomMenu, IReplaySettings
	{
		private enum RaycastState
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

		private class ModulesPopup : PopupWindowContent
		{
			private readonly NGRemoteCameraWindow	window;

			public	ModulesPopup(NGRemoteCameraWindow window)
			{
				this.window = window;
			}

			public override Vector2	GetWindowSize()
			{
				float	height = 0F;

				for (int i = 0; i < this.window.modules.Count; i++)
				{
					height += 18F;
				}

				height += 18F; // TODO temp add Screenshot module

				return new Vector2(Mathf.Max(this.window.position.width * .5F, 380F), height);
			}

			public override void	OnGUI(Rect r)
			{
				for (int i = 0; i < this.window.modules.Count; i++)
				{
					bool	available = false;

					if (this.window.cameraConnected == true)
					{
						if (this.window.modules[i].moduleID != ScreenshotModule.ModuleID)
						{
							for (int j = 0; j < this.window.modulesAvailable.Length; j++)
							{
								if (this.window.modulesAvailable[j] == this.window.modules[i].moduleID)
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
						this.window.modules[i].active = GUILayout.Toggle(this.window.modules[i].active, this.window.modules[i].name, GeneralStyles.ToolbarButton);
						if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
							this.window.Hierarchy.Client.AddPacket(new ClientToggleModulePacket(this.window.modules[i].moduleID, this.window.modules[i].active));
					}
					EditorGUI.EndDisabledGroup();

					this.window.modules[i].OnGUIModule(this.window.Hierarchy);
				}
			}
		}

		private class OptionPopup : PopupWindowContent
		{
			private readonly NGRemoteCameraWindow	window;

			public	OptionPopup(NGRemoteCameraWindow window)
			{
				this.window = window;
			}

			public override Vector2	GetWindowSize()
			{
				return new Vector2(Mathf.Max(this.window.position.width * .5F, 380F), 335F);
			}

			public override void	OnGUI(Rect r)
			{
				if (this.window.toolbarStyle == null)
				{
					this.window.toolbarStyle = new GUIStyle("Toolbar");
					this.window.toolbarStyle.fixedHeight = 0F;
					this.window.toolbarStyle.stretchHeight = true;
				}

				EditorGUI.BeginChangeCheck();
				this.window.targetRefresh = EditorGUILayout.IntSlider(LC.G("NGCamera_TargetRefresh"), this.window.targetRefresh, NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax);
				if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
				{
					this.window.targetRefresh = Mathf.Clamp(this.window.targetRefresh, NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax);
					this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.TargetRefresh, SettingType.Integer, this.window.targetRefresh));
				}

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUILayout.LabelField(LC.G("NGCamera_CameraSettings"));
				}
				EditorGUILayout.EndHorizontal();

				++EditorGUI.indentLevel;
				EditorGUI.BeginDisabledGroup(this.window.Hierarchy.IsClientConnected());
				{
					EditorGUILayout.BeginVertical();
					{
						EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
						{
							using (LabelWidthRestorer.Get(95F))
							{
								this.window.width = EditorGUILayout.IntField(LC.G("NGCamera_Resolution") + " W", this.window.width);

								using (LabelWidthRestorer.Get(16F))
								{
									--EditorGUI.indentLevel;
									this.window.height = EditorGUILayout.IntField("H", this.window.height);
									++EditorGUI.indentLevel;
								}

								using (LabelWidthRestorer.Get(55F))
									this.window.depth = (RenderTextureDepth)EditorGUILayout.EnumPopup(LC.G("NGCamera_Depth"), this.window.depth);
							}
						}
						EditorGUILayout.EndHorizontal();

						EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
						{
							this.window.renderTextureFormat = (RenderTextureFormat)EditorGUILayout.EnumPopup(LC.G("NGCamera_RenderTextureFormat"), this.window.renderTextureFormat);
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUI.EndDisabledGroup();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.clearFlags = EditorGUILayout.IntPopup(LC.G("NGCamera_ClearFlags"), this.window.clearFlags, NGRemoteCameraWindow.ClearFlagsLabels, NGRemoteCameraWindow.ClearFlagsValues);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClearFlags, SettingType.Integer, this.window.clearFlags));
				}
				EditorGUILayout.EndHorizontal();
							
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.background = EditorGUILayout.ColorField(LC.G("NGCamera_Background"), this.window.background);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraBackground, SettingType.Color, this.window.background));
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					if (this.window.Hierarchy == null || this.window.cameraConnected == true && (this.window.Hierarchy.ready & HierarchyReady.Layers) != 0)
					{
						EditorGUI.BeginChangeCheck();
						this.window.cullingMask = EditorGUILayout.MaskField(LC.G("NGCamera_CullingMask"), this.window.cullingMask, this.window.Hierarchy.Layers);
						if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
							this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraCullingMask, SettingType.Integer, this.window.cullingMask));
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.projection = EditorGUILayout.Popup(LC.G("NGCamera_Projection"), this.window.projection, NGRemoteCameraWindow.ProjectionLabels);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraProjection, SettingType.Integer, this.window.projection));
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					if (this.window.projection == 0)
					{
						EditorGUI.BeginChangeCheck();
						this.window.fieldOfView = EditorGUILayout.Slider(LC.G("NGCamera_FieldOfView"), this.window.fieldOfView, 1F, 179F);
						if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
							this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraFieldOfView, SettingType.Single, this.window.fieldOfView));
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						this.window.size = EditorGUILayout.FloatField(LC.G("NGCamera_Size"), this.window.size);
						if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
							this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraSize, SettingType.Single, this.window.size));
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(this.window.toolbarStyle, GUILayout.Height(40F));
				{
					EditorGUILayout.LabelField(LC.G("NGCamera_ClippingPlane"), GUILayout.Width(EditorGUIUtility.labelWidth));

					EditorGUILayout.BeginVertical();
					{
						using (LabelWidthRestorer.Get(50F))
						{
							EditorGUI.BeginChangeCheck();
							this.window.clippingPlanesNear = EditorGUILayout.FloatField(LC.G("NGCamera_Near"), this.window.clippingPlanesNear);
							if (EditorGUI.EndChangeCheck() == true)
							{
								if (this.window.clippingPlanesNear < .01F)
									this.window.clippingPlanesNear = .01F;

								if (this.window.cameraConnected == true)
									this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClippingPlanesNear, SettingType.Single, this.window.clippingPlanesNear));

								if (this.window.clippingPlanesNear >= this.window.clippingPlanesFar)
								{
									this.window.clippingPlanesFar = this.window.clippingPlanesNear + .01F;

									if (this.window.cameraConnected == true)
										this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClippingPlanesFar, SettingType.Single, this.window.clippingPlanesFar));
								}
							}

							EditorGUI.BeginChangeCheck();
							this.window.clippingPlanesFar = EditorGUILayout.FloatField(LC.G("NGCamera_Far"), this.window.clippingPlanesFar);
							if (EditorGUI.EndChangeCheck())
							{
								if (this.window.clippingPlanesFar <= this.window.clippingPlanesNear)
									this.window.clippingPlanesFar = this.window.clippingPlanesNear + .01F;

								if (this.window.cameraConnected == true)
									this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClippingPlanesFar, SettingType.Single, this.window.clippingPlanesFar));
							}
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(this.window.toolbarStyle, GUILayout.Height(55F));
				{
					EditorGUI.BeginChangeCheck();
					this.window.viewportRect = EditorGUILayout.RectField(LC.G("NGCamera_ViewportRect"), this.window.viewportRect);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraViewportRect, SettingType.Rect, this.window.viewportRect));
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.cdepth = EditorGUILayout.FloatField(LC.G("NGCamera_Depth"), this.window.cdepth);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
					{
						this.window.cdepth = Mathf.Clamp(this.window.cdepth, -100F, 100F);
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraDepth, SettingType.Single, this.window.cdepth));
					}
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.renderingPath = EditorGUILayout.IntPopup(LC.G("NGCamera_RenderingPath"), this.window.renderingPath, NGRemoteCameraWindow.RenderingPathLabels, NGRemoteCameraWindow.RenderingPathValues);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraRenderingPath, SettingType.Integer, this.window.renderingPath));
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.occlusionCulling = EditorGUILayout.Toggle(LC.G("NGCamera_OcclusionCulling"), this.window.occlusionCulling);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraOcclusionCulling, SettingType.Boolean, this.window.occlusionCulling));
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.HDR = EditorGUILayout.Toggle(LC.G("NGCamera_HDR"), this.window.HDR);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraHDR, SettingType.Boolean, this.window.HDR));
				}
				EditorGUILayout.EndHorizontal();

#if UNITY_5
				EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
				{
					EditorGUI.BeginChangeCheck();
					this.window.targetDisplay = EditorGUILayout.Popup(LC.G("NGCamera_TargetDisplay"), this.window.targetDisplay, NGRemoteCameraWindow.DisplayLabels);
					if (EditorGUI.EndChangeCheck() == true && this.window.cameraConnected == true)
						this.window.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraTargetDisplay, SettingType.Integer, this.window.targetDisplay));
				}
				EditorGUILayout.EndHorizontal();
#endif

				--EditorGUI.indentLevel;
			}
		}

		public const string				NormalTitle = "ƝƓ Ʀemote Ҁamera";
		public const string				ShortTitle = "ƝƓ Ʀ Ҁamera";
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

		public bool		displayGhostPanel = false;
		public bool		displayOverlay = false;

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

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetCursorPos(int X, int Y);

		public bool	keepCursorCenter = true;

		static	NGRemoteCameraWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGRemoteCameraWindow.NormalTitle + "	[BETA]");
		}

		[MenuItem(Constants.MenuItemPath + NGRemoteCameraWindow.NormalTitle + "	[BETA]", priority = Constants.MenuItemPriority + 220)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGRemoteCameraWindow>(NGRemoteCameraWindow.ShortTitle);
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

			this.modules.Sort((a, b) => b.priority - a.priority);
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

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
			if (Application.platform == RuntimePlatform.WindowsEditor && this.keepCursorCenter == true)
				Cursor.visible = true;
#endif

			this.Repaint();
		}

		protected override void	OnGUIHeader()
		{
			bool	clientConnected = this.Hierarchy.IsClientConnected();

			EditorGUILayout.BeginHorizontal(GeneralStyles.Toolbar);
			{
				if (GUILayout.Button("☰", "GV Gizmo DropDown") == true)
					PopupWindow.Show(new Rect(0F, 16F, 0F, 0F), new OptionPopup(this));

				if (GUILayout.Button(LC.G("NGCamera_Modules"), "GV Gizmo DropDown") == true)
					PopupWindow.Show(new Rect(32F, 16F, 0F, 0F), new ModulesPopup(this));

				Utility.content.text = LC.G("NGCamera_RecordDuration");
				using (LabelWidthRestorer.Get(GUI.skin.label.CalcSize(Utility.content).x))
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					this.recordLastSeconds = EditorGUILayout.FloatField(Utility.content, this.recordLastSeconds);
#else
					this.recordLastSeconds = EditorGUILayout.DelayedFloatField(Utility.content, this.recordLastSeconds);
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
						this.Hierarchy.Client.AddPacket(new ClientDisconnectPacket());
					}
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		protected override void	OnGUIConnected()
		{
			if (this.cameraConnected == false)
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
						this.Hierarchy.PickupResource(typeof(Transform), string.Empty, this.StickGhostCamera, this.stickyTransformID);
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
				this.modules[i].OnGUICamera(this, this.textureRect);

			if (this.ghostCamFocused == true)
				this.DrawCameraGUI();

			if (this.raycastState == RaycastState.RequestingRaycast)
			{
				Rect	r = this.textureRect;

				r.width = 250F;
				r.height = 16F;

				EditorGUI.DrawRect(r, NGRemoteCameraWindow.PanelBackgroundColor);
				GUI.Label(r, "Raycasting...");
			}
			else if (this.raycastState == RaycastState.ResultReceived)
			{
				EditorGUI.DrawRect(this.raycastResultRect, NGRemoteCameraWindow.PanelBackgroundColor);

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
				this.Hierarchy.Client.AddPacket(new ClientSendCameraZoomPacket(-Event.current.delta.y * this.zoomSpeed));

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
				this.dragging = false;
			else if (Event.current.type == EventType.MouseDrag && this.dragging == true)
			{
				if (Event.current.button != 2 && (Event.current.alt == false || Event.current.control == false))
				{
					Vector2	delta = new Vector2((Event.current.mousePosition.y - this.dragPos.y) * this.yAxisSensitivity,
												(Event.current.mousePosition.x - this.dragPos.x) * this.xAxisSensitivity);
					this.camRotation = this.dragCamRotation + delta;

#if !UNITY_4_5
					if (Application.platform == RuntimePlatform.WindowsEditor &&
						this.keepCursorCenter == true)
					{
						this.dragPos.x = (int)(this.textureRect.x + this.textureRect.width / 2);
						this.dragPos.y = (int)(this.textureRect.y + this.textureRect.height / 2);
						NGRemoteCameraWindow.SetCursorPos((int)(this.position.x + this.textureRect.x + this.textureRect.width / 2),
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

#if !UNITY_4_5
					if (this.keepCursorCenter == true)
					{
						this.dragPos.x = (int)(this.textureRect.x + this.textureRect.width / 2);
						this.dragPos.y = (int)(this.textureRect.y + this.textureRect.height / 2);
						NGRemoteCameraWindow.SetCursorPos((int)(this.position.x + this.textureRect.x + this.textureRect.width / 2),
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

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
					if (Application.platform == RuntimePlatform.WindowsEditor && this.keepCursorCenter == true)
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

#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7
				if (Application.platform == RuntimePlatform.WindowsEditor && this.keepCursorCenter == true)
					Cursor.visible = true;
#endif
			}

			if (this.displayOverlay == true)
			{
				this.overlayInputRect.x = 100F;
				this.overlayInputRect.y = this.textureRect.yMax - 160F;

				if (this.displayGhostPanel == true)
					this.overlayInputRect.y -= this.panelRect.height;

				Color	pressedColor = Event.current.control == true ? NGRemoteCameraWindow.SpeedHighlightArrowColor : NGRemoteCameraWindow.HighlightArrowColor;

				if (this.moveForward == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGRemoteCameraWindow.DefaultArrowColor);

				this.overlayInputRect.y += this.overlayInputRect.height + 4F;
				if (this.moveBackward == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGRemoteCameraWindow.DefaultArrowColor);

				this.overlayInputRect.x -= this.overlayInputRect.width + 4F;
				if (this.moveLeft == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGRemoteCameraWindow.DefaultArrowColor);

				this.overlayInputRect.x += this.overlayInputRect.width + 4F + this.overlayInputRect.width + 4F;
				if (this.moveRight == true)
					Utility.DrawUnfillRect(this.overlayInputRect, pressedColor);
				else
					Utility.DrawUnfillRect(this.overlayInputRect, NGRemoteCameraWindow.DefaultArrowColor);
			}

			if (this.displayGhostPanel == true)
			{
				EditorGUI.DrawRect(this.panelRect, NGRemoteCameraWindow.PanelBackgroundColor);

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

#if !UNITY_4_5
									if (Application.platform == RuntimePlatform.WindowsEditor)
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
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClearFlags, SettingType.Integer, this.clearFlags));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraBackground, SettingType.Color, this.background));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraCullingMask, SettingType.Integer, this.cullingMask));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraProjection, SettingType.Integer, this.projection));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraFieldOfView, SettingType.Single, this.fieldOfView));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraSize, SettingType.Single, this.size));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClippingPlanesNear, SettingType.Single, this.clippingPlanesNear));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraClippingPlanesFar, SettingType.Single, this.clippingPlanesFar));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraViewportRect, SettingType.Rect, this.viewportRect));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraDepth, SettingType.Single, this.cdepth));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraRenderingPath, SettingType.Integer, this.renderingPath));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraOcclusionCulling, SettingType.Boolean, this.occlusionCulling));
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraHDR, SettingType.Boolean, this.HDR));
#if UNITY_5
			this.Hierarchy.Client.AddPacket(new ClientSetSettingPacket(Setting.CameraTargetDisplay, SettingType.Integer, this.targetDisplay));
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
			NGRemoteHierarchyWindow.AddTabMenus(menu);
			menu.AddSeparator("");
			Utility.AddNGMenuItems(menu, this, NGRemoteCameraWindow.NormalTitle, Constants.WikiBaseURL + "#markdown-header-134-ng-remote-camera");
		}
	}
}