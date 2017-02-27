using NGTools.Network;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorMaterial : MonitorData
	{
		private static List<MonitorData>	updatedData = new List<MonitorData>();

		private Material	material;
		private Shader		workingShader;

		public	MonitorMaterial(Material gameObject, NGShader shader) : base(gameObject.GetInstanceID().ToString(), () => gameObject)
		{
			this.material = gameObject;
			this.workingShader = this.material.shader;

			this.children = new List<MonitorData>(shader.properties.Length);

			this.MonitorChildren(shader);
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.material == null)
			{
				updates.Add(this);
				return;
			}

			if (this.workingShader != this.material.shader)
			{
				this.workingShader = this.material.shader;
				this.children.Clear();

				updates.Add(this);

				if (this.material.shader != null)
				{
					NGServerScene	scene = Object.FindObjectOfType<NGServerScene>();
					NGShader		shader = scene.GetNGShader(this.material.shader);

					this.MonitorChildren(shader);
				}

				return;
			}

			// Remove properties if the shader has changed.
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].ToDelete == true)
					this.children.RemoveAt(i--);
			}

			for (int i = 0; i < this.children.Count; i++)
				this.children[i].CollectUpdates(updates);
		}

		public override void	Update()
		{
		}

		public override Packet[]	CreateUpdatePackets()
		{
			NGServerScene	scene = Object.FindObjectOfType<NGServerScene>();
			NGShader		shader = scene.GetNGShader(this.material.shader);

			if (shader != null)
				return new Packet[] { new ServerSendMaterialDataPacket(this.material, shader) };
			return null;
		}

		public void	UpdateValues(List<Client> clients)
		{
			MonitorMaterial.updatedData.Clear();

			this.CollectUpdates(MonitorMaterial.updatedData);

			for (int i = 0; i < MonitorMaterial.updatedData.Count; i++)
			{
				MonitorMaterial.updatedData[i].Update();

				Packet[]	packets = MonitorMaterial.updatedData[i].CreateUpdatePackets();
				if (packets == null)
					continue;

				for (int j = 0; j < clients.Count; j++)
				{
					for (int k = 0; k < packets.Length; k++)
						clients[j].AddPacket(packets[k]);
				}
			}
		}

		private void	MonitorChildren(NGShader shader)
		{
			for (int i = 0; i < shader.properties.Length; i++)
			{
				if (shader.properties[i].type == NGShader.ShaderPropertyType.TexEnv)
				{
					this.children.Add(new MonitorMaterialVector2(this.material, shader.properties[i], MaterialVector2Type.Offset));
					this.children.Add(new MonitorMaterialVector2(this.material, shader.properties[i], MaterialVector2Type.Scale));
				}

				this.children.Add(new MonitorMaterialProperty(this.material, shader.properties[i]));
			}
		}
	}
}