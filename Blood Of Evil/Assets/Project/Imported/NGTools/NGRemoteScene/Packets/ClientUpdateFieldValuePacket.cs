using NGTools.Network;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ClientUpdateFieldValue, true)]
	internal sealed class ClientUpdateFieldValuePacket : Packet
	{
		public string		fieldPath;
		public byte[]		rawValue;

		private TypeHandler	deserializer;
		private string		cachedGUI;

		private	ClientUpdateFieldValuePacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ClientUpdateFieldValuePacket(string fieldPath, byte[] rawValue, TypeHandler deserializer)
		{
			this.fieldPath = fieldPath;
			this.rawValue = rawValue;
			this.deserializer = deserializer;
		}

		public override void	OnGUI(IUnityData unityData)
		{
			if (this.cachedGUI == null)
			{
				string[]	paths = this.fieldPath.Split('.');

				try
				{
					int	gameObjectInstanceID = int.Parse(paths[0]);
					int	instanceID;

					paths[0] = unityData.GetGameObjectName(gameObjectInstanceID);

					if (int.TryParse(paths[1], out instanceID) == true)
						paths[1] = unityData.GetBehaviourName(gameObjectInstanceID, instanceID);

					if (this.deserializer == null)
						this.cachedGUI = "Updating " + string.Join(".", paths) + ".";
					else
						this.cachedGUI = "Updating " + string.Join(".", paths) + " (" + this.deserializer.Deserialize(new ByteBuffer(this.rawValue), null) + ").";
				}
				catch
				{
					if (this.deserializer == null)
						this.cachedGUI = "Updating " + this.fieldPath + ".";
					else
						this.cachedGUI = "Updating " + this.fieldPath + " (" + this.deserializer.Deserialize(new ByteBuffer(this.rawValue), null) + ").";
				}
			}

			GUILayout.Label(this.cachedGUI);
		}

		public override bool	AggregateInto(Packet pendingPacket)
		{
			ClientUpdateFieldValuePacket	a = pendingPacket as ClientUpdateFieldValuePacket;

			if (a != null && a.fieldPath.Equals(this.fieldPath) == true)
			{
				a.rawValue = this.rawValue;
				a.cachedGUI = null;
				return true;
			}

			return false;
		}
	}
}