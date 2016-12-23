using UnityEngine;

namespace NGTools
{
	[PacketLinkTo(PacketId.Scene_ServerSendMaterialData)]
	public class ServerSendMaterialDataPacket : Packet
	{
		public readonly Material	serverMaterial;
		public readonly NGShader	ngShader;
		public NetMaterial		netMaterial;

		protected	ServerSendMaterialDataPacket(ByteBuffer buffer) : base(buffer)
		{
		}

		public	ServerSendMaterialDataPacket(Material material, NGShader ngShader)
		{
			this.serverMaterial = material;
			this.ngShader = ngShader;
		}

		public override void	Out(ByteBuffer buffer)
		{
			NetMaterial.Serialize(this.serverMaterial, this.ngShader, buffer);
		}

		public override void	In(ByteBuffer buffer)
		{
			this.netMaterial = NetMaterial.Deserialize(buffer);
		}
	}
}