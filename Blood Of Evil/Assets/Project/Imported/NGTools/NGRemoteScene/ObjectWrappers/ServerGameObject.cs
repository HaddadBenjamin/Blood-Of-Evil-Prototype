using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	public sealed class ServerGameObject
	{
		private static List<Component>	tempListComponents = new List<Component>();

		public readonly GameObject				gameObject;
		public readonly int						instanceID;
		public readonly List<ServerGameObject>	children;
		public List<ServerComponent>			components;

		public	ServerGameObject(GameObject go, Dictionary<int, ServerGameObject> instanceIDs)
		{
			instanceIDs.Add(go.GetInstanceID(), this);

			this.gameObject = go;
			this.instanceID = this.gameObject.GetInstanceID();

			Transform	transform = go.transform;

			this.children = new List<ServerGameObject>(transform.childCount);

			if (transform.childCount > 0)
			{
				for (int i = 0; i < transform.childCount; i++)
					this.children.Add(new ServerGameObject(transform.GetChild(i).gameObject, instanceIDs));
			}
		}

		public void	RefreshChildren(Dictionary<int, ServerGameObject> instanceIDs)
		{
			this.children.Clear();

			Transform	transform = this.gameObject.transform;

			if (transform.childCount > 0)
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					GameObject			child = transform.GetChild(i).gameObject;
					int					instanceID = child.GetInstanceID();
					ServerGameObject	ng;

					if (instanceIDs.TryGetValue(instanceID, out ng) == true)
					{
						ng.RefreshChildren(instanceIDs);
						this.children.Add(ng);
					}
					else
						this.children.Add(new ServerGameObject(child, instanceIDs));
				}
			}
		}

		/// <summary>
		/// Populates field components with Components converted into NGComponents.
		/// </summary>
		public void	ProcessComponents()
		{
			if (this.components == null)
				this.components = new List<ServerComponent>();
			else
				this.components.Clear();

			// Unity 4 does not clear it.
			ServerGameObject.tempListComponents.Clear();

			this.gameObject.GetComponents<Component>(ServerGameObject.tempListComponents);

			for (int i = ServerGameObject.tempListComponents.Count - 1; i >= 0; --i)
				this.components.Add(new ServerComponent(ServerGameObject.tempListComponents[i]));
		}

		public bool	RemoveComponent(int instanceID)
		{
			// If entering this function, components should be already set, no need to check.
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].instanceID == instanceID)
				{
					GameObject.DestroyImmediate(this.components[i].component);
					return this.components[i].component == null && object.ReferenceEquals(this.components[i].component, null) == false;
				}
			}

			return false;
		}

		public ServerComponent	GetComponent(int instanceID)
		{
			// If entering this function, components should be already set, no need to check.
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].instanceID == instanceID)
					return this.components[i];
			}

			return null;
		}
	}
}