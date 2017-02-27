using System;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	public static class ComponentExposersManager
	{
		private static readonly Dictionary<Type, ComponentExposer>	types;

		static	ComponentExposersManager()
		{
			ComponentExposersManager.types = new Dictionary<Type, ComponentExposer>();

			foreach (Type t in Utility.EachSubClassesOf(typeof(ComponentExposer)))
			{
				ComponentExposer	exposer = Activator.CreateInstance(t, null) as ComponentExposer;
				ComponentExposersManager.types.Add(exposer.type, exposer);
			}
		}

		public static ComponentExposer[]	GetComponentExposers(Type targetType)
		{
			List<ComponentExposer>	list = null;

			foreach (var item in ComponentExposersManager.types)
			{
				if (item.Key == targetType || targetType.IsSubclassOf(item.Key) == true)
				{
					if (list == null)
						list = new List<ComponentExposer>();
					list.Add(item.Value);
				}
			}

			if (list != null)
				return list.ToArray();
			return null;
		}
	}
}