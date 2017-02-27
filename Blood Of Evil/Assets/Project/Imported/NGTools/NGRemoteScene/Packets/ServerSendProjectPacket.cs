using NGTools.Network;

namespace NGTools.NGRemoteScene
{
	[PacketLinkTo(PacketId.Scene_ServerSendProject)]
	internal sealed class ServerSendProjectPacket : Packet
	{
		public ListingAssets.AssetReferences[]	assets;

		private	ServerSendProjectPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendProjectPacket(ListingAssets.AssetReferences[] assets)
		{
			this.assets = assets;
		}

		public override void	Out(ByteBuffer buffer)
		{
			bool	error = false;

			buffer.Append(this.assets.Length);

			for (int i = 0; i < this.assets.Length; i++)
			{
				buffer.AppendUnicodeString(this.assets[i].asset);
				buffer.Append(this.assets[i].references.Length);

				for (int j = 0; j < this.assets[i].references.Length; j++)
				{
					if (this.assets[i].references[j] == null)
					{
						error = true;
						InternalNGDebug.LogWarning("Asset \"" + this.assets[i].asset + "\" has a null reference.");
						buffer.Append(0);
						buffer.AppendUnicodeString(null);
					}
					else
					{
						buffer.Append(this.assets[i].references[j].GetInstanceID());
						buffer.AppendUnicodeString(this.assets[i].references[j].GetType().GetShortAssemblyType());
					}
				}
			}

			if (error == true)
				InternalNGDebug.LogWarning("A null asset has been detected. You should refresh \"Embedded Resources\" in NG Server Scene.");
		}

		public override void	In(ByteBuffer buffer)
		{
			int	total = buffer.ReadInt32();

			this.assets = new ListingAssets.AssetReferences[total];

			for (int i = 0; i < total; i++)
			{
				this.assets[i] = new ListingAssets.AssetReferences();
				this.assets[i].asset = buffer.ReadUnicodeString();
				this.assets[i].IDs = new int[buffer.ReadInt32()];
				this.assets[i].types = new string[this.assets[i].IDs.Length];

				for (int j = 0; j < this.assets[i].IDs.Length; j++)
				{
					this.assets[i].IDs[j] = buffer.ReadInt32();
					this.assets[i].types[j] = buffer.ReadUnicodeString();
				}
			}
		}
	}
}