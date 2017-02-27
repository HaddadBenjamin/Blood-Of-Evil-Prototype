namespace NGTools.Network
{
	internal partial class PacketId
	{
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
		public const int	Scene_ServerReturnInvokeResult = 3122;

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
}