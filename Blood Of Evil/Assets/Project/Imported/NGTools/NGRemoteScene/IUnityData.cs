using System;

namespace NGTools
{
	/// <summary>Provides resources and tools to TypeHandlerDrawer.</summary>
	public interface IUnityData
	{
		Client		Client { get; }
		string[]	Layers { get; }
		void		GetResources(Type type, out string[] resourceNames, out int[] resourceInstanceIds);
		string		GetGameObjectName(int instanceID);
		string		GetBehaviourName(int gameObjectInstanceID, int instanceID);
		string		GetResourceName(Type type, int instanceID);
	}
}