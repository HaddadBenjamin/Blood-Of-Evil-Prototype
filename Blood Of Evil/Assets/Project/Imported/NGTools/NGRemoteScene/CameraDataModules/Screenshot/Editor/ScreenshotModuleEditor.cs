using NGTools.Network;
using NGTools.NGRemoteScene;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class ScreenshotModuleEditor : CameraDataModuleEditor
	{
		internal sealed class Screenshot : CameraData
		{
			public byte[]	data;
		}

		private bool		useJPG;
		private Texture2D	texture;

		public	ScreenshotModuleEditor() : base(ScreenshotModule.ModuleID, ScreenshotModule.Priority, ScreenshotModule.Name)
		{
		}

		public override void	OnGUICamera(IReplaySettings settings, Rect r)
		{
			if (this.texture == null ||
				this.texture.width != settings.TextureWidth ||
				this.texture.height != settings.TextureHeight)
			{
				this.texture = new Texture2D(settings.TextureWidth, settings.TextureHeight, TextureFormat.ARGB32, false);
			}

			EditorGUI.DrawPreviewTexture(r, this.texture, null, ScaleMode.ScaleAndCrop);
		}

		public override void	OnGUIModule(NGRemoteHierarchyWindow hierarchy)
		{
			EditorGUI.BeginChangeCheck();
			this.useJPG = EditorGUILayout.Toggle("Use JPG", this.useJPG);
			if (EditorGUI.EndChangeCheck() == true && hierarchy.IsClientConnected() == true)
				hierarchy.Client.AddPacket(new ClientModuleSetUseJPGPacket(this.useJPG));
		}

		public override void	OnServerInitialized(IReplaySettings settings, Client server)
		{
			server.AddPacket(new ClientModuleSetUseJPGPacket(this.useJPG));
		}

		public override void	HandlePacket(IReplaySettings settings, float time, byte[] data)
		{
			this.RemoveOldData(time - settings.RecordLastSeconds);

			this.texture.LoadImage(data);

			this.data.Add(new Screenshot() { time = time, data = data });
		}

		public override ReplayDataModule	ConvertToReplay(IReplaySettings settings)
		{
			return new ScreenshotReplayModule(this, settings.TextureWidth, settings.TextureHeight);
		}
	}
}