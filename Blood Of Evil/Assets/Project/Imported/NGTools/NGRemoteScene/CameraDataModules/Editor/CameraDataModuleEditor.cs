using NGTools.Network;
using NGTools.NGRemoteScene;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public abstract class CameraDataModuleEditor : CameraDataModule
	{
		public List<CameraData>	data = new List<CameraData>();

		protected	CameraDataModuleEditor(byte moduleID, int priority, string name) : base(moduleID, priority, name)
		{
		}

		/// <summary>
		/// Displays GUI over the video feed.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="r"></param>
		public virtual void	OnGUICamera(IReplaySettings settings, Rect r)
		{
		}

		/// <summary>
		/// Displays GUI to alter module's settings.
		/// </summary>
		/// <param name="hierarchy"></param>
		public virtual void	OnGUIModule(NGRemoteHierarchyWindow hierarchy)
		{
		}

		/// <summary>
		/// Initializes module. Invoked when the server is initialized.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="server"></param>
		public virtual void	OnServerInitialized(IReplaySettings settings, Client server)
		{
		}

		/// <summary>
		/// Handle packet data incoming from the server.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="time"></param>
		/// <param name="data"></param>
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