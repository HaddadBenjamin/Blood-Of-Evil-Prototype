using System;
using UnityEngine;

namespace NGToolsEditor
{
	public class TransformExclusion : TypeMembersExclusion
	{
		public	TransformExclusion()
		{
			this.exclusions.Add("parent");
		}

		public override bool	CanHandle(Type targetType)
		{
			return typeof(Transform).IsAssignableFrom(targetType);
		}
	}
}