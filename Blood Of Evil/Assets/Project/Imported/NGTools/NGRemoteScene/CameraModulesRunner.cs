using NGTools.Network;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public class CameraModulesRunner : MonoBehaviour, ICameraData
	{
		public List<CameraServerDataModule>	activeModules;
		public List<CameraServerDataModule>	modules;
		public NGServerScene				scene;

		public NGServerScene		ServerScene { get { return this.scene; } }
		public AbstractTcpListener	TCPListener { get { return this.scene.listener; } }

		private List<Client>[]	moduleCounters;

		protected virtual void	Awake()
		{
			this.activeModules = new List<CameraServerDataModule>();
		}

		protected virtual void	OnGUI()
		{
			for (int i = 0; i < this.activeModules.Count; i++)
				this.activeModules[i].OnGUI(this);
		}

		protected virtual void	Update()
		{
			for (int i = 0; i < this.activeModules.Count; i++)
				this.activeModules[i].Update(this);
		}

		public void	Init()
		{
			this.moduleCounters = new List<Client>[this.modules.Count];
			for (int i = 0; i < this.moduleCounters.Length; i++)
				this.moduleCounters[i] = new List<Client>();
		}

		public void	RemoveClient(Client sender)
		{
			for (int i = 0; i < this.moduleCounters.Length; i++)
				this.moduleCounters[i].Remove(sender);
			this.UpdateModules();
		}

		public bool	EnableModule(byte moduleID, Client sender)
		{
			for (int i = 0; i < this.modules.Count; i++)
			{
				if (this.modules[i].moduleID == moduleID)
				{
					this.moduleCounters[i].Add(sender);
					this.UpdateModules();
					return true;
				}
			}

			return false;
		}

		public bool	DisableModule(int moduleID, Client sender)
		{
			for (int i = 0; i < this.modules.Count; i++)
			{
				if (this.modules[i].moduleID == moduleID)
				{
					this.moduleCounters[i].Remove(sender);
					this.UpdateModules();
					return true;
				}
			}

			return false;
		}

		private void	UpdateModules()
		{
			this.activeModules.Clear();

			for (int i = 0; i < this.moduleCounters.Length; i++)
			{
				if (this.moduleCounters[i].Count > 0)
					this.activeModules.Add(this.modules[i]);
			}
		}
	}
}