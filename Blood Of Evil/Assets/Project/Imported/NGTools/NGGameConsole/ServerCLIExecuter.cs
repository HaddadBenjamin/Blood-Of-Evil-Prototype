using NGTools.Network;
using NGTools.NGConsole;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	internal class ServerCLIExecuter : PacketExecuter
	{
		private NGCLI	cli;

		public	ServerCLIExecuter(NGCLI cli)
		{
			this.cli = cli;

			this.HandlePacket(PacketId.Logger_ServerSendLog, this.HandleLogPacket);
			this.HandlePacket(PacketId.CLI_ClientRequestCommandNodes, this.HandleClientRequestCommandNodesPacket);
			this.HandlePacket(PacketId.CLI_ClientSendCommand, this.HandleClientSendCommandPacket);
		}

		private void	HandleLogPacket(Client sender, Packet command)
		{
			LogPacket	log = command as LogPacket;

			Debug.Log(log.condition + " " + log.stackTrace + " " + log.packetId);
		}

		private void	HandleClientRequestCommandNodesPacket(Client sender, Packet command)
		{
			if (this.cli == null)
				Debug.Log("Command has been requested but there is no CommandLineInterpreter. Aborted.");
			else
			{
				RemoteCommand					remoteCommand = new RemoteCommand(cli.parser.root);
				ServerSendCommandNodesPacket	response = new ServerSendCommandNodesPacket(remoteCommand);

				sender.AddPacket(response);
			}
		}

		private void	HandleClientSendCommandPacket(Client sender, Packet command)
		{
			if (cli == null)
				Debug.Log("Command has been requested but there is no CommandLineInterpreter. Aborted.");
			else
			{
				ClientSendCommandPacket	clientSendCommand = command as ClientSendCommandPacket;
				string					result = string.Empty;
				ExecResult				returnValue = cli.parser.Exec(clientSendCommand.command, ref result);

				sender.AddPacket(new ServerSendCommandResponsePacket(clientSendCommand.requestId, returnValue, result));
			}
		}
	}
}