using NGTools.Network;
using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	public abstract class TypeHandlerDrawer
	{
		protected readonly TypeHandler	typeHandler;

		protected	TypeHandlerDrawer(TypeHandler typeHandler)
		{
			this.typeHandler = typeHandler;
		}

		public virtual float	GetHeight(object value)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		public abstract void	Draw(Rect r, DataDrawer data);

		/// <summary>
		/// Sends update packet.
		/// </summary>
		/// <param name="unityData"></param>
		/// <param name="valuePath"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		protected void	AsyncUpdateCommand(IUnityData unityData, string valuePath, object value, Type type)
		{
			unityData.AddPacket(new ClientUpdateFieldValuePacket(valuePath, this.typeHandler.Serialize(type, value), this.typeHandler));
		}

		protected void	AsyncUpdateCommand(IUnityData unityData, string valuePath, object value, Type type, TypeHandler customTypeHandler)
		{
			unityData.AddPacket(new ClientUpdateFieldValuePacket(valuePath, customTypeHandler.Serialize(type, value), customTypeHandler));
		}
	}
}