using NGTools.Network;
using NGTools.NGConsole;

namespace NGTools.NGGameConsole
{
	using UnityEngine;

	public class NGServerCommand : BaseServer
	{
		public const float	UDPPingInterval = 3F;
		public const int	UDPPortBroadcastMin = 6600;
		public const int	UDPPortBroadcastMax = 6610;
		public const int	DefaultPort = 17255;

		[Header("[Optional] Allow to execute commands from NG Console.")]
		public NGCLI	cli;

		public new AbstractTcpListener	listener { get { return (AbstractTcpListener)base.listener; } }

		private AutoDetectUDPClient	udpClient;

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

			this.udpClient = new AutoDetectUDPClient(this, this.listener.port, NGServerCommand.UDPPortBroadcastMin, NGServerCommand.UDPPortBroadcastMax, NGServerCommand.UDPPingInterval);
		}

		protected virtual void	OnDisable()
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			Application.RegisterLogCallback(null);
#else
			Application.logMessageReceived -= this.HandleLog;
#endif

			if (this.udpClient != null)
				this.udpClient.Stop();
		}

		private	void	HandleLog(string condition, string stackTrace, LogType type)
		{
			LogPacket	log = new LogPacket(condition, stackTrace, type);

			for (int i = 0; i < this.listener.clients.Count; i++)
				this.listener.clients[i].AddPacket(log);
		}
	}
}