using NGTools.Network;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public class NGServerCamera : MonoBehaviour, ICameraScreenshotData
	{
		public const int			TargetRefreshMin = 5;
		public const int			TargetRefreshMax = 200;
		public static RaycastHit[]	RaycastResult = new RaycastHit[10];

		public NGServerScene	scene;
		public Client			sender;

		public NGServerScene		ServerScene { get { return this.scene; } }
		public Client				Sender { get { return this.sender; } }
		public int					Width { get { return this.width; } }
		public int					Height { get { return this.height; } }
		public int					TargetRefresh { get { return this.targetRefresh; } }
		public bool					Wireframe { get { return this.wireframe; } }
		public Camera				TargetCamera { get { return this.targetCamera; } }
		public RenderTexture		RenderTexture { get { return this.renderTexture; } }
		public AbstractTcpListener	TCPListener { get { return this.scene.listener; } }

		public int					width = 800;
		public int					height = 600;
		public int					depth = 24;
		public RenderTextureFormat	renderTextureFormat = RenderTextureFormat.ARGB32;

		public bool		wireframe = false;
		[Range(NGServerCamera.TargetRefreshMin, NGServerCamera.TargetRefreshMax)]
		public int		targetRefresh = 24;

		public bool		moveForward;
		public bool		moveBackward;
		public bool		moveLeft;
		public bool		moveRight;
		public float	moveSpeed;

		public Camera			ghostCamera;
		public Camera			targetCamera;
		public RenderTexture	renderTexture;

		public int		FPS;
		public int		FPSSent;
		public float	nextFPSTime;

		private ScreenshotModule	screenshotModule;

		private Transform	cacheTransform;
		private Vector3		lastPosition;
		private Vector3		lastEulerAngles;

		private Vector3	viewportRay = new Vector3();

		protected virtual void	Awake()
		{
			this.ghostCamera = this.gameObject.AddComponent<Camera>();
			this.ghostCamera.enabled = false;
			this.targetCamera = this.ghostCamera;
			this.cacheTransform = this.transform;
		}

		protected virtual void	OnDestroy()
		{
			this.screenshotModule.OnDestroy(this.scene);
		}

		public void	Init()
		{
			if (this.screenshotModule == null)
			{
				this.screenshotModule = new ScreenshotModule();
				this.screenshotModule.Awake(this.scene);
			}

			this.targetCamera = this.ghostCamera;
			this.renderTexture = new RenderTexture(this.width, this.height, this.depth, this.renderTextureFormat, RenderTextureReadWrite.sRGB);
		}

		protected virtual void	OnGUI()
		{
			this.screenshotModule.OnGUI(this);
		}

		protected virtual void	Update()
		{
			this.screenshotModule.Update(this);

			float	t = Time.time;
			this.FPSSent++;
			if (t >= this.nextFPSTime)
			{
				this.FPS = this.FPSSent;
				this.FPSSent = 0;
				this.nextFPSTime = t + 1F;
			}

			if (this.moveForward == true)
				this.cacheTransform.localPosition += this.cacheTransform.forward * Time.deltaTime * this.moveSpeed;
			else if (this.moveBackward == true)
				this.cacheTransform.localPosition -= this.cacheTransform.forward * Time.deltaTime * this.moveSpeed;
			if (this.moveLeft == true)
				this.cacheTransform.localPosition -= this.cacheTransform.right * Time.deltaTime * this.moveSpeed;
			else if (this.moveRight == true)
				this.cacheTransform.localPosition += this.cacheTransform.right * Time.deltaTime * this.moveSpeed;

			if (this.cacheTransform.position != this.lastPosition ||
				this.cacheTransform.eulerAngles != this.lastEulerAngles)
			{
				this.lastPosition = this.cacheTransform.position;
				this.lastEulerAngles = this.cacheTransform.eulerAngles;

				this.sender.AddPacket(new ServerSendCameraTransformPacket(this.cacheTransform.position, this.cacheTransform.eulerAngles.x, this.cacheTransform.eulerAngles.y));
			}
		}

		public void	SetTransformPosition(Vector3 position)
		{
			this.cacheTransform.position = position;
			this.lastPosition = this.cacheTransform.position;
		}

		public void	SetTransformRotation(Vector2 rotation)
		{
			this.cacheTransform.eulerAngles = new Vector3(rotation.x, rotation.y);
			this.lastEulerAngles = this.cacheTransform.eulerAngles;
		}

		public void	Zoom(float factor)
		{
			this.cacheTransform.position += this.cacheTransform.forward * factor;
		}

		public void	Raycast(List<GameObject> result, float viewportX, float viewportY)
		{
			this.viewportRay.x = viewportX;
			this.viewportRay.y = viewportY;

			// Assign the render texture to force camera to work on the same screen resolution.
			RenderTexture	r = this.targetCamera.targetTexture;
			this.targetCamera.targetTexture = this.renderTexture;
			Ray				ray = this.targetCamera.ViewportPointToRay(this.viewportRay);
			this.targetCamera.targetTexture = r;

#if UNITY_5_3 || UNITY_5_4
			int	n = Physics.RaycastNonAlloc(ray, NGServerCamera.RaycastResult, float.MaxValue);
#else
			NGServerCamera.RaycastResult = Physics.RaycastAll(ray, float.MaxValue);
			int	n = NGServerCamera.RaycastResult.Length;
#endif

			if (Conf.DebugMode != Conf.DebugModes.None)
			{
				Debug.DrawRay(ray.origin, ray.direction * this.targetCamera.farClipPlane, Color.blue, 3F, true);
				NGDebug.Log(NGServerCamera.RaycastResult);
			}

			result.Clear();

			for (int i = 0; i < n; i++)
				result.Add(NGServerCamera.RaycastResult[i].collider.gameObject);
		}
	}
}