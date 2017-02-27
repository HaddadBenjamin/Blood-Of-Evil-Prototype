using NGTools;
using NGTools.NGRemoteScene;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class KeyboardModuleEditor : CameraDataModuleEditor
	{
		internal sealed class KeyInput : CameraData
		{
			public KeyCode[]	keys;
		}

		private List<KeyCode>	currentState = new List<KeyCode>();

		public	KeyboardModuleEditor() : base(KeyboardModule.ModuleID, KeyboardModule.Priority, KeyboardModule.Name)
		{
		}

		public override void	HandlePacket(IReplaySettings settings, float time, byte[] data)
		{
			this.RemoveOldData(time - settings.RecordLastSeconds);

			ByteBuffer	buffer = Utility.GetBBuffer();
			buffer.Append(data);

			KeyCode	keyCode = (KeyCode)buffer.ReadUInt16();
			bool	up = buffer.ReadBoolean();

			if (up == false)
				this.currentState.Add(keyCode);
			else
				this.currentState.Remove(keyCode);

			this.data.Add(new KeyInput() {
				time = time,
				keys = this.currentState.ToArray()
			});

			Utility.RestoreBBuffer(buffer);
		}

		public override ReplayDataModule	ConvertToReplay(IReplaySettings replay)
		{
			if (this.data.Count == 0)
				return null;

			return new KeyboardReplayModule(this);
		}
	}
}