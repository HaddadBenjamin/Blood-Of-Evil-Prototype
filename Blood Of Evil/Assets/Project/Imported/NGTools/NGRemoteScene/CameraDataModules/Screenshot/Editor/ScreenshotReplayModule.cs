using NGTools;
using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class ScreenshotReplayModule : ReplayDataModule
	{
		private bool		keepAspectRatio = false;
		private Texture2D	texture;

		public	ScreenshotReplayModule() : base(ScreenshotModule.ModuleID, ScreenshotModule.Priority, ScreenshotModule.Name)
		{
		}

		public	ScreenshotReplayModule(ScreenshotModuleEditor module, int textureWidth, int textureHeight) : base(ScreenshotModule.ModuleID, ScreenshotModule.Priority, ScreenshotModule.Name)
		{
			this.data.AddRange(module.data);
			this.texture = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
			this.texture.hideFlags = HideFlags.HideAndDontSave;
			this.texture.LoadImage((this.data[0] as ScreenshotModuleEditor.Screenshot).data);
		}

		public override void	OnGUIReplay(Rect r)
		{
			EditorGUI.DrawPreviewTexture(r, this.texture, null, this.keepAspectRatio == true ? ScaleMode.ScaleToFit : ScaleMode.StretchToFill);
		}

		public override void	SetTime(float time)
		{
			int	lastIndex = this.index;

			base.SetTime(time);

			if (this.index < 0)
				this.index = 0;

			if (lastIndex != this.index)
				this.texture.LoadImage((this.data[this.index] as ScreenshotModuleEditor.Screenshot).data);
		}

		public override void	Export(ByteBuffer buffer)
		{
			buffer.Append(this.data.Count);

			foreach (ScreenshotModuleEditor.Screenshot screenshot in this.data)
			{
				buffer.Append(screenshot.time);
				buffer.Append(screenshot.data.Length);
				buffer.Append(screenshot.data);
			}
		}

		public override void	Import(Replay replay, ByteBuffer buffer)
		{
			Int32	count = buffer.ReadInt32();

			this.data.Clear();
			this.data.Capacity = count;

			for (int i = 0; i < count; i++)
			{
				ScreenshotModuleEditor.Screenshot	screenshot = new ScreenshotModuleEditor.Screenshot();
				screenshot.time = buffer.ReadSingle();
				screenshot.data = buffer.ReadBytes(buffer.ReadInt32());
				this.data.Add(screenshot);
			}

			this.texture = new Texture2D(replay.width, replay.height, TextureFormat.ARGB32, false);
			this.texture.hideFlags = HideFlags.HideAndDontSave;
		}
	}
}