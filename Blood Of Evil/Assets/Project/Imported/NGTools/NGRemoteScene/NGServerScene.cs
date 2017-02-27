#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using NGTools.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace NGTools.NGRemoteScene
{
	using UnityEngine;

	public enum ReturnDeleteGameObject
	{
		Success,
		GameObjectNotFound,
	}

	public enum ReturnUpdateFieldValue
	{
		Success,
		InternalError,
		GameObjectNotFound,
		ComponentNotFound,
		PathNotResolved,
	}

	public enum ReturnInvokeComponentMethod
	{
		Success,
		GameObjectNotFound,
		ComponentNotFound,
		MethodNotFound,
		InvalidArgument,
		InvocationFailed
	}

	public enum ReturnUpdateMaterialProperty
	{
		Success,
		InternalError,
		MaterialNotFound,
		ShaderNotFound,
		PropertyNotFound,
	}

	public enum ReturnUpdateMaterialVector2
	{
		Success,
		InternalError,
		MaterialNotFound,
		ShaderNotFound,
		PropertyNotFound,
	}

	public enum ReturnGetArray
	{
		Success,
		GameObjectNotFound,
		ComponentNotFound,
		PathNotResolved,
	}

	/// <summary>
	/// Might be a temp interface.
	/// </summary>
	public interface IHierarchyManagement
	{
		bool	SetSibling(int instanceID, int instanceIDParent, int siblingIndex);
	}

	[Serializable]
	public sealed class ListingAssets
	{
		[Serializable]
		public sealed class AssetReferences
		{
			public string	asset;
			public Object[]	references;
			public int[]	IDs;
			public string[]	types;
		}

		public AssetReferences[]	assets;
	}

	[Serializable]
	public sealed class ListingShaders
	{
		[Serializable]
		public sealed class AssetReferences
		{
			public string	asset;
			public Shader[]	references;
			public int[]	IDs;
		}

		public Shader[]	shaders;
		public byte[]	properties;
	}

	[HelpURL(Constants.WikiBaseURL + "#markdown-header-13-ng-remote-scene"), DisallowMultipleComponent]
	public class NGServerScene : BaseServer, IHierarchyManagement
	{
		public const int	UDPPortBroadcastMin = 6547;
		public const int	UDPPortBroadcastMax = 6557;

		private static ByteBuffer	Buffer = new ByteBuffer(64);

		[Header("Interval between watched GameObject updates (second)")]
		public float	refreshMonitoring = 2F;

		[Header("Disable NG Server Scene after Start() to avoid call to OnGUI().")]
		public bool		autoDisabled = true;

		private List<ServerGameObject>				rootGameObjects = new List<ServerGameObject>();
		private Dictionary<int, ServerGameObject>	cachedGameObjects = new Dictionary<int, ServerGameObject>();
		private Dictionary<int, KeyValuePair<MonitorGameObject, List<Client>>>	watchedGameObjects = new Dictionary<int, KeyValuePair<MonitorGameObject, List<Client>>>();
		private Dictionary<int, KeyValuePair<MonitorMaterial, List<Client>>>	watchedMaterials = new Dictionary<int, KeyValuePair<MonitorMaterial, List<Client>>>();
		private List<int>							currentWatchingListGameObjects = new List<int>();
		private List<int>							currentWatchingListMaterials = new List<int>();

		private Dictionary<Type, Dictionary<int, Object>>	registeredResources = new Dictionary<Type, Dictionary<int, Object>>();

		[Header("Assets embed into your build")]
		public ListingAssets	resources;
		[Header("Shaders embed into your build (Available in the list of shaders when changing a Material)")]
		public ListingShaders	shaderReferences;

		[Group("[Debug]")]
		public bool	displayDebug = false;
		[Group("[Debug]")]
		public int	offsetSent = 0;
		[Group("[Debug]")]
		public int	offsetReceived = 0;
		[Group("[Debug]")]
		public int	maxPacketsDisplay = 100;

		private GUIStyle	packetStyle;

		private int			selected = -1;
		private string[]	names = null;
		private Vector2		eventScrollPosition;
		private Vector2		sendScrollPosition;
		private Vector2		receiveScrollPosition;

		private List<string>	events = new List<string>();
		
		public new AbstractTcpListener	listener { get { return (AbstractTcpListener)base.listener; } }

		private NGShader[]	shaderProperties;

		private string		valuePath;
		private string[]	paths;
		private byte[]		rawValue;
		private byte[]		newRawValue;

		private List<int>		instanceIDs = new List<int>();
		private List<Transform>	result = new List<Transform>();

		private AutoDetectUDPClient	udpClient;

		protected override void	Start()
		{
			base.Start();

			this.StartCoroutine(this.UpdateWatchers());

			try
			{
				if (this.listener != null)
					this.udpClient = new AutoDetectUDPClient(this, this.listener.port, NGServerScene.UDPPortBroadcastMin, NGServerScene.UDPPortBroadcastMax, AutoDetectUDPClient.UDPPingInterval);
			}
			catch (SocketException ex)
			{
				InternalNGDebug.LogException("The UDP client has failed, it may be caused by port " + this.listener.port + " already in use.", ex);
			}

			this.events.Add("Start");

			if (this.autoDisabled == true)
				this.enabled = false;
		}

		protected virtual void	OnEnable()
		{
			this.events.Add("OnEnable");
		}

		protected virtual void	OnDisable()
		{
			this.events.Add("OnDisable");
		}

		protected override void	OnDestroy()
		{
			base.OnDestroy();

			if (this.udpClient != null)
				this.udpClient.Stop();

			this.events.Add("OnDestroy");
		}

		protected virtual void	OnGUI()
		{
			if (this.displayDebug == false)
				return;

			GUILayout.BeginHorizontal(GUILayout.MaxHeight(150F), GUILayout.Width(Screen.width));
			{
				this.eventScrollPosition = GUILayout.BeginScrollView(this.eventScrollPosition);
				{
					for (int i = this.events.Count - 1; i >= 0; --i)
						GUILayout.Label(this.events[i]);
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndHorizontal();

			if (this.names == null || this.listener.clients.Count != this.names.Length)
			{
				this.names = new string[this.listener.clients.Count];

				for (int i = 0; i < this.listener.clients.Count; i++)
					this.names[i] = this.listener.clients[i].tcpClient.Client.LocalEndPoint.ToString();
			}

			this.selected = Mathf.Clamp(GUILayout.Toolbar(this.selected, this.names), 0, this.listener.clients.Count - 1);

			if (this.selected >= 0 && this.selected < this.listener.clients.Count)
				this.DrawClientPackets(this.listener.clients[this.selected]);
		}

		private void	OnApplicationQuit()
		{
			this.events.Add("OnApplicationQuit");
		}

		private void	OnApplicationFocus(bool focusStatus)
		{
			this.events.Add("OnApplicationFocus " + focusStatus);
		}

		private void	OnApplicationPause(bool pauseStatus)
		{
			this.events.Add("OnApplicationPause " + pauseStatus);
		}

		private void	DrawClientPackets(Client client)
		{
			if (this.packetStyle == null)
			{
				this.packetStyle = new GUIStyle(GUI.skin.label);
				this.packetStyle.fontSize = 9;
			}

			GUILayout.Label("Client " + client.tcpClient.Client.LocalEndPoint.ToString() + " " + DateTime.Now.ToString("HH:mm:ss"));
			GUILayout.Label(client.ToString());

			GUILayout.BeginHorizontal();
			{
				if (client.saveSentPackets == true)
				{
					GUILayout.BeginVertical();
					{
						GUILayout.Label("Sent (" + client.sentPacketsHistoric.Count + ")");
						this.sendScrollPosition = GUILayout.BeginScrollView(this.sendScrollPosition);
						{
							for (int i = client.sentPacketsHistoric.Count - 1 - this.offsetSent, j = 0; i >= 0 && j < this.maxPacketsDisplay; --i, ++j)
								GUILayout.Label(client.sentPacketsHistoric[i].time + " " + client.sentPacketsHistoric[i].packet.ToString(), this.packetStyle);
						}
						GUILayout.EndScrollView();
					}
					GUILayout.EndVertical();
				}

				GUILayout.BeginVertical();
				{
					GUILayout.Label("Received (" + client.receivedPacketsHistoric.Count + ")");
					this.receiveScrollPosition = GUILayout.BeginScrollView(this.receiveScrollPosition);
					{
						for (int i = client.receivedPacketsHistoric.Count - 1 - this.offsetReceived, j = 0; i >= 0 && j < this.maxPacketsDisplay; --i, ++j)
							GUILayout.Label(client.receivedPacketsHistoric[i], this.packetStyle);
					}
					GUILayout.EndScrollView();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		private IEnumerator	UpdateWatchers()
		{
			int	k = 0;

			while (true)
			{
				lock (this.currentWatchingListGameObjects)
				{
					// Get list of GameObjects being watched.
					if (this.currentWatchingListGameObjects.Count == 0)
					{
						foreach (var watcher in this.watchedGameObjects)
						{
							if (watcher.Value.Value.Count > 0)
								this.currentWatchingListGameObjects.Add(watcher.Key);
						}

						k = 0;
					}

					// Continuous update of watchers spread in time; avoiding spikes.
					if (this.currentWatchingListGameObjects.Count > 0)
					{
						if (k < this.currentWatchingListGameObjects.Count)
						{
							KeyValuePair<MonitorGameObject, List<Client>>	watcher = this.watchedGameObjects[this.currentWatchingListGameObjects[k]];

							// Check if there still Clients watching.
							if (watcher.Value.Count > 0)
							{
								watcher.Key.UpdateValues(watcher.Value);
								if (watcher.Key.ToDelete == true)
									this.watchedGameObjects.Remove(this.currentWatchingListGameObjects[k]);
							}

							++k;
						}

						if (k >= this.currentWatchingListGameObjects.Count)
							this.currentWatchingListGameObjects.Clear();
					}
				}

				lock (this.currentWatchingListMaterials)
				{
					// Get list of Materials being watched.
					if (this.currentWatchingListMaterials.Count == 0)
					{
						foreach (var watcher in this.watchedMaterials)
						{
							if (watcher.Value.Value.Count > 0)
								this.currentWatchingListMaterials.Add(watcher.Key);
						}

						k = 0;
					}

					// Continuous update of watchers spread in time; avoiding spikes.
					if (this.currentWatchingListMaterials.Count > 0)
					{
						if (k < this.currentWatchingListMaterials.Count)
						{
							KeyValuePair<MonitorMaterial, List<Client>>	watcher = this.watchedMaterials[this.currentWatchingListMaterials[k]];

							// Check if there still Clients watching.
							if (watcher.Value.Count > 0)
							{
								watcher.Key.UpdateValues(watcher.Value);
								if (watcher.Key.ToDelete == true)
									this.watchedMaterials.Remove(this.currentWatchingListMaterials[k]);
							}

							++k;
						}

						if (k >= this.currentWatchingListMaterials.Count)
							this.currentWatchingListMaterials.Clear();
					}
				}

				int	total = this.currentWatchingListGameObjects.Count + this.currentWatchingListMaterials.Count;
				if (total >= 2)
					yield return new WaitForSeconds(this.refreshMonitoring / total);
				else
					yield return new WaitForSeconds(this.refreshMonitoring);
			}
		}

		protected override PacketExecuter	CreatePacketExecuter()
		{
			return new ServerSceneExecuter(this);
		}

		public List<ServerGameObject>	ScanHierarchy()
		{
			this.rootGameObjects.Clear();

			GameObject[]	go = Resources.FindObjectsOfTypeAll<GameObject>();

			// Hell of a trick...
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			InternalNGDebug.Log("NG Remote Scene is going to scan all GameObject from the current scene, the following errors that might appear are harmless.");

			GameObject	temp = new GameObject();
#endif

			// Process new or refresh old GameObject.
			for (int i = 0; i < go.Length; i++)
			{
				//Debug.Log(go[i].name + " " + go[i].hideFlags + " " + go[i].transform.GetSiblingIndex() + " " + go[i].activeInHierarchy + " " + go[i].transform.parent + " " + go[i].transform.root, go[i]);
				if ((go[i].hideFlags & HideFlags.HideInHierarchy) != 0
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
					|| go[i].scene.IsValid() == false
#endif
					)
				{
					continue;
				}

				try
				{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					Transform	t = go[i].transform.parent;

#if UNITY_5
					if (go[i].transform is RectTransform)
						go[i].transform.SetParent(temp.transform, false);
					else
#endif
					go[i].transform.parent = temp.transform; // Prior to Unity 5.3, this line will fail, and therefore give us the information we need, if it is a prefab or not.
					if (go[i].transform.parent != temp.transform)
						continue;

#if UNITY_5
					if (go[i].transform is RectTransform)
						go[i].transform.SetParent(t, false);
					else
#endif
						go[i].transform.parent = t;
#endif
					this.ProcessRootGameObject(go[i].transform.root.gameObject);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}

#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Object.DestroyImmediate(temp);
#endif

			this.rootGameObjects.Sort((a, b) => b.gameObject.transform.GetSiblingIndex() - a.gameObject.transform.GetSiblingIndex());

			// Remove destroyed GameObjects.
			foreach (var item in this.cachedGameObjects)
			{
				if (item.Value == null)
					this.cachedGameObjects.Remove(item.Key);
			}

			return this.rootGameObjects;
		}

		public ServerGameObject	GetGameObject(int instanceID)
		{
			ServerGameObject	ng = null;

			this.cachedGameObjects.TryGetValue(instanceID, out ng);
			return ng;
		}

		public List<ServerComponent>	ScanGameObject(int instanceID)
		{
			ServerGameObject	ng;

			if (this.cachedGameObjects.TryGetValue(instanceID, out ng) == true)
			{
				ng.ProcessComponents();
				return ng.components;
			}

			return null;
		}

		/// <summary>
		/// Gets a registered resource from an <paramref name="instanceID"/> in the list associated to <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type of resource, any inheriting from Object.</param>
		/// <param name="instanceID">An instanceID from a previously registered object.</param>
		/// <returns></returns>
		/// <remarks>Look at <see cref="GetResources"/> to register resources.</remarks>
		public T	GetResource<T>(int instanceID) where T : Object
		{
			Object					instance = null;
			Dictionary<int, Object>	dic = this.RegisterResources(typeof(T));

			dic.TryGetValue(instanceID, out instance);

			return instance as T;
		}

		/// <summary>
		/// Gets a registered resource from an <paramref name="instanceID"/> in the list associated to <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type of resource, any inheriting from Object.</param>
		/// <param name="instanceID">An instanceID from a previously registered object.</param>
		/// <returns></returns>
		/// <remarks>Look at <see cref="GetResources"/> to register resources.</remarks>
		public Object	GetResource(Type type, int instanceID)
		{
			Object					instance = null;
			Dictionary<int, Object>	dic = this.RegisterResources(type);

			dic.TryGetValue(instanceID, out instance);

			return instance as Object;
		}

		/// <summary>
		/// Gets names and IDs from all resources of a <paramref name="type"/> and registers them localy for future requests.
		/// </summary>
		/// <param name="type">Type of resource, any inheriting from Object.</param>
		/// <param name="resourceNames">An array containing names of resources.</param>
		/// <param name="instanceIDs">An array containing instanceIDs of resources.</param>
		/// <remarks>Look at <see cref="GetResource"/> to use resources.</remarks>
		public void	GetResources(Type type, out string[] resourceNames, out int[] instanceIDs)
		{
			Dictionary<int, Object>	dic = this.RegisterResources(type);

			resourceNames = new string[dic.Count];
			instanceIDs = new int[dic.Count];

			int	i = 0;

			foreach (var resource in dic)
			{
				// Skip destroyed resources.
				if (resource.Value != null)
				{
					resourceNames[i] = resource.Value.name;
					instanceIDs[i] = resource.Key;

					++i;
				}
			}
		}

		private Dictionary<int, Object>	RegisterResources(Type type)
		{
			InternalNGDebug.Assert(typeof(Object).IsAssignableFrom(type) == true, "RegisterResources is requesting a non-Object type.");

			Dictionary<int, Object>	dic;

			if (this.registeredResources.TryGetValue(type, out dic) == false)
			{
				Object[]	resources = Resources.FindObjectsOfTypeAll(type);

				dic = new Dictionary<int, Object>();

				for (int i = 0; i < resources.Length; i++)
					dic.Add(resources[i].GetInstanceID(), resources[i]);

				this.registeredResources.Add(type, dic);
			}

			return dic;
		}

		/// <summary>
		/// Sets a GameObject from <paramref name="instanceID"/> child of GameObject from <paramref name="instanceIDParent"/> at the position <paramref name="siblingIndex"/>.
		/// </summary>
		/// <param name="instanceID">ID of any GameObject.</param>
		/// <param name="instanceIDParent">ID of the parent GameObject.</param>
		/// <param name="siblingIndex">Position in children.</param>
		/// <returns></returns>
		public bool	SetSibling(int instanceID, int instanceIDParent, int siblingIndex)
		{
			ServerGameObject	a;
			ServerGameObject	b = null;

			//Debug.Log("SetSibling " + instanceID + "	" + instanceIDParent + "	" + siblingIndex);

			if (this.cachedGameObjects.TryGetValue(instanceID, out a) == true &&
				(instanceIDParent == -1 || this.cachedGameObjects.TryGetValue(instanceIDParent, out b) == true))
			{
				if (instanceIDParent == -1)
				{
					a.gameObject.transform.SetParent(null, true);
					//Debug.Log(a.gameObject + " go into root at " + siblingIndex, a.gameObject);
				}
				else
				{
					a.gameObject.transform.SetParent(b.gameObject.transform, true);
					//Debug.Log(a.gameObject + " go into " + b.gameObject + " at " + siblingIndex, a.gameObject);
				}

				//Debug.Log(a.gameObject.transform.GetSiblingIndex());
				a.gameObject.transform.SetSiblingIndex(siblingIndex);
				//Debug.Log(a.gameObject.transform.GetSiblingIndex() + "	<>	" + siblingIndex);

				// HACK Workaround of bug #719312_ppd83fdc6vqv5sel
				// Finally does not resolve the issue when parent is root...
				//if (a.gameObject.transform.GetSiblingIndex() != siblingIndex &&
				//	a.gameObject.transform.parent != null &&
				//	a.gameObject.transform.childCount >= 2)
				//{
				//	a.gameObject.transform.GetChild(a.gameObject.transform.childCount - 2).SetAsLastSibling();
				//	Debug.Log(a.gameObject.transform.GetSiblingIndex() + "	<>	" + siblingIndex);
				//	a.gameObject.transform.SetSiblingIndex(siblingIndex - 1);
				//	Debug.Log(a.gameObject.transform.GetSiblingIndex() + "	<>	" + siblingIndex);
				//}

				return true;
			}

			return false;
		}

		public ReturnInvokeComponentMethod	InvokeComponentMethod(int gameObjectInstanceID, int instanceID, string methodSignature, byte[] arguments, ref string result)
		{
			ServerGameObject	gameObject;

			if (this.cachedGameObjects.TryGetValue(gameObjectInstanceID, out gameObject) == false)
				return ReturnInvokeComponentMethod.GameObjectNotFound;

			ServerComponent	component = gameObject.GetComponent(instanceID);

			if (component == null)
				return ReturnInvokeComponentMethod.ComponentNotFound;

			ServerMethodInfo	method = component.GetMethodFromSignature(methodSignature);

			if (method == null)
				return ReturnInvokeComponentMethod.MethodNotFound;

			object[]	convertedArgs = new object[method.argumentTypes.Length];

			NGServerScene.Buffer.Clear();
			NGServerScene.Buffer.Append(arguments);

			try
			{
				for (int i = 0; i < method.argumentTypes.Length; i++)
				{
					TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(method.argumentTypes[i]);

					if (typeHandler == null)
						return ReturnInvokeComponentMethod.InvalidArgument;

					convertedArgs[i] = typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, method.argumentTypes[i]);
				}
			}
			catch
			{
				return ReturnInvokeComponentMethod.InvalidArgument;
			}
			
			try
			{
				object	value = method.methodInfo.Invoke(component.component, convertedArgs);

				if (value != null)
					result = value.ToString();
				return ReturnInvokeComponentMethod.Success;
			}
			catch
			{
				return ReturnInvokeComponentMethod.InvocationFailed;
			}
		}

		public ReturnDeleteGameObject DeleteGameObject(int instanceID, List<int> instanceIDsDeleted)
		{
			ServerGameObject	gameObject = this.GetGameObject(instanceID);

			if (gameObject == null)
				return ReturnDeleteGameObject.GameObjectNotFound;

			gameObject.gameObject.GetComponentsInChildren<Transform>(true, result);

			for (int i = 0; i < result.Count; i++)
				instanceIDsDeleted.Add(result[i].gameObject.GetInstanceID());

			GameObject.Destroy(gameObject.gameObject);

			return ReturnDeleteGameObject.Success;
		}

		public void	DeleteGameObjects(List<int> instanceIDs)
		{
			ServerDeleteGameObjectsPacket	packet = new ServerDeleteGameObjectsPacket();

			for (int i = 0; i < instanceIDs.Count; i++)
			{
				this.DeleteGameObject(instanceIDs[i], this.instanceIDs);

				for (int j = 0; j < this.instanceIDs.Count; j++)
					packet.Add(this.instanceIDs[j]);
			}

			if (packet.instanceIDs.Count > 0)
				this.listener.BroadcastPacket(packet);
		}

		public void	DeleteComponents(List<int> gameObjectInstanceIDs, List<int> instanceIDs)
		{
			ServerDeleteComponentsPacket	packet = new ServerDeleteComponentsPacket();
			ServerDeleteGameObjectsPacket	gpacket = new ServerDeleteGameObjectsPacket();

			for (int i = 0; i < gameObjectInstanceIDs.Count; i++)
			{
				ServerGameObject	gameObject = this.GetGameObject(gameObjectInstanceIDs[i]);

				if (gameObject != null)
				{
					if (gameObject.RemoveComponent(instanceIDs[i]) == true)
						packet.Add(gameObjectInstanceIDs[i], instanceIDs[i]);
				}
				else
					gpacket.Add(gameObjectInstanceIDs[i]);
			}

			if (packet.instanceIDs.Count > 0)
			{
				lock (this.currentWatchingListGameObjects)
				{
					foreach (var pair in this.watchedGameObjects)
					{
						for (int j = 0; j < packet.instanceIDs.Count; j++)
							pair.Value.Key.DeleteComponent(packet.instanceIDs[j]);
					}

					this.listener.BroadcastPacket(packet);
				}
			}
			if (gpacket.instanceIDs.Count > 0)
				this.listener.BroadcastPacket(gpacket);
		}

		public void	WatchGameObjects(Client sender, int[] instanceIDs)
		{
			int	skipped = 0;

			// Stop watchers when there is no more Client watching it.
			foreach (var pair in this.watchedGameObjects)
			{
				if (instanceIDs.Contains(pair.Key) == true)
				{
					++skipped;
					continue;
				}

				// Remove Client from watcher.
				pair.Value.Value.Remove(sender);

				if (pair.Value.Value.Count == 0)
				{
					lock (this.currentWatchingListGameObjects)
						this.currentWatchingListGameObjects.Remove(pair.Key);
				}
			}

			for (int i = 0; i < instanceIDs.Length; i++)
			{
				ServerGameObject	serverGameObject = this.GetGameObject(instanceIDs[i]);

				if (serverGameObject == null)
					return;

				KeyValuePair<MonitorGameObject, List<Client>>	watcher;

				if (this.watchedGameObjects.TryGetValue(instanceIDs[i], out watcher) == true)
					watcher.Value.Add(sender);
				else
				{
					watcher = new KeyValuePair<MonitorGameObject, List<Client>>(new MonitorGameObject(serverGameObject), new List<Client>());
					watcher.Value.Add(sender);
					this.watchedGameObjects.Add(instanceIDs[i], watcher);
				}
			}
		}

		public void	WatchMaterials(Client sender, int[] instanceIDs)
		{
			int	skipped = 0;

			// Stop watchers when there is no more Client watching it.
			foreach (var pair in this.watchedMaterials)
			{
				if (instanceIDs.Contains(pair.Key) == true)
				{
					++skipped;
					continue;
				}

				// Remove Client from watcher.
				pair.Value.Value.Remove(sender);

				if (pair.Value.Value.Count == 0)
				{
					lock (this.currentWatchingListMaterials)
						this.currentWatchingListMaterials.Remove(pair.Key);
				}
			}

			for (int i = 0; i < instanceIDs.Length; i++)
			{
				Material	mat = this.GetResource<Material>(instanceIDs[i]);

				if (mat == null)
					return;

				KeyValuePair<MonitorMaterial, List<Client>>	watcher;

				if (this.watchedMaterials.TryGetValue(instanceIDs[i], out watcher) == true)
					watcher.Value.Add(sender);
				else
				{
					NGShader	shader = this.GetNGShader(mat.shader);
					watcher = new KeyValuePair<MonitorMaterial, List<Client>>(new MonitorMaterial(mat, shader), new List<Client>());
					watcher.Value.Add(sender);
					this.watchedMaterials.Add(instanceIDs[i], watcher);
				}
			}
		}

		public ReturnUpdateMaterialProperty	UpdateMaterialProperty(int instanceID, string propertyName, byte[] rawValue, out byte[] newValue)
		{
			Material	material = this.GetResource<Material>(instanceID);

			newValue = null;

			if (material == null)
				return ReturnUpdateMaterialProperty.MaterialNotFound;

			try
			{
				NGShader	shader = this.GetNGShader(material.shader);

				if (shader == null)
					return ReturnUpdateMaterialProperty.ShaderNotFound;

				NGShaderProperty	prop = shader.GetProperty(propertyName);

				if (prop == null)
					return ReturnUpdateMaterialProperty.PropertyNotFound;

				Type	type = null;

				if (prop.type == NGShader.ShaderPropertyType.Color)
					type = typeof(Color);
				else if (prop.type == NGShader.ShaderPropertyType.Float ||
						 prop.type == NGShader.ShaderPropertyType.Range)
				{
					type = typeof(float);
				}
				else if (prop.type == NGShader.ShaderPropertyType.TexEnv)
					type = typeof(Texture);
				else if (prop.type == NGShader.ShaderPropertyType.Vector)
					type = typeof(Vector4);

				TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(type);
				InternalNGDebug.Assert(typeHandler != null, "TypeHandler for " + prop.name + " is not supported.");

				NGServerScene.Buffer.Clear();
				NGServerScene.Buffer.Append(rawValue);

				if (prop.type == NGShader.ShaderPropertyType.Color)
				{
					material.SetColor(prop.name, (Color)typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type));
					newValue = typeHandler.Serialize(type, material.GetColor(prop.name));
					return ReturnUpdateMaterialProperty.Success;
				}
				else if (prop.type == NGShader.ShaderPropertyType.Float ||
						 prop.type == NGShader.ShaderPropertyType.Range)
				{
					material.SetFloat(prop.name, (float)typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type));
					newValue = typeHandler.Serialize(type, material.GetFloat(prop.name));
					return ReturnUpdateMaterialProperty.Success;
				}
				else if (prop.type == NGShader.ShaderPropertyType.TexEnv)
				{
					material.SetTexture(prop.name, (Texture)typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type));
					newValue = typeHandler.Serialize(type, material.GetTexture(prop.name));
					return ReturnUpdateMaterialProperty.Success;
				}
				else if (prop.type == NGShader.ShaderPropertyType.Vector)
				{
					material.SetVector(prop.name, (Vector4)typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type));
					newValue = typeHandler.Serialize(type, material.GetVector(prop.name));
					return ReturnUpdateMaterialProperty.Success;
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("Material property \"" + propertyName  + "\" not updated.", ex);
			}

			return ReturnUpdateMaterialProperty.InternalError;
		}

		public ReturnUpdateMaterialVector2	UpdateMaterialVector2(int instanceID, string propertyName, Vector2 value, MaterialVector2Type vectorType, out Vector2 newValue)
		{
			Material	material = this.GetResource<Material>(instanceID);

			newValue = default(Vector2);

			if (material == null)
				return ReturnUpdateMaterialVector2.MaterialNotFound;

			if (vectorType == MaterialVector2Type.Offset)
			{
				material.SetTextureOffset(propertyName, value);
				newValue = material.GetTextureOffset(propertyName);
			}
			else if (vectorType == MaterialVector2Type.Scale)
			{
				material.SetTextureScale(propertyName, value);
				newValue = material.GetTextureScale(propertyName);
			}

			return ReturnUpdateMaterialVector2.Success;
		}

		/// <summary>Resolves the path and updates the value with <paramref name="rawValue"/>.</summary>
		/// <param name="valuePath"></param>
		/// <param name="rawValue"></param>
		/// <returns></returns>
		/// <remarks>See <see cref="NGToolsEditor.NGHierarchyEditorWindow.UpdateFieldValue" /> for the client equivalent.</remarks>
		/// <exception cref="System.MissingFieldException">Thrown when an unknown field from ClientGameObject is being assigned.</exception>
		/// <exception cref="System.InvalidCastException">Thrown when an array is assigned but the array is not supported.</exception>
		/// <exception cref="System.ArgumentException">Thrown when the path seems to be not resolvable.</exception>
		public ReturnUpdateFieldValue	UpdateFieldValue(string valuePath, byte[] rawValue, out byte[] newValue)
		{
			string[]	paths = valuePath.Split('.');

			newValue = null;

			try
			{
				ServerGameObject	go = this.GetGameObject(int.Parse(paths[0]));

				if (go == null)
				{
					InternalNGDebug.LogError("GameObject \"" + paths[0] + "\" does not exist in path \"" + valuePath + "\".");
					return ReturnUpdateFieldValue.GameObjectNotFound;
				}

				int	instanceID;

				// Is a field for ServerGameObject.
				if (int.TryParse(paths[1], out instanceID) == false)
				{
					// No time to waste on abstracting ClientGameObject, maybe later.
					switch (paths[1])
					{
						case "tag":
						case "name":
							TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(typeof(string));

							NGServerScene.Buffer.Clear();
							NGServerScene.Buffer.Append(rawValue);

							if (paths[1] == "tag")
								go.gameObject.tag = typeHandler.Deserialize(NGServerScene.Buffer, typeof(string)) as string;
							else if (paths[1] == "name")
								go.gameObject.name = typeHandler.Deserialize(NGServerScene.Buffer, typeof(string)) as string;

							NGServerScene.Buffer.Clear();
							if (paths[1] == "tag")
								typeHandler.Serialize(NGServerScene.Buffer, typeof(string), go.gameObject.tag);
							else if (paths[1] == "name")
								typeHandler.Serialize(NGServerScene.Buffer, typeof(string), go.gameObject.name);

							newValue = NGServerScene.Buffer.Flush();
							return ReturnUpdateFieldValue.Success;

						case "active":
						case "isStatic":
							typeHandler = TypeHandlersManager.GetTypeHandler(typeof(bool));

							NGServerScene.Buffer.Clear();
							NGServerScene.Buffer.Append(rawValue);

							if (paths[1] == "active")
								go.gameObject.SetActive((bool)typeHandler.Deserialize(NGServerScene.Buffer, typeof(bool)));
							else if (paths[1] == "isStatic")
								go.gameObject.isStatic = (bool)typeHandler.Deserialize(NGServerScene.Buffer, typeof(bool));

							NGServerScene.Buffer.Clear();
							if (paths[1] == "active")
								typeHandler.Serialize(NGServerScene.Buffer, typeof(bool), go.gameObject.activeSelf);
							else if (paths[1] == "isStatic")
								typeHandler.Serialize(NGServerScene.Buffer, typeof(bool), go.gameObject.isStatic);

							newValue = NGServerScene.Buffer.Flush();
							return ReturnUpdateFieldValue.Success;

						case "layer":
							typeHandler = TypeHandlersManager.GetTypeHandler(typeof(int));

							NGServerScene.Buffer.Clear();
							NGServerScene.Buffer.Append(rawValue);

							go.gameObject.layer = (int)typeHandler.Deserialize(NGServerScene.Buffer, typeof(int));

							NGServerScene.Buffer.Clear();
							typeHandler.Serialize(NGServerScene.Buffer, typeof(int), go.gameObject.layer);

							newValue = NGServerScene.Buffer.Flush();
							return ReturnUpdateFieldValue.Success;

						default:
							throw new MissingFieldException(valuePath);
					}
				}

				ServerComponent	b = go.GetComponent(int.Parse(paths[1]));

				if (b == null)
				{
					Debug.LogError("Component \"" + paths[1] + "\" does not exist in path \"" + valuePath + "\".");
					return ReturnUpdateFieldValue.ComponentNotFound;
				}

				IFieldModifier	f = b.GetField(paths[2]);

				if (f == null)
				{
					InternalNGDebug.LogError("Path \"" + valuePath + "\" is invalid.");
					return ReturnUpdateFieldValue.PathNotResolved;
				}

				this.valuePath = valuePath;
				this.paths = paths;
				this.rawValue = rawValue;
				this.newRawValue = null;

				f.SetValue(b.component, this.Resolve(f, b.component, 2));

				newValue = this.newRawValue;
				return ReturnUpdateFieldValue.Success;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException("Path="+ this.valuePath, ex);
			}

			return ReturnUpdateFieldValue.InternalError;
		}

		public NGShader	GetNGShader(Shader shader)
		{
			if (shader == null)
				return null;

			if (this.shaderProperties == null)
			{
				if (this.shaderReferences == null ||
					this.shaderReferences.shaders.Length == 0)
				{
					InternalNGDebug.LogError("Materials need to be first cached before you build the project.");
					return null;
				}

				NGServerScene.Buffer.Clear();
				NGServerScene.Buffer.Append(this.shaderReferences.properties);

				int	total = NGServerScene.Buffer.ReadInt32();

				this.shaderProperties = new NGShader[total];

				for (int i = 0; i < total; i++)
					this.shaderProperties[i] = new NGShader(NGServerScene.Buffer);
			}

			for (int i = 0; i < this.shaderProperties.Length; i++)
			{
				if (this.shaderProperties[i].name.Equals(shader.name) == true)
					return this.shaderProperties[i];
			}

			InternalNGDebug.LogWarning("Shader " + shader + " was not found in cache. You might need to refresh the cache.", shader);

			return null;
		}

		private void	ProcessRootGameObject(GameObject root)
		{
			int					instanceID = root.GetInstanceID();
			ServerGameObject	ng;

			if (this.cachedGameObjects.TryGetValue(instanceID, out ng) == true)
			{
				if (this.rootGameObjects.Contains(ng) == false)
				{
					this.rootGameObjects.Add(ng);
					ng.RefreshChildren(this.cachedGameObjects);
				}
			}
			else
			{
				ng = new ServerGameObject(root, this.cachedGameObjects);
				this.rootGameObjects.Add(ng);
			}

		}

		private object	Resolve(IFieldModifier field, object instance, int i)
		{
			Type	type = field.Type;

			if (i == this.paths.Length - 1)
			{
				// If an array resize.
				if (type.IsUnityArray() == true)
				{
					object		array = field.GetValue(instance);
					Type		subType = Utility.GetArraySubType(type);
					TypeHandler	subTypeHandler = TypeHandlersManager.GetTypeHandler(subType);

					NGServerScene.Buffer.Clear();
					NGServerScene.Buffer.Append(rawValue);

					TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(typeof(int));
					int			newSize = (int)typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, typeof(int));

					if (type.IsArray == true)
					{
						Array	originArray = array as Array;

						if (originArray.Length != newSize)
						{
							Array	newArray = Array.CreateInstance(subType, newSize);
							int		k = 0;

							for (; k < newSize && k < originArray.Length; k++)
								newArray.SetValue(originArray.GetValue(k), k);

							for (; k < newSize; k++)
							{
								object	item;

								if (typeof(Object).IsAssignableFrom(subType) == true)
									item = null;
								else if (subType == typeof(string)) // WTF do we have to do this trick. -_-
									item = string.Empty;
								else
									item = Activator.CreateInstance(subType);

								this.listener.BroadcastPostPacket(new ServerUpdateFieldValuePacket(this.valuePath + "." + k, subTypeHandler.Serialize(subType, item)));

								newArray.SetValue(item, k);
							}

							array = newArray;
						}
					}
					else if (typeof(IList).IsAssignableFrom(type) == true)
					{
						IList	originArray = array as IList;

						if (originArray.Count != newSize)
						{
							while (originArray.Count > newSize)
								originArray.RemoveAt(originArray.Count - 1);

							while (originArray.Count < newSize)
							{
								object	item;

								if (typeof(Object).IsAssignableFrom(subType) == true)
									item = null;
								else if (subType == typeof(string)) // WTF do we have to do this trick. -_-
									item = string.Empty;
								else
									item = Activator.CreateInstance(subType);

								this.listener.BroadcastPostPacket(new ServerUpdateFieldValuePacket(this.valuePath + "." + originArray.Count, subTypeHandler.Serialize(subType, item)));

								originArray.Add(item);
							}
						}
					}
					else
						throw new InvalidCastException("Type at \"" + this.valuePath + "\" is not supported as an array.");

					this.newRawValue = typeHandler.Serialize(newSize);

					return array;
				}
				else
				{
					TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(type);
					InternalNGDebug.Assert(typeHandler != null, "TypeHandler for type " + type + " does not exist.");

					NGServerScene.Buffer.Clear();
					NGServerScene.Buffer.Append(rawValue);

					object	newValue = typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type);

					// Replace new value by old value when types are incompatible.
					if (newValue != null && type.IsAssignableFrom(newValue.GetType()) == false)
						newValue = field.GetValue(instance);

					this.newRawValue = typeHandler.Serialize(type, newValue);

					return newValue;
				}
			}
			else if (type.IsUnityArray() == true) // Any Array or IList.
			{
				object	array = field.GetValue(instance);
				Type	subType = Utility.GetArraySubType(type);
				int		index = int.Parse(paths[i + 1]);

				if (type.IsArray == true)
					(array as Array).SetValue(this.ResolveArray(subType, (array as Array).GetValue(index), i + 1), index);
				else if (typeof(IList).IsAssignableFrom(type) == true)
					(array as IList)[index] = this.ResolveArray(subType, (array as IList)[index], i + 1);
				else
					throw new InvalidCastException("Type at \"" + this.valuePath + "\" is not supported as an array.");

				return array;
			}
			else if (type.IsClass == true || // Any class.
					 type.IsStruct() == true) // Any struct.
			{
				object			fieldInstance = field.GetValue(instance);
				IFieldModifier	fieldInfo = Utility.GetFieldInfo(field.Type, this.paths[i + 1]);

				fieldInfo.SetValue(fieldInstance, this.Resolve(fieldInfo, fieldInstance, i + 1));

				return fieldInstance;
			}

			throw new InvalidOperationException("Resolve failed at " + i + " with " + instance + " of type " + type + ".");
		}

		private object	ResolveArray(Type type, object instance, int i)
		{
			if (i == this.paths.Length - 1)
			{
				TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(type);
				InternalNGDebug.Assert(typeHandler != null, "TypeHandler for type " + type + " does not exist.");

				NGServerScene.Buffer.Clear();
				NGServerScene.Buffer.Append(rawValue);

				object	newValue = typeHandler.DeserializeRealValue(this, NGServerScene.Buffer, type);

				this.newRawValue = typeHandler.Serialize(type, newValue);

				return newValue;
			}

			if (type.IsClass == true || // Any class.
				type.IsStruct() == true) // Any struct.
			{
				IFieldModifier	fieldInfo = Utility.GetFieldInfo(type, this.paths[i + 1]);

				fieldInfo.SetValue(instance, this.Resolve(fieldInfo, instance, i + 1));

				return instance;
			}

			return this.Resolve(Utility.GetFieldInfo(type, this.paths[i + 1]), instance, i + 1);
		}

		private object	GetResolve(IFieldModifier field, object instance, int i)
		{
			Type	type = field.Type;

			if (i == this.paths.Length - 1)
				return field.GetValue(instance);
			else if (type.IsUnityArray() == true) // Any Array or IList.
			{
				object	array = field.GetValue(instance);
				Type	subType = Utility.GetArraySubType(type);
				int		index = int.Parse(paths[i + 1]);

				if (type.IsArray == true)
					return this.GetResolveArray(subType, (array as Array).GetValue(index), i + 1);
				else if (typeof(IList).IsAssignableFrom(type) == true)
					return this.GetResolveArray(subType, (array as IList)[index], i + 1);
				else
					throw new InvalidCastException("Type at \"" + this.valuePath + "\" is not supported as an array.");
			}
			else if (type.IsClass == true || // Any class.
					 type.IsStruct() == true) // Any struct.
			{
				object			fieldInstance = field.GetValue(instance);
				IFieldModifier	fieldInfo = Utility.GetFieldInfo(field.Type, this.paths[i + 1]);

				return this.GetResolve(fieldInfo, fieldInstance, i + 1);
			}

			throw new InvalidOperationException("Resolve failed at " + i + " with " + instance + " of type " + type + ".");
		}

		private object	GetResolveArray(Type type, object instance, int i)
		{
			if (i == this.paths.Length - 1)
				return instance;

			if (type.IsClass == true || // Any class.
				type.IsStruct() == true) // Any struct.
			{
				IFieldModifier	fieldInfo = Utility.GetFieldInfo(type, this.paths[i + 1]);

				return this.GetResolve(fieldInfo, instance, i + 1);
			}

			return this.GetResolve(Utility.GetFieldInfo(type, this.paths[i + 1]), instance, i + 1);
		}

		public ReturnGetArray	GetArray(string arrayPath, out IEnumerable array)
		{
			string[]			paths = arrayPath.Split('.');
			ServerGameObject	go = this.GetGameObject(int.Parse(paths[0]));

			array = null;

			if (go == null)
			{
				Debug.LogError("GameObject \"" + paths[0] + "\" does not exist in path \"" + arrayPath + "\".");
				return ReturnGetArray.GameObjectNotFound;
			}

			ServerComponent	b = go.GetComponent(int.Parse(paths[1]));

			if (b == null)
			{
				Debug.LogError("Component \"" + paths[1] + "\" does not exist in path \"" + arrayPath + "\".");
				return ReturnGetArray.ComponentNotFound;
			}

			IFieldModifier	f = b.GetField(paths[2]);

			if (f == null)
			{
				Debug.LogError("Path \"" + this.valuePath + "\" is invalid.");
				return ReturnGetArray.PathNotResolved;
			}

			this.valuePath = arrayPath;
			this.paths = paths;

			array = this.GetResolve(f, b.component, 2) as IEnumerable;
			return ReturnGetArray.Success;
		}
	}
}