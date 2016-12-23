using System;
using System.Collections;
using System.Collections.Generic;

namespace NGTools
{
	using UnityEngine;

	public partial class PacketId
	{
		public const int	ServerHasDisconnect = 1;
		public const int	ClientHasDisconnect = 2;
		public const int	Server_ErrorNotification = 3;

		public const int	Scene_ClientRequestHierarchy = 3000;
		public const int	Scene_ServerSendHierarchy = 3001;
		public const int	Scene_ClientRequestLayers = 3002;
		public const int	Scene_ServerSendLayers = 3003;

		public const int	Scene_ClientRequestResources = 3004;
		public const int	Scene_ServerSendResources = 3005;

		public const int	Scene_ClientSetSibling = 3008;

		public const int	Scene_ClientRequestGameObjectData = 3100;
		public const int	Scene_ServerSendGameObjectData = 3101;

		public const int	Scene_ServerSendComponent = 3102;

		public const int	Scene_ClientUpdateFieldValue = 3103;
		public const int	Scene_ServerUpdateFieldValue = 3104;

		public const int	Scene_ClientInvokeBehaviourMethod = 3105;

		public const int	Scene_ClientWatchGameObjects = 3106;
		public const int	Scene_ClientWatchMaterials = 3107;
		public const int	Scene_ClientDeleteGameObjects = 3108;
		public const int	Scene_ServerDeleteGameObjects = 3109;
		public const int	Scene_ClientDeleteComponents = 3110;
		public const int	Scene_ServerDeleteComponents = 3111;

		public const int	Scene_ClientRequestMaterialData = 3112;
		public const int	Scene_ServerSendMaterialData = 3113;
		public const int	Scene_ClientUpdateMaterialProperty = 3114;
		public const int	Scene_ServerUpdateMaterialProperty = 3115;
		public const int	Scene_ClientUpdateMaterialVector2 = 3116;
		public const int	Scene_ServerUpdateMaterialVector2 = 3117;
		public const int	Scene_ClientChangeMaterialShader = 3118;

		public const int	Scene_ClientRequestEnumData = 3119;
		public const int	Scene_ServerSendEnumData = 3120;

		public const int	Scene_ClientLoadBigArray = 3121;

		public const int	Scene_ClientRequestProject = 3200;
		public const int	Scene_ServerSendProject = 3201;

		public const int	Camera_ClientConnect = 3300;
		public const int	Camera_ClientDisconnect = 3317;
		public const int	Camera_ServerIsInitialized = 3301;
		public const int	Camera_ClientRequestAllCameras = 3302;
		public const int	Camera_ServerSendAllCameras = 3303;
		public const int	Camera_ClientPickCamera = 3304;
		public const int	Camera_ClientPickGhostCameraAtCamera = 3305;
		public const int	Camera_ClientStickGhostCamera = 3318;
		public const int	Camera_ServerStickGhostCamera = 3319;
		public const int	Camera_ClientSetSetting = 3306;
		public const int	Camera_ServerSendTexture = 3307;
		public const int	Camera_ServerSendCameraTransform = 3308;
		public const int	Camera_ClientSendCameraInput = 3309;
		public const int	Camera_ClientSendCameraTransformPosition = 3310;
		public const int	Camera_ClientSendCameraTransformRotation = 3311;
		public const int	Camera_ClientSendCameraZoom = 3312;

		public const int	Camera_ClientRaycastScene = 3313;
		public const int	Camera_ServerSendRaycastResult = 3314;
		
		public const int	Camera_ServerSendCameraData = 3315;

		public const int	Camera_ClientToggleModule = 3316;
	}

	public class ServerSceneExecuter : PacketExecuter
	{
		private NGServerScene		ssm;
		private List<GameObject>	cameraRaycastResult = new List<GameObject>();

		public	ServerSceneExecuter(NGServerScene ssm)
		{
			this.ssm = ssm;

			// NG Hierarchy
			this.HandlePacket(PacketId.ClientHasDisconnect, this.Handle_Scene_ClientIsDisconnected);

			this.HandlePacket(PacketId.Scene_ClientRequestHierarchy, this.Handle_Scene_ClientRequestHierarchy);
			this.HandlePacket(PacketId.Scene_ClientRequestLayers, this.Handle_Scene_ClientRequestLayers);
			this.HandlePacket(PacketId.Scene_ClientRequestResources, this.Handle_Scene_ClientRequestResources);
			this.HandlePacket(PacketId.Scene_ClientSetSibling, this.Handle_Scene_ClientSetSibling);

			// NG Inspector
			this.HandlePacket(PacketId.Scene_ClientRequestGameObjectData, this.Handle_Scene_ClientRequestGameObjectData);
			this.HandlePacket(PacketId.Scene_ClientUpdateFieldValue, this.Handle_Scene_ClientUpdateFieldValue);

			this.HandlePacket(PacketId.Scene_ClientInvokeBehaviourMethod, this.Handle_Scene_ClientInvokeBehaviourMethod);

			this.HandlePacket(PacketId.Scene_ClientWatchGameObjects, this.Handle_Scene_ClientWatchGameObjects);
			this.HandlePacket(PacketId.Scene_ClientWatchMaterials, this.Handle_Scene_ClientWatchMaterials);
			this.HandlePacket(PacketId.Scene_ClientDeleteGameObjects, this.Handle_Scene_ClientDeleteGameObjects);
			this.HandlePacket(PacketId.Scene_ClientDeleteComponents, this.Handle_Scene_ClientDeleteComponents);

			this.HandlePacket(PacketId.Scene_ClientRequestMaterialData, this.Handle_Scene_ClientRequestMaterialData);
			this.HandlePacket(PacketId.Scene_ClientUpdateMaterialProperty, this.Handle_Scene_ClientUpdateMaterialProperty);
			this.HandlePacket(PacketId.Scene_ClientUpdateMaterialVector2, this.Handle_Scene_ClientUpdateMaterialVector2);
			this.HandlePacket(PacketId.Scene_ClientChangeMaterialShader, this.Handle_Scene_ClientChangeMaterialShader);

			this.HandlePacket(PacketId.Scene_ClientRequestEnumData, this.Handle_Scene_ClientRequestEnumData);
			this.HandlePacket(PacketId.Scene_ClientLoadBigArray, this.Handle_Scene_ClientLoadBigArray);

			// NG Project
			this.HandlePacket(PacketId.Scene_ClientRequestProject, this.Handle_Scene_ClientRequestProject);

			// NG Camera
			this.HandlePacket(PacketId.Camera_ClientConnect, this.Handle_Camera_ClientConnect);
			this.HandlePacket(PacketId.Camera_ClientDisconnect, this.Handle_Camera_ClientDisconnect);
			this.HandlePacket(PacketId.Camera_ClientRequestAllCameras, this.Handle_Camera_ClientRequestAllCameras);
			this.HandlePacket(PacketId.Camera_ClientPickCamera, this.Handle_Camera_ClientPickCamera);
			this.HandlePacket(PacketId.Camera_ClientPickGhostCameraAtCamera, this.Handle_Camera_ClientPickGhostCameraAtCamera);
			this.HandlePacket(PacketId.Camera_ClientSetSetting, this.Handle_Camera_ClientSetSetting);
			this.HandlePacket(PacketId.Camera_ClientSendCameraInput, this.Handle_Camera_ClientSendCameraInput);
			this.HandlePacket(PacketId.Camera_ClientSendCameraTransformPosition, this.Handle_Camera_ClientSendCameraTransformPosition);
			this.HandlePacket(PacketId.Camera_ClientSendCameraTransformRotation, this.Handle_Camera_ClientSendCameraTransformRotation);
			this.HandlePacket(PacketId.Camera_ClientSendCameraZoom, this.Handle_Camera_ClientSendCameraZoom);
			this.HandlePacket(PacketId.Camera_ClientRaycastScene, this.Handle_Camera_ClientRaycastScene);
			this.HandlePacket(PacketId.Camera_ClientToggleModule, this.Handle_Camera_ClientToggleModule);
			this.HandlePacket(PacketId.Camera_ClientStickGhostCamera, this.Handle_Camera_ClientStickGhostCamera);
		}
		
		private void	Handle_Scene_ClientIsDisconnected(Client sender, Packet _packet)
		{
			// Clean data from client.

			NGServerCamera	cam;
			if (this.NGGhostCams.TryGetValue(sender, out cam) == true)
			{
				this.NGGhostCams.Remove(sender);
				Object.DestroyImmediate(cam.gameObject);
			}
		}

		private void	Handle_Scene_ClientRequestHierarchy(Client sender, Packet _packet)
		{
			sender.AddPacket(new ServerSendHierarchyPacket(this.ssm.ScanHierarchy()));
		}

		private void	Handle_Scene_ClientRequestLayers(Client sender, Packet _packet)
		{
			sender.AddPacket(new ServerSendLayersPacket());
		}

		private void	Handle_Scene_ClientRequestResources(Client sender, Packet _packet)
		{
			ClientRequestResourcesPacket	packet = _packet as ClientRequestResourcesPacket;
			string[]						resourceNames;
			int[]							instanceIDs;

			this.ssm.GetResources(packet.type, out resourceNames, out instanceIDs);

			sender.AddPacket(new ServerSendResourcesPacket(packet.type, resourceNames, instanceIDs));
		}

		private void	Handle_Scene_ClientSetSibling(Client sender, Packet _packet)
		{
			ClientSetSiblingPacket	packet = _packet as ClientSetSiblingPacket;

			if (this.ssm.SetSibling(packet.instanceID, packet.instanceIDParent, packet.siblingIndex) == true)
				sender.AddPacket(packet);
			else
				sender.AddPacket(new ErrorNotificationPacket(Errors.Server_GameObjectNotFound, this.PrepareGameObjectNotFoundMessage(packet, packet.instanceID + " or " + packet.instanceIDParent)));
		}

		private void	Handle_Scene_ClientRequestGameObjectData(Client sender, Packet _packet)
		{
			ClientRequestGameObjectDataPacket	packet = _packet as ClientRequestGameObjectDataPacket;
			ServerGameObject					gameObject = this.ssm.GetGameObject(packet.gameObjectInstanceID);

			if (gameObject != null)
			{
				try
				{
					sender.AddPacket(new ServerSendGameObjectDataPacket(gameObject));
				}
				catch (Exception ex)
				{
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_Exception, this.PrepareExceptionMessage(packet, ex)));
				}
			}
			else
				sender.AddPacket(new ErrorNotificationPacket(Errors.Server_GameObjectNotFound, this.PrepareGameObjectNotFoundMessage(packet, packet.gameObjectInstanceID.ToString())));
		}

		private void	Handle_Scene_ClientUpdateFieldValue(Client sender, Packet _packet)
		{
			ClientUpdateFieldValuePacket	packet = _packet as ClientUpdateFieldValuePacket;
			byte[]							newValue;
			ReturnUpdateFieldValue			r = this.ssm.UpdateFieldValue(packet.fieldPath, packet.rawValue, out newValue);

			if (r == ReturnUpdateFieldValue.Success)
				sender.AddPacket(new ServerUpdateFieldValuePacket(packet.fieldPath, newValue));
			else
			{
				switch (r)
				{
					case ReturnUpdateFieldValue.InternalError:
						sender.AddPacket(new ErrorNotificationPacket(Errors.Server_InternalServerError, this.PrepareInternalErrorMessage(packet)));
						break;
					case ReturnUpdateFieldValue.GameObjectNotFound:
						sender.AddPacket(new ErrorNotificationPacket(Errors.Server_GameObjectNotFound, this.PrepareGameObjectNotFoundMessage(packet, this.ExtractGameObjectInstanceID(packet.fieldPath))));
						break;
					case ReturnUpdateFieldValue.ComponentNotFound:
						sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ComponentNotFound, this.PrepareComponentNotFoundMessage(packet, this.ExtractComponentInstanceID(packet.fieldPath))));
						break;
					case ReturnUpdateFieldValue.PathNotResolved:
						sender.AddPacket(new ErrorNotificationPacket(Errors.Server_PathNotResolved, this.PreparePathNotResolvedMessage(packet, packet.fieldPath)));
						break;
				}
			}
		}

		private void	Handle_Scene_ClientInvokeBehaviourMethod(Client sender, Packet _packet)
		{
			ClientInvokeBehaviourMethodPacket	packet = _packet as ClientInvokeBehaviourMethodPacket;
			ReturnInvokeComponentMethod			r = this.ssm.InvokeComponentMethod(packet.gameObjectInstanceID, packet.instanceID, packet.method, packet.arguments);

			switch (r)
			{
				case ReturnInvokeComponentMethod.GameObjectNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_GameObjectNotFound, this.PrepareGameObjectNotFoundMessage(packet, packet.gameObjectInstanceID.ToString())));
					break;
				case ReturnInvokeComponentMethod.ComponentNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ComponentNotFound, this.PrepareComponentNotFoundMessage(packet, packet.instanceID.ToString())));
					break;
				case ReturnInvokeComponentMethod.MethodNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_MethodNotFound, this.PrepareMethodNotFoundMessage(packet, packet.method)));
					break;
				case ReturnInvokeComponentMethod.InvalidArgument:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_InvalidArgument, this.PrepareInvalidArgumentMessage(packet)));
					break;
			}
		}

		private void	Handle_Scene_ClientWatchGameObjects(Client sender, Packet _packet)
		{
			ClientWatchGameObjectsPacket	packet = _packet as ClientWatchGameObjectsPacket;

			this.ssm.WatchGameObjects(sender, packet.instanceIDs);
		}

		private void	Handle_Scene_ClientWatchMaterials(Client sender, Packet _packet)
		{
			ClientWatchMaterialsPacket	packet = _packet as ClientWatchMaterialsPacket;

			this.ssm.WatchMaterials(sender, packet.instanceIDs);
		}

		private void	Handle_Scene_ClientDeleteGameObjects(Client sender, Packet _packet)
		{
			ClientDeleteGameObjectsPacket	packet = _packet as ClientDeleteGameObjectsPacket;

			this.ssm.DeleteGameObjects(packet.instanceIDs);
		}

		private void	Handle_Scene_ClientDeleteComponents(Client sender, Packet _packet)
		{
			ClientDeleteComponentsPacket	packet = _packet as ClientDeleteComponentsPacket;

			this.ssm.DeleteComponents(packet.gameObjectInstanceIDs, packet.instanceIDs);
		}

		private void	Handle_Scene_ClientRequestMaterialData(Client sender, Packet _packet)
		{
			ClientRequestMaterialDataPacket	packet = _packet as ClientRequestMaterialDataPacket;
			Material						material = this.ssm.GetResource<Material>(packet.instanceID);

			if (material != null)
			{
				NGShader	ngShader = this.ssm.GetNGShader(material.shader);

				if (ngShader != null)
					sender.AddPacket(new ServerSendMaterialDataPacket(material, ngShader));
				else
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ShaderNotFound, this.PrepareShaderNotFoundMessage(packet, (material.shader != null ? material.shader.ToString() : "NULL"))));
			}
			else
				sender.AddPacket(new ErrorNotificationPacket(Errors.Server_MaterialNotFound, this.PrepareMaterialNotFoundMessage(packet, packet.instanceID.ToString())));
		}

		private void	Handle_Scene_ClientUpdateMaterialProperty(Client sender, Packet _packet)
		{
			ClientUpdateMaterialPropertyPacket	packet = _packet as ClientUpdateMaterialPropertyPacket;
			byte[]								newValue;
			ReturnUpdateMaterialProperty		r = this.ssm.UpdateMaterialProperty(packet.instanceID, packet.propertyName, packet.rawValue, out newValue);

			switch (r)
			{
				case ReturnUpdateMaterialProperty.Success:
					sender.AddPacket(new ServerUpdateMaterialPropertyPacket(packet.instanceID, packet.propertyName, newValue));
					break;
				case ReturnUpdateMaterialProperty.MaterialNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_MaterialNotFound, this.PrepareMaterialNotFoundMessage(packet, packet.instanceID.ToString())));
					break;
				case ReturnUpdateMaterialProperty.ShaderNotFound:
					Material	material = this.ssm.GetResource<Material>(packet.instanceID);
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ShaderNotFound, this.PrepareShaderNotFoundMessage(packet, (material.shader != null ? material.shader.ToString() : "NULL"))));
					break;
				case ReturnUpdateMaterialProperty.PropertyNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ShaderPropertyNotFound, this.PrepareShaderPropertyNotFoundMessage(packet, packet.propertyName)));
					break;
				case ReturnUpdateMaterialProperty.InternalError:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_InternalServerError, this.PrepareInternalErrorMessage(packet)));
					break;
			}
		}

		private void	Handle_Scene_ClientUpdateMaterialVector2(Client sender, Packet _packet)
		{
			ClientUpdateMaterialVector2Packet	packet = _packet as ClientUpdateMaterialVector2Packet;
			Vector2								newValue;
			ReturnUpdateMaterialVector2			r = this.ssm.UpdateMaterialVector2(packet.instanceID, packet.propertyName, packet.value, packet.type, out newValue);

			switch (r)
			{
				case ReturnUpdateMaterialVector2.Success:
					sender.AddPacket(new ServerUpdateMaterialVector2Packet(packet.instanceID, packet.propertyName, newValue, packet.type));
					break;
				case ReturnUpdateMaterialVector2.MaterialNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_MaterialNotFound, this.PrepareMaterialNotFoundMessage(packet, packet.instanceID.ToString())));
					break;
			}
		}

		private void	Handle_Scene_ClientChangeMaterialShader(Client sender, Packet _packet)
		{
			ClientChangeMaterialShaderPacket	packet = _packet as ClientChangeMaterialShaderPacket;
			Material							material = this.ssm.GetResource<Material>(packet.instanceID);

			if (material != null)
			{
				Shader	shader = this.ssm.GetResource<Shader>(packet.shaderInstanceID);

				if (shader != null)
				{
					material.shader = shader;

					NGShader	ngShader = this.ssm.GetNGShader(material.shader);

					if (ngShader != null)
						sender.AddPacket(new ServerSendMaterialDataPacket(material, ngShader));
					else
						sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ShaderNotFound, this.PrepareShaderNotFoundMessage(packet, (material.shader != null ? material.shader.ToString() : "NULL"))));
				}
				else
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ShaderNotFound, this.PrepareShaderNotFoundMessage(packet, packet.shaderInstanceID.ToString())));
			}
			else
				sender.AddPacket(new ErrorNotificationPacket(Errors.Server_MaterialNotFound, this.PrepareMaterialNotFoundMessage(packet, packet.instanceID.ToString())));
		}

		private void	Handle_Scene_ClientRequestEnumData(Client sender, Packet _packet)
		{
			ClientRequestEnumDataPacket	packet = _packet as ClientRequestEnumDataPacket;

			sender.AddPacket(new ServerSendEnumDataPacket(packet.type));
		}

		private void	Handle_Scene_ClientLoadBigArray(Client sender, Packet _packet)
		{
			ClientLoadBigArrayPacket	packet = _packet as ClientLoadBigArrayPacket;
			IEnumerable					array;
			ReturnGetArray				r = this.ssm.GetArray(packet.arrayPath, out array);

			switch (r)
			{
				case ReturnGetArray.Success:
					ArrayHandler	typeHandler = TypeHandlersManager.GetArrayRefTypeHandler();

					typeHandler.forceBigArray = true;
					sender.AddPacket(new ServerUpdateFieldValuePacket(packet.arrayPath, typeHandler.Serialize(array.GetType(), array)));
					break;
				case ReturnGetArray.GameObjectNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_GameObjectNotFound, this.PrepareGameObjectNotFoundMessage(packet, this.ExtractGameObjectInstanceID(packet.arrayPath))));
					break;
				case ReturnGetArray.ComponentNotFound:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_ComponentNotFound, this.PrepareComponentNotFoundMessage(packet, this.ExtractComponentInstanceID(packet.arrayPath))));
					break;
				case ReturnGetArray.PathNotResolved:
					sender.AddPacket(new ErrorNotificationPacket(Errors.Server_PathNotResolved, this.PreparePathNotResolvedMessage(packet, packet.arrayPath)));
					break;
			}
		}

		private void	Handle_Scene_ClientRequestProject(Client sender, Packet _packet)
		{
			sender.AddPacket(new ServerSendProjectPacket(this.ssm.resources.assets));
		}

		private CameraModulesRunner					modulesRunner;
		private Dictionary<Client, NGServerCamera>	NGGhostCams = new Dictionary<Client, NGServerCamera>();
		private List<CameraServerDataModule>		modules;

		private void	Handle_Camera_ClientDisconnect(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Disconnecting NG Camera while the server has not been initialized.");

			cam.enabled = false;

			this.modulesRunner.RemoveClient(sender);
		}

		private void	Handle_Camera_ClientConnect(Client sender, Packet _packet)
		{
			ClientConnectPacket	packet = _packet as ClientConnectPacket;

			if (SystemInfo.SupportsRenderTextureFormat(packet.format) == false)
			{
				sender.AddPacket(new ErrorNotificationPacket(Errors.Camera_RenderTextureFormatNotSupported, this.PrepareRenderTextureFormatNotSupportedMessage(packet, packet.format)));
				return;
			}

			// Initialize modules.
			if (this.modulesRunner == null)
			{
				GameObject modulesGameObject = new GameObject("NG Camera Modules");
				this.modules = new List<CameraServerDataModule>();

				this.modulesRunner = modulesGameObject.AddComponent<CameraModulesRunner>();
				this.modulesRunner.modules = this.modules;
				this.modulesRunner.scene = this.ssm;

				foreach (Type type in Utility.EachSubClassesOf(typeof(CameraServerDataModule)))
				{
					if (type == typeof(ScreenshotModule))
						continue;

					CameraServerDataModule	module = Activator.CreateInstance(type) as CameraServerDataModule;

					module.Awake(this.ssm);
					this.modules.Add(module);
				}

				this.modulesRunner.Init();
			}

			NGServerCamera	cam;

			if (this.NGGhostCams.TryGetValue(sender, out cam) == false)
			{
				GameObject	gameObject = new GameObject("NG Ghost Cam " + (this.NGGhostCams.Count + 1));
				cam = gameObject.AddComponent<NGServerCamera>();
				cam.scene = this.ssm;
				cam.sender = sender;
				this.NGGhostCams.Add(sender, cam);
			}

			cam.enabled = true;
			cam.width = packet.width;
			cam.height = packet.height;
			cam.depth = packet.depth;
			cam.targetRefresh = packet.targetRefresh;
			cam.renderTextureFormat = packet.format;
			cam.Init();

			List<byte>	modulesAvailable = new List<byte>();

			for (int i = 0; i < this.modules.Count; i++)
				modulesAvailable.Add(this.modules[i].moduleID);

			for (int j = 0; j < packet.modulesAvailable.Length; j++)
				this.modulesRunner.EnableModule(packet.modulesAvailable[j], sender);

			sender.AddPacket(new ServerIsInitializedPacket(packet.width, packet.height, packet.depth, packet.format, modulesAvailable.ToArray()));
		}

		private void	Handle_Camera_ClientRequestAllCameras(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Requesting all cameras while the server has not been initialized.");
			Camera[]	cameras = Resources.FindObjectsOfTypeAll<Camera>();

			sender.AddPacket(new ServerSendAllCamerasPacket(cameras, cam.ghostCamera.GetInstanceID()));
		}

		private void	Handle_Camera_ClientPickCamera(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Picking a camera while the server has not been initialized.");
			ClientPickCameraPacket	packet = _packet as ClientPickCameraPacket;

			Camera[]	cameras = Resources.FindObjectsOfTypeAll<Camera>();

			for (int i = 0; i < cameras.Length; i++)
			{
				if (cameras[i].GetInstanceID() == packet.ID)
				{
					cam.targetCamera = cameras[i];
					break;
				}
			}
		}

		private void	Handle_Camera_ClientPickGhostCameraAtCamera(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Picking ghost camera while the server has not been initialized.");
			ClientPickGhostCameraAtCameraPacket	packet = _packet as ClientPickGhostCameraAtCameraPacket;

			Camera[]	cameras = Resources.FindObjectsOfTypeAll<Camera>();

			for (int i = 0; i < cameras.Length; i++)
			{
				if (cameras[i].GetInstanceID() == packet.ID)
				{
					cam.ghostCamera.transform.position = cameras[i].transform.position;
					cam.ghostCamera.transform.rotation = cameras[i].transform.rotation;
					cam.targetCamera = cam.ghostCamera;
					break;
				}
			}
		}
		
		private void	Handle_Camera_ClientSetSetting(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Picking a camera while the server has not been initialized.");
			ClientSetSettingPacket	packet = _packet as ClientSetSettingPacket;

			if (packet.setting == ClientSetSettingPacket.Settings.TargetRefresh)
				cam.targetRefresh = Mathf.Clamp((int)packet.value, NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax);
			else if (packet.setting == ClientSetSettingPacket.Settings.Wireframe)
				cam.wireframe = (bool)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraClearFlags)
				cam.ghostCamera.clearFlags = (CameraClearFlags)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraBackground)
				cam.ghostCamera.backgroundColor = (Color)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraCullingMask)
				cam.ghostCamera.cullingMask = (int)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraProjection)
				cam.ghostCamera.orthographic = (int)packet.value == 1;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraFieldOfView)
				cam.ghostCamera.fieldOfView = (float)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraSize)
				cam.ghostCamera.orthographicSize = (float)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraClippingPlanesFar)
				cam.ghostCamera.farClipPlane = (float)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraClippingPlanesNear)
				cam.ghostCamera.nearClipPlane = (float)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraViewportRect)
				cam.ghostCamera.rect = (Rect)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraDepth)
				cam.ghostCamera.depth = (float)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraRenderingPath)
				cam.ghostCamera.renderingPath = (RenderingPath)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraOcclusionCulling)
				cam.ghostCamera.useOcclusionCulling = (bool)packet.value;
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraHDR)
				cam.ghostCamera.hdr = (bool)packet.value;
#if UNITY_5
			else if (packet.setting == ClientSetSettingPacket.Settings.CameraTargetDisplay)
				cam.ghostCamera.targetDisplay = (int)packet.value;
#endif
		}

		private void	Handle_Camera_ClientSendCameraInput(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Picking a camera while the server has not been initialized.");
			ClientSendCameraInputPacket	packet = _packet as ClientSendCameraInputPacket;

			cam.moveForward = packet.forward;
			cam.moveBackward = packet.backward;
			cam.moveLeft = packet.left;
			cam.moveRight = packet.right;
			cam.moveSpeed = packet.speed;
		}

		private void	Handle_Camera_ClientSendCameraTransformPosition(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Moving the camera while the server has not been initialized.");
			ClientSendCameraTransformPositionPacket	packet = _packet as ClientSendCameraTransformPositionPacket;

			cam.SetTransformPosition(packet.position);
		}

		private void	Handle_Camera_ClientSendCameraTransformRotation(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Rotating the camera while the server has not been initialized.");
			ClientSendCameraTransformRotationPacket	packet = _packet as ClientSendCameraTransformRotationPacket;

			cam.SetTransformRotation(packet.rotation);
		}

		private void	Handle_Camera_ClientSendCameraZoom(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Zooming the camera while the server has not been initialized.");
			ClientSendCameraZoomPacket	packet = _packet as ClientSendCameraZoomPacket;

			cam.Zoom(packet.factor);
		}

		private void	Handle_Camera_ClientRaycastScene(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Raycasting while the server has not been initialized.");
			ClientRaycastScenePacket	packet = _packet as ClientRaycastScenePacket;

			cam.Raycast(this.cameraRaycastResult, packet.viewportX, packet.viewportY);

			sender.AddPacket(new ServerSendRaycastResultPacket(this.cameraRaycastResult));
		}

		private void	Handle_Camera_ClientToggleModule(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Toggling a module while the server has not been initialized.");
			ClientToggleModulePacket	packet = _packet as ClientToggleModulePacket;

			if (packet.active == true)
			{
				if (this.modulesRunner.EnableModule(packet.moduleID, sender) == false)
					sender.AddPacket(new ErrorNotificationPacket(Errors.Camera_ModuleNotFound, "Module " + packet.moduleID + " was not found. Can not toggle to true."));
			}
			else if (this.modulesRunner.DisableModule(packet.moduleID, sender) == false)
				sender.AddPacket(new ErrorNotificationPacket(Errors.Camera_ModuleNotFound, "Module " + packet.moduleID + " was not found. Can not toggle to false."));
		}

		private void	Handle_Camera_ClientStickGhostCamera(Client sender, Packet _packet)
		{
			NGServerCamera	cam;
			bool	mustSucceed = this.NGGhostCams.TryGetValue(sender, out cam);
			InternalNGDebug.Assert(mustSucceed, "Toggling a module while the server has not been initialized.");
			ClientStickGhostCameraPacket	packet = _packet as ClientStickGhostCameraPacket;

			if (packet.ID == 0)
				cam.ghostCamera.transform.SetParent(null, true);
			else
			{
				Transform	parent = this.ssm.GetResource<Transform>(packet.ID);
				cam.ghostCamera.transform.SetParent(parent, true);
			}

			sender.AddPacket(new ServerStickGhostCameraPacket(packet.ID));
		}

		private string	ExtractGameObjectInstanceID(string path)
		{
			return path.Substring(0, path.IndexOf('.'));
		}

		private string	ExtractComponentInstanceID(string path)
		{
			int	n = path.IndexOf('.');
			return path.Substring(n + 1, path.IndexOf('.', n + 1) - n - 1);
		}

		private string	PrepareInternalErrorMessage(Packet packet)
		{
			return "Internal error occurs on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareExceptionMessage(Packet packet, Exception ex)
		{
			return "An exception has been thrown on the server for packet " + packet.GetType().Name + "." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
		}

		private string	PrepareGameObjectNotFoundMessage(Packet packet, string instanceID)
		{
			return "GameObject (" + instanceID + ") was not found on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareComponentNotFoundMessage(Packet packet, string instanceID)
		{
			return "Component (" + instanceID + ") was not found on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PreparePathNotResolvedMessage(Packet packet, string path)
		{
			return "Server was not able to resolve the path (" + path + ") for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareMethodNotFoundMessage(Packet packet, string method)
		{
			return "Server failed to found method (" + method + ") for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareInvalidArgumentMessage(Packet packet)
		{
			return "An argument is invalid for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareMaterialNotFoundMessage(Packet packet, string instanceID)
		{
			return "Material (" + instanceID + ") was not found on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareShaderNotFoundMessage(Packet packet, string shader)
		{
			return "Shader (" + shader + ") was not found on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareShaderPropertyNotFoundMessage(Packet packet, string propertyName)
		{
			return "Shader property (" + propertyName + ") was not found on the server for packet " + packet.GetType().Name + ".";
		}

		private string	PrepareRenderTextureFormatNotSupportedMessage(Packet packet, RenderTextureFormat format)
		{
			return "The RenderTextureFormat \"" + format + "\" requested by " + packet.GetType().Name + " is not supported by the platform.";
		}
	}
}