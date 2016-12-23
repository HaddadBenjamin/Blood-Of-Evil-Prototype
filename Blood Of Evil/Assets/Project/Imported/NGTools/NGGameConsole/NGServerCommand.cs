namespace NGTools
{
	using UnityEngine;

	public class NGServerCommand : BaseServer
	{
		public const int	DefaultPort = 17255;

		[Header("[Optional] Allow to execute commands from NG Console.")]
		public NGCLI	cli;

		public new AbstractTcpListener	listener { get { return (AbstractTcpListener)base.listener; } }

		protected override PacketExecuter	CreatePacketExecuter()
		{
			return new ServerCLIExecuter(this.cli);
		}

		protected virtual void	OnEnable()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Application.RegisterLogCallback(this.HandleLog);
#else
			Application.logMessageReceived += this.HandleLog;
#endif
		}

		protected virtual void	OnDisable()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Application.RegisterLogCallback(null);
#else
			Application.logMessageReceived -= this.HandleLog;
#endif
		}

		private	void	HandleLog(string condition, string stackTrace, LogType type)
		{
			LogPacket	log = new LogPacket(condition, stackTrace, type);

			for (int i = 0; i < this.listener.clients.Count; i++)
				this.listener.clients[i].AddPacket(log);
		}
	}
}