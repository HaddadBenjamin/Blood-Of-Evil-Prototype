using NGTools;
using NGTools.NGRemoteScene;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class TouchModuleEditor : CameraDataModuleEditor
	{
		internal sealed class TouchesState : CameraData
		{
			internal struct Touch
			{
				public float	x;
				public float	y;
				public int		fingerID;
			}

			public Touch[]	touches;
		}

		public	TouchModuleEditor() : base(TouchModule.ModuleID, TouchModule.Priority, TouchModule.Name)
		{
		}

		public override void	HandlePacket(IReplaySettings settings, float time, byte[] data)
		{
			this.RemoveOldData(time - settings.RecordLastSeconds);

			ByteBuffer	buffer = Utility.GetBBuffer();
			buffer.Append(data);

			int				touchCount = (int)buffer.ReadByte();
			TouchesState	input = new TouchesState() {
				time = time,
				touches = new TouchesState.Touch[touchCount]
			};

			for (int i = 0; i < touchCount; i++)
			{
				input.touches[i].x = buffer.ReadSingle();
				input.touches[i].y = buffer.ReadSingle();
				input.touches[i].fingerID = buffer.ReadInt32();
			}

			this.data.Add(input);
			Utility.RestoreBBuffer(buffer);
		}

		public override ReplayDataModule	ConvertToReplay(IReplaySettings replay)
		{
			if (this.data.Count == 0)
				return null;

			return new TouchReplayModule(this);
		}
	}
}