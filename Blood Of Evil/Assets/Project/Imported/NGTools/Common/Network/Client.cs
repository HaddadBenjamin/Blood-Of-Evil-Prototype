using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

namespace NGTools.Network
{
	public class Client
	{
		public enum BatchMode
		{
			Off,
			On
		}

		public struct ExecutedPacket
		{
			public readonly string	time;
			public readonly Packet	packet;

			public	ExecutedPacket(Packet packet)
			{
				this.time = DateTime.Now.ToString("HH:mm:ss.fff");
				this.packet = packet;
			}
		}

		public class Batch
		{
			public string				name;
			public readonly Packet[]	batchedPackets;

			public	Batch(string name, Packet[] batch)
			{
				this.name = name;
				this.batchedPackets = batch;
			}
		}

		public const int	SendBufferCapacity = 1024;
		public const int	ReadBufferSize = 16384;
		public const int	TempBufferSize = 4048;

		public readonly TcpClient	tcpClient;
		public BatchMode			batchMode;
		public Int64				bytesSent { get; private set; }
		public Int64				bytesReceived { get; private set; }
		public int					PendingPacketsCount { get { return this.pendingPackets.Count; } }

		public readonly List<Packet>	batchedPackets;
		public string[]	batchNames;

		public readonly bool					saveSentPackets;
		public readonly List<string>			receivedPacketsHistoric;
		public readonly List<ExecutedPacket>	sentPacketsHistoric;
		private readonly List<Batch>	batchesHistoric;
		private readonly List<Packet>	pendingPackets;
		private readonly List<Packet>	receivedPackets;

		private readonly ByteBuffer		packetBuffer;
		private readonly ByteBuffer		receiveBuffer;
		private readonly ByteBuffer		fullPacketBuffer;
		private readonly NetworkStream	reader;
		private readonly ByteBuffer		sendBuffer;
		private readonly ByteBuffer		tinySendBuffer;
		private readonly byte[]			tempBuffer;

		private readonly object[]	packetArgument;
		private int					packetId = -1;
		private uint				length = 0;

		private readonly Dictionary<int, Type>	packetTypes;

		public	Client(TcpClient tcpClient, bool save = true)
		{
			this.tcpClient = tcpClient;
			this.batchMode = BatchMode.Off;
			this.reader = this.tcpClient.GetStream();

			this.saveSentPackets = save;
			if (this.saveSentPackets == true)
				this.sentPacketsHistoric = new List<ExecutedPacket>(512);
			this.receivedPacketsHistoric = new List<string>(512);
			this.batchedPackets = new List<Packet>(64);
			this.batchesHistoric = new List<Batch>(4);
			this.pendingPackets = new List<Packet>(4);
			this.receivedPackets = new List<Packet>(4);
			this.batchNames = new string[0];

			this.packetBuffer = new ByteBuffer(Client.ReadBufferSize);
			this.receiveBuffer = new ByteBuffer(Client.ReadBufferSize);
			this.fullPacketBuffer = new ByteBuffer(Client.ReadBufferSize);
			this.sendBuffer = new ByteBuffer(Client.SendBufferCapacity);
			this.tinySendBuffer = new ByteBuffer(Client.SendBufferCapacity);
			this.tempBuffer = new byte[Client.TempBufferSize];

			this.packetArgument = new object[1] { this.packetBuffer };
			this.packetId = -1;

			this.packetTypes = new Dictionary<int, Type>();
			foreach (Type t in Utility.EachSubClassesOf(typeof(Packet)))
			{
				object[]	attributes = t.GetCustomAttributes(typeof(PacketLinkToAttribute), false);

				if (attributes.Length == 0)
					continue;

				if (this.packetTypes.ContainsKey((attributes[0] as PacketLinkToAttribute).packetId) == true)
					InternalNGDebug.LogError("Packet \"" + t.FullName + "\" shares the same ID as \"" + this.packetTypes[(attributes[0] as PacketLinkToAttribute).packetId] + "\".");
				else
					this.packetTypes.Add((attributes[0] as PacketLinkToAttribute).packetId, t);
			}

			if (this.reader.CanRead == true)
				this.reader.BeginRead(this.tempBuffer, 0, this.tempBuffer.Length, new AsyncCallback(this.ReadCallBack), this);
			else
				UnityEngine.Debug.LogError("Client has a non readable NetworkStream.");
		}

		public void	Close()
		{
			this.reader.Close();
			this.tcpClient.Close();
		}

		public void	ReadCallBack(IAsyncResult ar)
		{
			int	bytesCount = this.reader.EndRead(ar);

			lock (this.receiveBuffer)
			{
				this.receiveBuffer.Append(this.tempBuffer, 0, bytesCount);
			}

			lock (this.fullPacketBuffer)
			{
				this.reader.BeginRead(this.tempBuffer, 0, this.tempBuffer.Length, new AsyncCallback(this.ReadCallBack), this);

				if (this.reader.DataAvailable == false)
				{
					lock (this.receiveBuffer)
					{
						this.fullPacketBuffer.Append(this.receiveBuffer);

						this.bytesReceived += this.receiveBuffer.Length;

						this.receiveBuffer.Clear();
					}

					// 8 corresponds to the minimum length for a command.
					for (uint i = (uint)this.fullPacketBuffer.Position; (uint)this.fullPacketBuffer.Length - i >= 8U;)
					{
						if (this.packetId == -1)
						{
							this.packetId = this.fullPacketBuffer.ReadInt32();
							this.length = this.fullPacketBuffer.ReadUInt32();

							i = (uint)this.fullPacketBuffer.Position;
						}

						if (this.length <= this.fullPacketBuffer.Length - this.fullPacketBuffer.Position)
						{
							InternalNGDebug.LogFile("X " + PacketId.GetPacketName(this.packetId) + " (" + this.packetId + ") " + this.length + " B @ " + this.fullPacketBuffer.Position + "/" + this.fullPacketBuffer.Length);

							Type	t;

							if (this.packetTypes.TryGetValue(this.packetId, out t) == true)
							{
								try
								{
									this.fullPacketBuffer.CopyBuffer(this.packetBuffer, (int)i, (int)this.length);
									lock (this.receivedPackets)
									{
										Packet	packet = Activator.CreateInstance(t, BindingFlags.NonPublic | BindingFlags.Instance, null, this.packetArgument, null) as Packet;
										this.receivedPackets.Add(packet);
										this.receivedPacketsHistoric.Add(DateTime.Now.ToString("HH:mm:ss.fff") + " " + packet);
									}
								}
								catch (Exception ex)
								{
									InternalNGDebug.LogFileException("Packet parsing failed: Type: " + t, ex);
									this.fullPacketBuffer.Clear();
								}
							}
							else
							{
								InternalNGDebug.LogFile("Unknown command " + PacketId.GetPacketName(this.packetId) + " (" + this.packetId + ") of " + this.length + " chars.");
							}

							i += this.length;
							this.fullPacketBuffer.Position = (int)i;

							this.packetId = -1;

							if (this.fullPacketBuffer.Length == this.fullPacketBuffer.Position)
								this.fullPacketBuffer.Clear();
						}
						else
						{
							InternalNGDebug.LogFile("X... " + PacketId.GetPacketName(this.packetId) + " (" + this.packetId + ") " + this.length + " B @ " + this.fullPacketBuffer.Position + "/" + this.fullPacketBuffer.Length);
							break;
						}
					}
				}
			}
		}

		public void	Write()
		{
			if (this.pendingPackets.Count == 0)
				return;

			InternalNGDebug.LogFile("W " + this.pendingPackets.Count + " packet(s).");

			for (int i = 0; i < this.pendingPackets.Count; i++)
			{
				this.tinySendBuffer.Clear();
				this.pendingPackets[i].Out(this.tinySendBuffer);

				InternalNGDebug.LogFile("W " + PacketId.GetPacketName(this.pendingPackets[i].packetId) + " (" + this.pendingPackets[i].packetId + ") " + (uint)this.tinySendBuffer.Length + " B.");
				this.sendBuffer.Append(this.pendingPackets[i].packetId);
				this.sendBuffer.Append((uint)this.tinySendBuffer.Length);

				this.sendBuffer.Append(this.tinySendBuffer);
			}

			byte[]	buffer = this.sendBuffer.Flush();
			this.reader.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(this.WriteCallBack), this);

			this.bytesSent += buffer.Length;

			// Only add to historic after they are all buffered and sent. In case of raising exception.
			if (this.saveSentPackets == true)
			{
				for (int i = 0; i < this.pendingPackets.Count; i++)
					this.sentPacketsHistoric.Add(new ExecutedPacket(this.pendingPackets[i]));
			}

			this.pendingPackets.Clear();
		}

		public void	WriteCallBack(IAsyncResult ar)
		{
			this.reader.EndWrite(ar);
		}

		/// <summary>
		/// Executes commands received during the last frames. Is called in Update, thus the main-thread context is guaranteed.
		/// </summary>
		/// <param name="executer"></param>
		public void	ExecReceivedCommands(PacketExecuter executer)
		{
			lock (this.receivedPackets)
			{
				if (this.receivedPackets.Count == 0)
					return;

				for (int j = 0; j < this.receivedPackets.Count; j++)
				{
					try
					{
						executer.ExecutePacket(this, this.receivedPackets[j]);
					}
					catch (Exception ex)
					{
						InternalNGDebug.LogException(ex);
					}
				}

				this.receivedPackets.Clear();
			}
		}

		public void	AddPacket(Packet packet)
		{
			if (this.batchMode == BatchMode.On && packet.isBatchable == true)
			{
				for (int i = 0; i < this.batchedPackets.Count; i++)
				{
					if (packet.AggregateInto(this.batchedPackets[i]) == true)
						return;
				}

				this.batchedPackets.Add(packet);
				return;
			}

			for (int i = 0; i < this.pendingPackets.Count; i++)
			{
				if (packet.AggregateInto(this.pendingPackets[i]) == true)
					return;
			}

			this.pendingPackets.Add(packet);
		}

		public void	SaveBatch(string name)
		{
			if (this.batchedPackets.Count > 0)
			{
				this.batchesHistoric.Add(new Batch(name, this.batchedPackets.ToArray()));

				this.batchNames = new string[this.batchesHistoric.Count];

				for (int i = 0; i < this.batchesHistoric.Count; i++)
					this.batchNames[i] = this.batchesHistoric[i].name + " (" + this.batchesHistoric[i].batchedPackets.Length + ")";
			}
		}

		public void	ExecuteBatch()
		{
			if (this.batchedPackets.Count > 0)
			{
				for (int i = 0; i < this.batchedPackets.Count; i++)
					this.pendingPackets.Add(this.batchedPackets[i]);

				this.batchedPackets.Clear();
			}
		}

		public void	LoadBatch(int i)
		{
			if (0 <= i && i < this.batchesHistoric.Count)
			{
				this.batchedPackets.Clear();

				for (int j = 0; j < this.batchesHistoric[i].batchedPackets.Length; j++)
					this.batchedPackets.Add(this.batchesHistoric[i].batchedPackets[j]);
			}
		}

		public override string	ToString()
		{
			return "FPL=" + this.fullPacketBuffer.Position + "/" + this.fullPacketBuffer.Length + " BRecv=" + this.bytesReceived + " PP=" + this.pendingPackets.Count;
		}
	}
}