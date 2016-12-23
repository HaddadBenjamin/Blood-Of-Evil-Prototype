using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class ColliderExposer : ComponentExposer
	{
		public	ColliderExposer() : base(typeof(Collider))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("enabled"),
				this.type.GetProperty("isTrigger"),
				this.type.GetProperty("sharedMaterial")
			};

			return fields;
		}
	}
}