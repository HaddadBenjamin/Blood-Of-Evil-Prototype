using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public sealed class ClientField
	{
		public readonly	Type				fieldType;
		public readonly	string				name;
		public readonly	bool				isPublic;
		public readonly	FieldInnerValueType	innerValueType;

		public object	value;

		public readonly IUnityData	unityData;

		private readonly ClientComponent	parentBehaviour;
		private readonly TypeHandlerDrawer	drawer;
		private readonly DataDrawer			dataDrawer;

		public	ClientField(ClientComponent behaviour, NetField netField, IUnityData unityData)
		{
			this.unityData = unityData;
			this.parentBehaviour = behaviour;

			this.fieldType = netField.fieldType;
			this.name = netField.name;
			this.isPublic = netField.isPublic;
			this.innerValueType = netField.innerValueType;
			this.value = netField.value;

			this.drawer = TypeHandlerDrawersManager.CreateTypeHandlerDrawer(netField.handler, this.fieldType, this.value);
			this.dataDrawer = new DataDrawer(this.unityData);
		}

		public float	GetHeight(NGRemoteInspectorWindow inspector)
		{
			return this.drawer.GetHeight(this.value);
		}

		public void		Draw(Rect r, NGRemoteInspectorWindow inspector)
		{
			this.dataDrawer.Init(this.parentBehaviour.parent.instanceID + "." + this.parentBehaviour.instanceID, inspector);

			this.drawer.Draw(r, this.dataDrawer.DrawChild(this.name, this.value));
		}
	}
}