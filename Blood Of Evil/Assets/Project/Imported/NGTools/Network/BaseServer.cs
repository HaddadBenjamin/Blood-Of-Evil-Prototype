using UnityEngine;

namespace NGTools
{
	public abstract class BaseServer : MonoBehaviour
	{
		[Header("Starts the server when awaking.")]
		public bool	autoStart = true;

		[Header("[Required] A listener to communicate via network.")]
		public NetworkListener	listener;

		public PacketExecuter	executer { get; private set; }

		protected virtual void	Awake()
		{
			this.executer = this.CreatePacketExecuter();
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