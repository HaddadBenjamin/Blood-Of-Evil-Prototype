using System.Collections.Generic;
using UnityEngine;

namespace NGTools
{
	public class MonitorMaterialVector2 : MonitorData
	{
		private Material								material;
		private NGShaderProperty						propertyInfo;
		private ClientUpdateMaterialVector2Packet.Type	type;

		public	MonitorMaterialVector2(Material material, NGShaderProperty propertyInfo, ClientUpdateMaterialVector2Packet.Type type) : base(null, null)
		{
			this.material = material;
			this.propertyInfo = propertyInfo;
			this.type = type;

			if (this.type == ClientUpdateMaterialVector2Packet.Type.Offset)
				this.value = this.material.GetTextureOffset(this.propertyInfo.name);
			else if (this.type == ClientUpdateMaterialVector2Packet.Type.Scale)
				this.value = this.material.GetTextureScale(this.propertyInfo.name);
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.material.HasProperty(this.propertyInfo.name) == false)
			{
				updates.Add(this);
				this.ToDelete = true;
				return;
			}

			if (this.type == ClientUpdateMaterialVector2Packet.Type.Offset)
			{
				if (object.Equals(this.value, this.material.GetTextureOffset(this.propertyInfo.name)) == false)
					updates.Add(this);
			}
			else if (this.type == ClientUpdateMaterialVector2Packet.Type.Scale)
			{
				if (object.Equals(this.value, this.material.GetTextureScale(this.propertyInfo.name)) == false)
					updates.Add(this);
			}
		}

		public override void	Update()
		{
			if (this.ToDelete == true)
				return;

			if (this.type == ClientUpdateMaterialVector2Packet.Type.Offset)
				this.value = this.material.GetTextureOffset(this.propertyInfo.name);
			else if (this.type == ClientUpdateMaterialVector2Packet.Type.Scale)
				this.value = this.material.GetTextureScale(this.propertyInfo.name);
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerUpdateMaterialVector2Packet(this.material.GetInstanceID(), this.propertyInfo.name, (Vector2)this.value, this.type) };
		}
	}
}