using System.Collections.Generic;
using System.Reflection;

namespace NGTools.Network
{
	/// <summary>
	/// A partial class to gather in one place your packets' ID.
	/// </summary>
	internal partial class PacketId
	{
		public const int	ServerHasDisconnect = 1;
		public const int	ClientHasDisconnect = 2;
		public const int	Server_ErrorNotification = 3;

		private static Dictionary<int, string>	packetNames;

		public static string	GetPacketName(int id)
		{
			if (packetNames == null)
			{
				packetNames = new Dictionary<int, string>();

				FieldInfo[]	fis = typeof(PacketId).GetFields();

				for (int i = 0; i < fis.Length; i++)
					packetNames.Add((int)fis[i].GetRawConstantValue(), fis[i].Name);
			}

			return packetNames[id];
		}
	}
}