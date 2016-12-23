using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class TrailRendererExposer : ComponentExposer
	{
		public	TrailRendererExposer() : base(typeof(TrailRenderer))
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
				this.type.GetProperty("probeAnchor"),
				this.type.GetProperty("time"),
				this.type.GetProperty("startWidth"),
				this.type.GetProperty("endWidth"),
				this.type.GetProperty("autodestruct")
			};

			return fields;
		}
	}
}