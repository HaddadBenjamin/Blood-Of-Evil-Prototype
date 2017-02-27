using NGTools.Network;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorMaterialProperty : MonitorData
	{
		private Material			material;
		private NGShaderProperty	propertyInfo;
		private TypeHandler			typeHandler;

		public	MonitorMaterialProperty(Material material, NGShaderProperty propertyInfo) : base(null, null)
		{
			this.material = material;
			this.propertyInfo = propertyInfo;

			switch (this.propertyInfo.type)
			{
				case NGShader.ShaderPropertyType.Color:
					this.typeHandler = TypeHandlersManager.GetTypeHandler(typeof(Color));
					this.value = this.material.GetColor(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.Float:
				case NGShader.ShaderPropertyType.Range:
					this.typeHandler = TypeHandlersManager.GetTypeHandler(typeof(float));
					this.value = this.material.GetFloat(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.TexEnv:
					this.typeHandler = TypeHandlersManager.GetTypeHandler(typeof(Texture));
					this.value = this.material.GetTexture(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.Vector:
					this.typeHandler = TypeHandlersManager.GetTypeHandler(typeof(Vector4));
					this.value = this.material.GetVector(this.propertyInfo.name);
					break;
			}
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.material.HasProperty(this.propertyInfo.name) == false)
			{
				updates.Add(this);
				this.ToDelete = true;
				return;
			}

			switch (this.propertyInfo.type)
			{
				case NGShader.ShaderPropertyType.Color:
					if (object.Equals(this.value, this.material.GetColor(this.propertyInfo.name)) == false)
						updates.Add(this);
					break;
				case NGShader.ShaderPropertyType.Float:
				case NGShader.ShaderPropertyType.Range:
					if (object.Equals(this.value, this.material.GetFloat(this.propertyInfo.name)) == false)
						updates.Add(this);
					break;
				case NGShader.ShaderPropertyType.TexEnv:
					if (object.Equals(this.value, this.material.GetTexture(this.propertyInfo.name)) == false)
						updates.Add(this);
					break;
				case NGShader.ShaderPropertyType.Vector:
					if (object.Equals(this.value, this.material.GetVector(this.propertyInfo.name)) == false)
						updates.Add(this);
					break;
			}
		}

		public override void	Update()
		{
			if (this.ToDelete == true)
				return;

			switch (this.propertyInfo.type)
			{
				case NGShader.ShaderPropertyType.Color:
					this.value = this.material.GetColor(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.Float:
				case NGShader.ShaderPropertyType.Range:
					this.value = this.material.GetFloat(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.TexEnv:
					this.value = this.material.GetTexture(this.propertyInfo.name);
					break;
				case NGShader.ShaderPropertyType.Vector:
					this.value = this.material.GetVector(this.propertyInfo.name);
					break;
			}
		}

		public override Packet[]	CreateUpdatePackets()
		{
			switch (this.propertyInfo.type)
			{
				case NGShader.ShaderPropertyType.Color:
				case NGShader.ShaderPropertyType.Float:
				case NGShader.ShaderPropertyType.Range:
				case NGShader.ShaderPropertyType.TexEnv:
				case NGShader.ShaderPropertyType.Vector:
					return new Packet[] { new ServerUpdateMaterialPropertyPacket(this.material.GetInstanceID(), this.propertyInfo.name, this.typeHandler.Serialize(this.value)) };
			}

			throw new Exception();
		}
	}
}