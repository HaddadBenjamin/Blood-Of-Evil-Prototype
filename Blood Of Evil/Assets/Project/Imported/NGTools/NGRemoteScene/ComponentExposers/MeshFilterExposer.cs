using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public class MeshFilterExposer : ComponentExposer
	{
		public	MeshFilterExposer() : base(typeof(MeshFilter))
		{
		}

		public override PropertyInfo[]	GetPropertyInfos()
		{
			PropertyInfo[]	fields = new PropertyInfo[] {
				this.type.GetProperty("sharedMesh")
			};

			return fields;
		}
	}
}