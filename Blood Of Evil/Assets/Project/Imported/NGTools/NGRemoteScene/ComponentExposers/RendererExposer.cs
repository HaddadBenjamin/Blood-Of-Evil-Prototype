using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class RendererExposer : ComponentExposer
	{
		public	RendererExposer() : base(typeof(Renderer))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("enabled"),
				this.type.GetProperty("shadowCastingMode"),
				this.type.GetProperty("receiveShadows"),
				this.type.GetProperty("sharedMaterials"),
				this.type.GetProperty("useLightProbes"),
				this.type.GetProperty("reflectionProbeUsage"),
				this.type.GetProperty("probeAnchor")
			};

			return fields;
		}
	}
}