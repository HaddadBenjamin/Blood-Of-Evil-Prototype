using NGTools.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorComponent : MonitorData
	{
		public int	InstanceID { get { return this.instanceID; } }

		private Component	component;
		private int			gameObjectInstanceID;
		private int			instanceID;

		public	MonitorComponent(string path, Func<object> component) : base(path, component)
		{
			this.component = this.getInstance() as Component;
			this.gameObjectInstanceID = this.component.gameObject.GetInstanceID();
			this.instanceID = this.component.GetInstanceID();

			this.MonitorSubData(this.component.GetType(), this.getInstance, true);
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.component == null)
			{
				updates.Add(this);
				this.ToDelete = true;
				return;
			}

			if (this.children != null)
			{
				for (int i = 0; i < this.children.Count; i++)
					this.children[i].CollectUpdates(updates);
			}
		}

		public override void	Update()
		{
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerDeleteComponentsPacket(this.gameObjectInstanceID, this.instanceID) };
		}
	}
}