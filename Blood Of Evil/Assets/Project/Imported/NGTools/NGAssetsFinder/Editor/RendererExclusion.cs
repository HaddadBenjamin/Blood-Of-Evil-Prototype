using System;
using UnityEngine;

namespace NGToolsEditor.NGAssetsFinder
{
	internal sealed class RendererExclusion : TypeMembersExclusion
	{
		public	RendererExclusion()
		{
			this.exclusions.Add("material");
			this.exclusions.Add("materials");
		}

		public override bool	CanHandle(Type targetType)
		{
			return typeof(Renderer).IsAssignableFrom(targetType);
		}
	}
}