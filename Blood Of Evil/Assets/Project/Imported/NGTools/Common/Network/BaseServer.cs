using System.Collections.Generic;
using UnityEngine;

namespace NGTools.Network
{
	public abstract class BaseServer : MonoBehaviour
	{
		private static List<BaseServer>	instances = new List<BaseServer>();

		[Header("Starts the server when awaking.")]
		public bool	autoStart = true;
		[Header("Keep the server alive between scenes.")]
		public bool	dontDestroyOnLoad = true;

		[Header("[Required] A listener to communicate via network.")]
		public NetworkListener	listener;

		public PacketExecuter	executer { get; private set; }

		protected virtual void	Awake()
		{
			if (this.executer != null)
				return;

			for (int i = 0; i < BaseServer.instances.Count; i++)
			{
				if (BaseServer.instances[i].GetType() == this.GetType())
				{
					Object.Destroy(this.gameObject);
					return;
				}
			}

			this.executer = this.CreatePacketExecuter();

			if (this.dontDestroyOnLoad == true)
			{
				BaseServer.instances.Add(this);
				Object.DontDestroyOnLoad(this.transform.root.gameObject);
			}
		}

		protected virtual void	Start()
		{
			if (this.listener == null)
				Debug.LogError("A NetworkListener is required.", this);
			else
			{
				this.listener.SetServer(this);

				if (this.autoStart == true)
					this.StartServer();
			}
		}

		protected virtual void	OnDestroy()
		{
			if (this.listener != null)
			{
				this.listener.StopServer();
				this.listener = null;
			}
		}

		public void	StartServer()
		{
			this.listener.StartServer();
		}

		public void	StopServer()
		{
			this.OnDestroy();
		}

		protected abstract PacketExecuter	CreatePacketExecuter();
	}
}