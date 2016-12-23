using NGTools;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class ScreenshotModuleEditor : CameraDataModuleEditor
	{
		public class Screenshot : CameraData
		{
			public byte[]	data;
		}

		private bool		useJPG;
		private Texture2D	texture;

		public	ScreenshotModuleEditor() : base(ScreenshotModule.ModuleID, ScreenshotModule.Priority, ScreenshotModule.Name)
		{
		}

		public override void	OnGUI(IReplaySettings settings, Rect r)
		{
			if (this.texture == null ||
				this.texture.width != settings.TextureWidth ||
				this.texture.height != settings.TextureHeight)
			{
				this.texture = new Texture2D(settings.TextureWidth, settings.TextureHeight, TextureFormat.ARGB32, false);
			}

			EditorGUI.DrawPreviewTexture(r, this.texture, null, ScaleMode.ScaleAndCrop);
		}

		public override void	OnGUIModule(NGHierarchyWindow hierarchy)
		{
			EditorGUI.BeginChangeCheck();
			this.useJPG = EditorGUILayout.Toggle("Use JPG", this.useJPG);
			if (EditorGUI.EndChangeCheck() == true && hierarchy.IsClientConnected() == true)
			{
				hierarchy.Client.AddPacket(new ClientModulesetUseJPG(this.useJPG));
			}
		}

		public override void	OnServerInitialized(IReplaySettings settings, Client server)
		{
			server.AddPacket(new ClientModulesetUseJPG(this.useJPG));
		}

		public override void	HandlePacket(IReplaySettings settings, float time, byte[] data)
		{
			this.RemoveOldData(time - settings.RecordLastSeconds);

			this.texture.LoadImage(data);

			this.data.Add(new Screenshot() { time = time, data = data });
		}

		public override ReplayDataModule	ConvertToReplay(IReplaySettings settings)
		{
			ScreenshotReplayModule	module = new ScreenshotReplayModule(this, settings.TextureWidth, settings.TextureHeight);
			return module;
		}
	}
}