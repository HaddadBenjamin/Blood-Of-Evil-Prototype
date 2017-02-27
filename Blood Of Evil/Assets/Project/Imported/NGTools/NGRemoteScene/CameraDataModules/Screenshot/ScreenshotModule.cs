using NGTools.Network;
using UnityEngine;

namespace NGTools.Network
{
	internal partial class PacketId
	{
		public const int	Camera_ClientModuleSetUseJPG = 10000;
	}
}

namespace NGTools.NGRemoteScene
{
	internal sealed class ScreenshotModule : CameraServerDataModule
	{
		public const byte	ModuleID = 1;
		public const int	Priority = 1000;
		public const string	Name = "Screenshot";

		private float		nextTime;
		private Texture2D	texture;
		private bool		useJPG = true;

		public	ScreenshotModule() : base(ScreenshotModule.ModuleID, ScreenshotModule.Priority, ScreenshotModule.Name)
		{
		}

		public override void	Awake(NGServerScene scene)
		{
			scene.executer.HandlePacket(PacketId.Camera_ClientModuleSetUseJPG, this.SetUseJPG);
		}

		public override void	OnDestroy(NGServerScene scene)
		{
			scene.executer.UnhandlePacket(PacketId.Camera_ClientModuleSetUseJPG);
		}

		public void	Update(ICameraScreenshotData data)
		{
			float	t = Time.time;

			if (t <= this.nextTime)
				return;

			float	timeOverflow = 0F;
			if (t - this.nextTime <= (1F / data.TargetRefresh) * 2F)
				timeOverflow = t - this.nextTime;

			this.nextTime = t + (1F / data.TargetRefresh) - timeOverflow;

			RenderTexture	restore = data.TargetCamera.targetTexture;
			data.TargetCamera.targetTexture = data.RenderTexture;

			bool	restoreWire = GL.wireframe;
			GL.wireframe = data.Wireframe;

			data.TargetCamera.Render();

			GL.wireframe = restoreWire;
			data.TargetCamera.targetTexture = restore;

			RenderTexture.active = data.RenderTexture;

			if (this.texture == null)
			{
				this.texture = new Texture2D(data.Width, data.Height, TextureFormat.ARGB32, false);
				this.texture.hideFlags = HideFlags.HideAndDontSave;
			}

			this.texture.ReadPixels(new Rect(0F, 0F, data.Width, data.Height), 0, 0);
			RenderTexture.active = null;

			data.Sender.AddPacket(new CameraDataPacket(this.moduleID, Time.time, this.useJPG == true ? this.texture.EncodeToJPG() : this.texture.EncodeToPNG()));
		}

		private void	SetUseJPG(Client client, Packet _packet)
		{
			ClientModuleSetUseJPGPacket	packet = _packet as ClientModuleSetUseJPGPacket;

			this.useJPG = packet.useJPG;
		}
	}
}