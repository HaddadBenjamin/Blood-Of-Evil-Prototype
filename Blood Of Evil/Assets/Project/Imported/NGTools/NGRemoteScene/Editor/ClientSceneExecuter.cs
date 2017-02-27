using NGTools;
using NGTools.Network;
using NGTools.NGRemoteScene;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class ClientSceneExecuter : PacketExecuter
	{
		public NGRemoteHierarchyWindow	hierarchy;

		public	ClientSceneExecuter(NGRemoteHierarchyWindow hierarchy)
		{
			this.hierarchy = hierarchy;

			this.HandlePacket(PacketId.ServerHasDisconnect, this.Handle_Scene_HasDisconnected);
			this.HandlePacket(PacketId.Server_ErrorNotification, this.Handle_Scene_ErrorNotificationPacket);

			this.HandlePacket(PacketId.Scene_ServerSendHierarchy, this.Handle_Scene_ServerSendHierarchy);
			this.HandlePacket(PacketId.Scene_ServerSendLayers, this.Handle_Scene_ServerSendLayers);
			this.HandlePacket(PacketId.Scene_ServerSendResources, this.Handle_Scene_ServerSendResources);

			this.HandlePacket(PacketId.Scene_ServerSendGameObjectData, this.Handle_Scene_ServerSendGameObjectData);
			this.HandlePacket(PacketId.Scene_ServerUpdateFieldValue, this.Handle_Scene_ServerUpdateFieldValue);
			this.HandlePacket(PacketId.Scene_ServerDeleteGameObjects, this.Handle_Scene_ServerDeleteGameObjects);
			this.HandlePacket(PacketId.Scene_ServerDeleteComponents, this.Handle_Scene_ServerDeleteComponents);
			this.HandlePacket(PacketId.Scene_ServerSendMaterialData, this.Handle_Scene_ServerSendMaterialData);
			this.HandlePacket(PacketId.Scene_ServerUpdateMaterialProperty, this.Handle_Scene_ServerUpdateMaterialProperty);
			this.HandlePacket(PacketId.Scene_ServerUpdateMaterialVector2, this.Handle_Scene_ServerUpdateMaterialVector2);
			this.HandlePacket(PacketId.Scene_ServerSendEnumData, this.Handle_Scene_ServerSendEnumData);
			this.HandlePacket(PacketId.Scene_ServerSendComponent, this.Handle_Scene_ServerSendComponent);
			this.HandlePacket(PacketId.Scene_ServerReturnInvokeResult, this.Handle_Scene_ServerReturnInvokeResult);
		}

		private void	Handle_Scene_HasDisconnected(Client sender, Packet _packet)
		{
			this.hierarchy.CloseClient();
		}

		private void	Handle_Scene_ErrorNotificationPacket(Client sender, Packet _packet)
		{
			ErrorNotificationPacket	packet = _packet as ErrorNotificationPacket;

			InternalNGDebug.Log(packet.error, packet.message);
		}

		private void	Handle_Scene_ServerSendHierarchy(Client sender, Packet _packet)
		{
			ServerSendHierarchyPacket	packet = _packet as ServerSendHierarchyPacket;

			this.hierarchy.SetRootGameObjects(packet.clientRoot);
			this.hierarchy.Repaint();
		}

		private void	Handle_Scene_ServerSendLayers(Client sender, Packet _packet)
		{
			ServerSendLayersPacket	packet = _packet as ServerSendLayersPacket;

			this.hierarchy.SetLayers(packet.layers);
		}

		private void	Handle_Scene_ServerSendResources(Client sender, Packet _packet)
		{
			ServerSendResourcesPacket	packet = _packet as ServerSendResourcesPacket;

			this.hierarchy.SetResources(packet.type, packet.resourceNames, packet.instanceIDs);
		}

		private void	Handle_Scene_ServerSendGameObjectData(Client sender, Packet _packet)
		{
			ServerSendGameObjectDataPacket	packet = _packet as ServerSendGameObjectDataPacket;

			ClientGameObject	n = this.hierarchy.GetGameObject(packet.gameObjectData.gameObjectInstanceID);

			if (n != null)
				n.UpdateData(packet.gameObjectData);
			else
				InternalNGDebug.Log(Errors.Scene_GameObjectNotFound, "GameObject (" + packet.gameObjectData.gameObjectInstanceID + ") was not found. Failed to update its data.");
		}

		private void	Handle_Scene_ServerUpdateFieldValue(Client sender, Packet _packet)
		{
			ServerUpdateFieldValuePacket	packet = _packet as ServerUpdateFieldValuePacket;

			this.hierarchy.UpdateFieldValue(packet.fieldPath, packet.rawValue);
		}

		private void	Handle_Scene_ServerDeleteGameObjects(Client sender, Packet _packet)
		{
			ServerDeleteGameObjectsPacket	packet = _packet as ServerDeleteGameObjectsPacket;

			this.hierarchy.DeleteGameObjects(packet.instanceIDs);
		}

		private void	Handle_Scene_ServerDeleteComponents(Client sender, Packet _packet)
		{
			ServerDeleteComponentsPacket	packet = _packet as ServerDeleteComponentsPacket;

			this.hierarchy.DeleteComponents(packet.gameObjectInstanceIDs, packet.instanceIDs);
		}

		private void	Handle_Scene_ServerSendMaterialData(Client sender, Packet _packet)
		{
			ServerSendMaterialDataPacket	packet = _packet as ServerSendMaterialDataPacket;

			this.hierarchy.CreateMaterialData(packet.netMaterial);
		}

		private void	Handle_Scene_ServerUpdateMaterialProperty(Client sender, Packet _packet)
		{
			ServerUpdateMaterialPropertyPacket	packet = _packet as ServerUpdateMaterialPropertyPacket;

			this.hierarchy.UpdateMaterialProperty(packet.instanceID, packet.propertyName, packet.rawValue);
		}

		private void	Handle_Scene_ServerUpdateMaterialVector2(Client sender, Packet _packet)
		{
			ServerUpdateMaterialVector2Packet	packet = _packet as ServerUpdateMaterialVector2Packet;

			this.hierarchy.UpdateMaterialVector2(packet.instanceID, packet.propertyName, packet.value, packet.type);
		}

		private void	Handle_Scene_ServerSendEnumData(Client sender, Packet _packet)
		{
			ServerSendEnumDataPacket	packet = _packet as ServerSendEnumDataPacket;

			this.hierarchy.SetEnumData(packet.type, packet.hasFlagAttribute, packet.names, packet.values);
		}

		private void	Handle_Scene_ServerSendComponent(Client sender, Packet _packet)
		{
			ServerSendComponentPacket	packet = _packet as ServerSendComponentPacket;

			this.hierarchy.AddComponent(packet.gameObjectInstanceID, packet.component);
		}

		private void	Handle_Scene_ServerReturnInvokeResult(Client sender, Packet _packet)
		{
			ServerReturnInvokeResultPacket	packet = _packet as ServerReturnInvokeResultPacket;

			this.hierarchy.SetInvokeResult(packet.result);
		}
	}
}