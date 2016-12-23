//using UnityEngine;
//using System.Collections;
//using System;

///// <summary>
///// Exception thrown when a singleton must be instantiated in scene and hasn't been instantiated in scene.
///// </summary>
///// <typeparam name="T">Singleton to instantiate in scene</typeparam>
//public class SingletonNotInstanciatedException<T> : Exception where T : Singleton<T>, new()
//{
//	/// <summary>
//	/// Type of the singleton to instantiate in scene
//	/// </summary>
//	public Type SingletonType
//	{
//		get
//		{
//			return typeof(T);
//		}
//	}

//	/// <summary>
//	/// Creates the exception
//	/// </summary>
//	public SingletonNotInstanciatedException() : base(typeof(T).Name + " must be instanciated in scene.") { }

//	/// <summary>
//	/// Creates the exception with a custom message
//	/// </summary>
//	/// <param name="msg"></param>
//	public SingletonNotInstanciatedException(string msg) : base(msg) { }
//}

///// <summary>
///// Base class for singletons
///// </summary>
///// <typeparam name="T"></typeparam>
//public abstract class Singleton<T> : MonoBehaviourEx where T : Singleton<T>, new()
//{
//	/// <summary>
//	/// Setting : If true, the singleton can be instantiated at initialisation.
//	/// </summary>
//	protected static bool					CanBeInstanciated;

//	/// <summary>
//	/// Setting : If can be instantiated, the singleton instance name
//	/// </summary>
//	protected static string					InstanceName;

//	/// <summary>
//	/// Setting ; If can be instantiated, the singleton parent name (if any)
//	/// </summary>
//	protected static string					ParentName;

//	/// <summary>
//	/// Setting : If can be instantiated, shall we hide the singleton?
//	/// </summary>
//	protected static bool					HideInstanciated;

//	/// <summary>
//	/// Setting : If can be instantiated, must we prevent the object from being destroyed at load scene?
//	/// </summary>
//	protected static bool					DontDestroyOnLoadScene;

//	/// <summary>
//	/// Setting : If can't be instantiated and doesn't exists in scene, type of the exception to create and throw
//	/// </summary>
//	protected static Type					CantInstanciateExceptionType;

//	/// <summary>
//	/// Event raised when a singleton has been initialized
//	/// </summary>
//	protected static event System.Action	SingletonInitializedEvent;

//	/// <summary>
//	/// Tells whether we are initializing or not
//	/// </summary>
//	private static bool						IsInitializing;

//	/// <summary>
//	/// Creates the singleton and sets its default settings
//	/// </summary>
//	static Singleton()
//	{
//		CanBeInstanciated				= true;
//		ParentName						= null;
//		HideInstanciated				= true;
//		DontDestroyOnLoadScene			= false;
//		InstanceName					= typeof(T).Name;
//		CantInstanciateExceptionType	= typeof(SingletonNotInstanciatedException<T>);
//	}

//	/// <summary>
//	/// Singleton instance
//	/// </summary>
//	private static T m_instance;

//	/// <summary>
//	/// Returns the singleton instance
//	/// </summary>
//	public static T instance
//	{
//		get
//		{
//			if (m_instance == null)
//			{
//				InitializeInstance(null);
//			}
//			return m_instance;
//		}
//	}

//	/// <summary>
//	/// Check whenether this Singleton is instantiated, (safer than instance != null statement)
//	/// </summary>
//	public static bool Exist { get { return m_instance != null; } }
	
//	/// <summary>
//	/// Initialises the singleton instance
//	/// </summary>
//	/// <param name="o">Previous instance</param>
//	private static void InitializeInstance(T o)
//	{
//		if (!IsInitializing)
//		{
//			IsInitializing = true;

//			if (o != null)
//				m_instance = o;
//			else
//				m_instance = GameObject.FindObjectOfType(typeof(T)) as T;

//			if (m_instance == null)
//			{
//				if (CanBeInstanciated)
//				{
//					m_instance = InstantiateSingleton();
//				}
//				else if (CantInstanciateExceptionType != null)
//				{
//					throw (Exception)Activator.CreateInstance(CantInstanciateExceptionType);
//				}
//				else
//				{
//					Debug.Log("Singleton:InitializeInstance: Singleton<" + typeof(T).Name + "> Not found.");
//				}
//			}
//			else
//			{
//				//Debug.Log("Singleton<" + typeof(T).Name + "> Started in scene.");
//			}
//			if (SingletonInitializedEvent != null)
//				SingletonInitializedEvent();
//			IsInitializing = false;
//		}
//	}

//	/// <summary>
//	/// Instantiate the singleton instance
//	/// </summary>
//	/// <returns>created instance</returns>
//	private static T InstantiateSingleton()
//	{
//		GameObject parent = null;
//		if (ParentName != null)
//		{
//			parent = GameObject.Find(ParentName);
//			if (parent == null)
//				parent = new GameObject(ParentName);
//		}

//		GameObject instanceGO = new GameObject(InstanceName);
//		T instance = instanceGO.AddComponent<T>();

//		if (parent)
//			instance.transform.parent = parent.transform;

//		if (HideInstanciated)
//			instanceGO.hideFlags = HideFlags.HideAndDontSave;

//		if (DontDestroyOnLoadScene)
//			DontDestroyOnLoad(instanceGO);

//		//Debug.Log("Singleton<" + typeof(T).Name + "> Created in scene.", instanceGO);

//		return instance;
//	}

//	/// <summary>
//	/// Called during first frame of existance of the given instance.
//	/// Destroys if not the main instance
//	/// </summary>
//	private void Awake()
//	{
//		if ((this as T) != m_instance && m_instance != null)
//		{
//			Destroy(this.gameObject);
//			return;
//		}

//		if (m_instance == null)
//		{
//			InitializeInstance(this as T);
//		}

//		OnAwake();
//	}

//	private void OnDestroy()
//	{
//		if(m_instance == this)
//			m_instance = null;
//		CleanSingleton();
//	}

//	protected virtual void CleanSingleton() { }

//	/// <summary>
//	/// Called during the first frame of the main instance existance
//	/// </summary>
//	protected virtual void OnAwake() { }

//	/// <summary>
//	/// Calls a instance function by name if the instance inherits from the given type
//	/// </summary>
//	/// \deprecated You should use an event or a virtual method instead
//	/// <param name="singletonType">Singleton extension type name</param>
//	/// <param name="functionName">Function name</param>
//	/// <returns>true if the function exists (and has been called)</returns>
//	[System.Obsolete("You should use an event or a virtual method instead")]
//	public static bool CallIfExists(string singletonType, string functionName)
//	{
//		Type t = Type.GetType(singletonType);
//		if (t == null || !t.IsInstanceOfType(m_instance))
//			return false;

//		System.Reflection.MethodInfo method = t.GetMethod(functionName);
//		if (method == null)
//			return false;

//		method.Invoke(m_instance, null);
//		return true;
//	}
//}
