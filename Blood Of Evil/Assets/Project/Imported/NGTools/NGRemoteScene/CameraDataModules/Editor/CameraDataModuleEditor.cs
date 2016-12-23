using NGTools;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	public abstract class CameraDataModuleEditor : CameraDataModule
	{
		public List<CameraData>	data = new List<CameraData>();

		protected	CameraDataModuleEditor(byte moduleID, int priority, string name) : base(moduleID, priority, name)
		{
		}

		public virtual void	OnGUI(IReplaySettings settings, Rect r)
		{
		}

		public virtual void	OnGUIModule(NGHierarchyWindow hierarchy)
		{
		}

		public virtual void	OnServerInitialized(IReplaySettings settings, Client server)
		{
		}

		public abstract void	HandlePacket(IReplaySettings settings, float time, byte[] data);

		/// <summary>
		/// Generates a Replay version of the current module. Returns null if the module has no data.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public abstract ReplayDataModule	ConvertToReplay(IReplaySettings settings);

		protected void	RemoveOldData(float time)
		{
			for (int i = 0; i < this.data.Count; i++)
			{
				if (this.data[i].time > time)
				{
					if (i > 0)
						this.data.RemoveRange(0, i);
					break;
				}
			}
		}
	}
}