using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// <para>Requests all primary data from a GameObject.</para>
	/// <para>That includes:</para>
	/// <list type="bullet">
	/// <item><description></description></item>
	/// <item><description>Tag</description></item>
	/// <item><description>Layer</description></item>
	/// <item><description>IsStatic</description></item>
	/// <item><description>Behaviours</description></item>
	/// <item><description>Behaviours' fields</description></item>
	/// <item><description>Behaviours' methods</description></item>
	/// </list>
	/// </summary>
	/// <seealso cref="NGTools.ServerSendGameObjectDataPacket"/>
	[PacketLinkTo(PacketId.Scene_ClientRequestGameObjectData)]
	internal sealed class ClientRequestGameObjectDataPacket : Packet
	{
		public int	gameObjectInstanceID;

		private	ClientRequestGameObjectDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientRequestGameObjectDataPacket(int gameObjectInstanceId)
		{
			this.gameObjectInstanceID = gameObjectInstanceId;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			GUILayout.Label("Requesting data from GameObject \"" + unityData.GetGameObjectName(this.gameObjectInstanceID) + "\".");
		}
	}
}