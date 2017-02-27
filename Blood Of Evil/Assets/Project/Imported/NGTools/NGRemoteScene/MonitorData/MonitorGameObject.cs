using NGTools.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorGameObject : MonitorData
	{
		private static List<MonitorData>	updatedData = new List<MonitorData>();

		private ServerGameObject		gameObject;
		private int						gameObjectInstanceID;
		private List<ServerComponent>	newComponents = new List<ServerComponent>();

		public	MonitorGameObject(ServerGameObject gameObject) : base(gameObject.gameObject.GetInstanceID().ToString(), () => gameObject)
		{
			this.gameObject = gameObject;
			this.gameObjectInstanceID = this.gameObject.instanceID;

			Component[]	components = this.gameObject.gameObject.GetComponents<Component>();
			this.children = new List<MonitorData>(components.Length + 5);

			Type	type = typeof(GameObject);

			PropertyInfo	propertyInfo = type.GetProperty("active");
			this.children.Add(new MonitorProperty(this.path + ".active", () => this.gameObject.gameObject, propertyInfo));
			propertyInfo = type.GetProperty("name");
			this.children.Add(new MonitorString(this.path + ".name", () => this.gameObject.gameObject, new PropertyModifier(propertyInfo)));
			propertyInfo = type.GetProperty("isStatic");
			this.children.Add(new MonitorProperty(this.path + ".isStatic", () => this.gameObject.gameObject, propertyInfo));
			propertyInfo = type.GetProperty("tag");
			this.children.Add(new MonitorString(this.path + ".tag", () => this.gameObject.gameObject, new PropertyModifier(propertyInfo)));
			propertyInfo = type.GetProperty("layer");
			this.children.Add(new MonitorProperty(this.path + ".layer", () => this.gameObject.gameObject, propertyInfo));

			for (int i = 0; i < components.Length; i++)
				this.children.Add(new MonitorComponent(this.path + "." + components[i].GetInstanceID().ToString(), this.GetClosureComponent(components[i])));
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.gameObject.gameObject == null)
			{
				updates.Add(this);
				return;
			}

			// Remove destroyed components.
			for (int i = 5; i < this.children.Count; i++)
			{
				if (this.children[i].ToDelete == true)
					this.children.RemoveAt(i--);
			}

			bool	inList = false;

			// Add new components.
			Component[]	components = this.gameObject.gameObject.GetComponents<Component>();
			for (int i = 0; i < components.Length; i++)
			{
				int	j = 0;

				for (; j < this.children.Count; j++)
				{
					if (object.Equals(components[i], this.children[j].Instance) == true)
						break;
				}

				if (j == this.children.Count)
				{
					this.children.Add(new MonitorComponent(this.path + "." + components[i].GetInstanceID().ToString(), this.GetClosureComponent(components[i])));

					ServerComponent	newComponent = new ServerComponent(components[i]);
					this.gameObject.components.Add(newComponent);
					this.newComponents.Add(newComponent);

					if (inList == false)
					{
						inList = true;
						updates.Add(this);
					}
				}
			}

			for (int i = 0; i < this.children.Count; i++)
				this.children[i].CollectUpdates(updates);
		}

		public override void	Update()
		{
		}

		public override Packet[]	CreateUpdatePackets()
		{
			if (this.newComponents.Count > 0)
			{
				Packet[]	p = new Packet[this.newComponents.Count];

				for (int i = 0; i < this.newComponents.Count; i++)
					p[i] = new ServerSendComponentPacket(this.gameObjectInstanceID, this.newComponents[i]);

				this.newComponents.Clear();
				return p;
			}

			return new Packet[] { new ServerDeleteGameObjectsPacket(this.gameObjectInstanceID) };
		}

		public void	UpdateValues(List<Client> clients)
		{
			MonitorGameObject.updatedData.Clear();

			this.CollectUpdates(MonitorGameObject.updatedData);
			//InternalNGDebug.Log("WatcherCollect=" + MonitorGameObject.updatedData.Count);

			for (int i = 0; i < MonitorGameObject.updatedData.Count; i++)
			{
				MonitorGameObject.updatedData[i].Update();

				Packet[]	packets = MonitorGameObject.updatedData[i].CreateUpdatePackets();
				if (packets == null)
					continue;

				for (int j = 0; j < clients.Count; j++)
				{
					for (int k = 0; k < packets.Length; k++)
						clients[j].AddPacket(packets[k]);
				}
			}
		}

		public void	DeleteComponent(int instanceID)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				MonitorComponent	component = this.children[i] as MonitorComponent;

				if (component != null)
				{
					if (component.InstanceID == instanceID)
					{
						this.children.RemoveAt(i);
						break;
					}
				}
			}
		}

		private Func<object>	GetClosureComponent(Component c)
		{
			return () => c;
		}
	}
}