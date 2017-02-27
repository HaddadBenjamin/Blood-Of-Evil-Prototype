using NGTools.Network;
using NGTools.NGConsole;
using NGTools.NGGameConsole;

namespace NGToolsEditor.NGConsole
{
	internal sealed class ClientCLIExecuter : PacketExecuter
	{
		private RemoteModule	logClientModule;

		public	ClientCLIExecuter(RemoteModule logClientModule)
		{
			this.logClientModule = logClientModule;

			this.HandlePacket(PacketId.Logger_ServerSendLog, this.Handle_Logger_ServerSendLog);
			this.HandlePacket(PacketId.CLI_ServerSendCommandNodes, this.Handle_CLI_ServerSendCommandNodes);
			this.HandlePacket(PacketId.CLI_ServerSendCommandResponse, this.Handle_CLI_ServerSendCommandResponse);
		}

		private void	Handle_Logger_ServerSendLog(Client sender, Packet command)
		{
			LogPacket	log = command as LogPacket;

			// Simulate a Log Entry.
			LogEntry	logEntry = new LogEntry();
			logEntry.condition = log.condition + "\n" + log.stackTrace;

			if (log.logType == UnityEngine.LogType.Log)
				logEntry.mode = Mode.ScriptingLog | Mode.MayIgnoreLineNumber;
			else if (log.logType == UnityEngine.LogType.Warning)
				logEntry.mode = Mode.ScriptingWarning | Mode.MayIgnoreLineNumber;
			else if (log.logType == UnityEngine.LogType.Error)
				logEntry.mode = Mode.ScriptingError | Mode.MayIgnoreLineNumber;
			else if (log.logType == UnityEngine.LogType.Exception ||
						log.logType == UnityEngine.LogType.Assert)
			{
				logEntry.mode = Mode.ScriptingError | Mode.ScriptingException | Mode.Log;
			}

			if (string.IsNullOrEmpty(log.stackTrace))
				logEntry.mode |= Mode.DontExtractStacktrace;
			else
			{
				int	fileStart = log.stackTrace.IndexOf(") (at ");

				if (fileStart != -1)
				{
					int	comma = log.stackTrace.IndexOf(':', fileStart);

					if (comma != -1)
					{
						int	par = log.stackTrace.IndexOf(')', comma);

						if (par != -1)
						{
							logEntry.file = log.stackTrace.Substring(fileStart + 6, comma - fileStart - 6);
							logEntry.line = int.Parse(log.stackTrace.Substring(comma + 1, par - comma - 1));
						}
					}
				}
			}

			DefaultRow	row = new DefaultRow();
			row.Init(this.logClientModule.console, logEntry);

			this.logClientModule.AddRow(row);
		}

		private void	Handle_CLI_ServerSendCommandNodes(Client sender, Packet command)
		{
			ServerSendCommandNodesPacket	serverSendCommandNodes = command as ServerSendCommandNodesPacket;

			this.logClientModule.parser.SetRoot(serverSendCommandNodes.root);
			this.logClientModule.console.Repaint();
		}

		private void	Handle_CLI_ServerSendCommandResponse(Client sender, Packet command)
		{
			ServerSendCommandResponsePacket	serverSendCommandResponse = command as ServerSendCommandResponsePacket;

			for (int i = 0; i < this.logClientModule.pendingCommands.Count; i++)
			{
				if (this.logClientModule.pendingCommands[i].id == serverSendCommandResponse.requestId)
				{
					if (serverSendCommandResponse.returnValue == ExecResult.Success)
						this.logClientModule.pendingCommands[i].row.result = serverSendCommandResponse.response;
					else
					{
						this.logClientModule.pendingCommands[i].row.error = serverSendCommandResponse.response;
					}

					this.logClientModule.pendingCommands.RemoveAt(i);
					break;
				}
			}

			this.logClientModule.console.Repaint();
		}
	}
}