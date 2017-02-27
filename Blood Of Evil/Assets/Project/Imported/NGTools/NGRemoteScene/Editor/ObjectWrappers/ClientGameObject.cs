using NGTools.Network;
using NGTools.NGRemoteScene;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public enum GameObjectReady
	{
		TagAndLayer = 0x1
	}

	public sealed class ClientGameObject
	{
		private const float	MinTimeBetweenRefresh = 5F;

		public static ClientGameObject[]	EmptyGameObjectArray = {};

		private ClientGameObject	parent;
		public ClientGameObject		Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				if (this.parent != null)
					this.parent.children.Remove(this);
				this.parent = value;
				if (this.parent != null)
					this.parent.children.Add(this);
			}
		}

		#region Editor
		public bool					fold = false;
		#endregion

		public bool								active;
		public string							name;
		public readonly int						instanceID;
		public readonly List<ClientGameObject>	children;
		public List<ClientComponent>			components;

		#region Additionnal Data
		public string	tag;
		public int		layer;
		public bool		isStatic;
		#endregion

		private bool	hasSelection;
		/// <summary>
		/// Defines if this game object is selected or contains selected children.
		/// </summary>
		public bool		HasSelection { get {return this.hasSelection; } }

		private bool	selected;
		public bool		Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				if (this.selected != value)
				{
					this.selected = value;
					this.UpdateHasSelection();
				}
			}
		}

		public GameObjectReady	ready { get; private set; }

		private readonly IUnityData	unityData;
		private float				requestingBehaviours = 0F;

		private bool	destroyed;

		public	ClientGameObject()
		{
			this.children = new List<ClientGameObject>();
		}

		public	ClientGameObject(ClientGameObject parent, NetGameObject remote, IUnityData unityData)
		{
			this.unityData = unityData;
			this.parent = parent;
			this.active = remote.active;
			this.name = remote.name;
			this.instanceID = remote.instanceID;

			int	length = remote.children.Length;

			this.children = new List<ClientGameObject>(remote.children.Length);
			for (int i = 0; i < length; i++)
				this.children.Add(new ClientGameObject(this, remote.children[i], this.unityData));
		}

		public void	Destroy()
		{
			this.Selected = false;
			this.Parent = null;
			this.destroyed = true;
		}

		public void	UpdateData(NetGameObjectData data)
		{
			this.tag = data.tag;
			this.layer = data.layer;
			this.isStatic = data.isStatic;

			if (this.components == null)
				this.components = new List<ClientComponent>();
			else
				this.components.Clear();

			for (int i = 0; i < data.components.Length ; i++)
				this.components.Add(new ClientComponent(this, data.components[i], this.unityData));

			this.ready |= GameObjectReady.TagAndLayer;
		}

		public void	RequestBehaviours(Client client)
		{
			if (this.components == null && this.requestingBehaviours < Time.realtimeSinceStartup)
			{
				this.requestingBehaviours = Time.realtimeSinceStartup + ClientGameObject.MinTimeBetweenRefresh;
				client.AddPacket(new ClientRequestGameObjectDataPacket(this.instanceID));
			}
		}

		public void	AddComponent(NetComponent netComponent)
		{
			this.components.Add(new ClientComponent(this, netComponent, this.unityData));
		}

		/// <summary></summary>
		/// <param name="instanceID"></param>
		/// <exception cref="NGTools.NGRemoteScene.MissingComponentException">Thrown when there is no component with the given instanceID.</exception>
		public void	RemoveComponent(int instanceID)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].instanceID == instanceID)
				{
					this.components.RemoveAt(i);
					return;
				}
			}

			throw new NGTools.NGRemoteScene.MissingComponentException(instanceID);
		}

		public ClientComponent	GetComponent(int instanceID)
		{
			if (this.components == null)
				return null;

			// If entering this function, components should be already set, no need to check.
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i].instanceID == instanceID)
					return this.components[i];
			}

			return null;
		}


		public void	ClearSelection()
		{
			this.selected = false;
			this.hasSelection = false;

			for (int i = 0; i < this.children.Count; i++)
				this.children[i].ClearSelection();
		}

		public int	GetSiblingIndex()
		{
			return this.parent.children.IndexOf(this);
		}

		public void	SetSiblingIndex(int index)
		{
			this.parent.children.Remove(this);

			if (index > this.parent.children.Count)
				index = this.parent.children.Count;
			this.parent.children.Insert(index, this);
		}

		private void	UpdateHasSelection()
		{
			this.hasSelection = this.selected;

			// If not selected, look into children.
			if (this.hasSelection == false)
			{
				for (int i = 0; i < this.children.Count; i++)
				{
					if (this.children[i].hasSelection == true)
					{
						this.hasSelection = true;
						break;
					}
				}
			}

			// Propagate through hierarchy.
			if (this.Parent != null)
				this.Parent.UpdateHasSelection();
		}

		public static implicit operator bool(ClientGameObject exists)
		{
			return exists.destroyed == false;
		}
	}
}