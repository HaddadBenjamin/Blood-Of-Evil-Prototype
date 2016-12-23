using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class TransformExposer : ComponentExposer
	{
		public	TransformExposer() : base(typeof(Transform))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("localPosition"),
				this.type.GetProperty("localEulerAngles"),
				this.type.GetProperty("localScale")
			};

			return fields;
		}
	}
}