using NGTools;
using NGTools.NGRemoteScene;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class MouseModuleEditor : CameraDataModuleEditor
	{
		internal sealed class MouseInput : CameraData
		{
			public float	x;
			public float	y;
			public MouseButtons	buttons;
		}

		public	MouseModuleEditor() : base(MouseModule.ModuleID, MouseModule.Priority, MouseModule.Name)
		{
		}

		public override void	HandlePacket(IReplaySettings settings, float time, byte[] data)
		{
			this.RemoveOldData(time - settings.RecordLastSeconds);

			ByteBuffer	buffer = Utility.GetBBuffer();
			buffer.Append(data);

			this.data.Add(new MouseInput() {
				time = time,
				x = buffer.ReadSingle(),
				y = buffer.ReadSingle(),
				buttons = (MouseButtons)buffer.ReadInt32()
			});

			Utility.RestoreBBuffer(buffer);
		}

		public override ReplayDataModule	ConvertToReplay(IReplaySettings replay)
		{
			if (this.data.Count == 0)
				return null;

			return new MouseReplayModule(this);
		}
	}
}