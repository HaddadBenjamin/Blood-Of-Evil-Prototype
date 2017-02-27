using NGTools;
using NGTools.NGRemoteScene;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public abstract class ReplayDataModule : CameraDataModule
	{
		public List<CameraData>	data = new List<CameraData>();

		protected int	index = -1;

		protected	ReplayDataModule(byte moduleID, int priority, string name) : base(moduleID, priority, name)
		{
		}

		/// <summary>
		/// Called in OnGUI in NG Replay.
		/// </summary>
		/// <param name="r"></param>
		public virtual void	OnGUIReplay(Rect r)
		{
		}

		/// <summary>
		/// Set the index on the most recent data before the given <paramref name="time"/>.
		/// </summary>
		/// <param name="time"></param>
		public virtual void	SetTime(float time)
		{
			if (this.index == -1 || time > this.data[this.index].time)
			{
				for (; this.index < this.data.Count; this.index++)
				{
					if (this.index >= 0 && this.data[this.index].time > time)
					{
						if (this.index >= 0)
							--this.index;
						break;
					}
				}

				if (this.index >= this.data.Count)
					this.index = this.data.Count - 1;
			}
			else if (time < this.data[this.index].time)
			{
				for (; this.index >= 0; --this.index)
				{
					if (this.data[this.index].time <= time)
						break;
				}
			}
		}

		public abstract void	Export(ByteBuffer writer);
		public abstract void	Import(Replay settings, ByteBuffer reader);

		public virtual void	OnGUIDBG()
		{
		}
	}
}