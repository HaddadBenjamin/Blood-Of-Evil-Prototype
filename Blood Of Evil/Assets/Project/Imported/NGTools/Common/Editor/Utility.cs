#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditorInternal;
using InnerUtility = NGTools.Utility;

namespace NGToolsEditor
{
	using UnityEngine;

	[InitializeOnLoad]
	public static partial class	Utility
	{
		static	Utility()
		{
			try
			{
				Utility.InitiateAssemblies();

				for (int i = 0; i < Utility.allAssemblies.Length; i++)
				{
					foreach (Type t in Utility.allTypes[i])
					{
						FieldInfo[]	fields = t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

						for (int j = 0; j < fields.Length; j++)
						{
							if (fields[j].IsDefined(typeof(SetColorAttribute), false) == true)
							{
								object[]	attr = fields[j].GetCustomAttributes(typeof(SetColorAttribute), false);

								for (int k = 0; k < attr.Length; k++)
								{
									SetColorAttribute	setColor = attr[k] as SetColorAttribute;

									if (setColor != null)
									{
										// For security, set the personal color by default.
										fields[j].SetValue(null, setColor.personal);
	#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
										fields[j].SetValue(null, EditorGUIUtility.isProSkin == true ? setColor.pro : setColor.personal);
	#else
										Utility.ClosureSetColor(setColor, fields[j]);
	#endif
										break;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}
		}

		private static void	ClosureSetColor(SetColorAttribute setColor, FieldInfo field)
		{
			EditorApplication.delayCall += () =>
			{
				field.SetValue(null, EditorGUIUtility.isProSkin == true ? setColor.pro : setColor.personal);
			};
		}

		#region Shared variables
		public const string				DragObjectDataName = "i";
		public static Vector2			position2D = new Vector2();
		public static GUIContent		content = new GUIContent();
		/// <summary>Is setup before OnGUI by NGConsole and ModuleEditorWindow. Use this variable to know which window is drawing.</summary>
		public static EditorWindow		drawingWindow = null;
		#endregion

		#region Buffer tools
		private static Stack<ByteBuffer>	poolBBuffers = new Stack<ByteBuffer>(2);
		private static Stack<StringBuilder>	poolBuffers = new Stack<StringBuilder>(2);
		private static Type[]				assemblyTypes;

		public static StringBuilder	GetBuffer(string initialValue)
		{
			StringBuilder	b = GetBuffer();
			b.Append(initialValue);
			return b;
		}

		public static StringBuilder	GetBuffer()
		{
			if (Utility.poolBuffers.Count > 0)
				return Utility.poolBuffers.Pop();
			return new StringBuilder(64);
		}

		public static void			RestoreBuffer(StringBuilder buffer)
		{
			buffer.Length = 0;
			Utility.poolBuffers.Push(buffer);
		}

		public static string		ReturnBuffer(StringBuilder buffer)
		{
			string	result = buffer.ToString();
			buffer.Length = 0;
			Utility.poolBuffers.Push(buffer);
			return result;
		}

		public static ByteBuffer	GetBBuffer(byte[] initialValue)
		{
			ByteBuffer	b = GetBBuffer();
			b.Append(initialValue);
			return b;
		}

		public static ByteBuffer	GetBBuffer()
		{
			if (Utility.poolBBuffers.Count > 0)
				return Utility.poolBBuffers.Pop();
			return new ByteBuffer(64);
		}

		public static void			RestoreBBuffer(ByteBuffer buffer)
		{
			buffer.Clear();
			Utility.poolBBuffers.Push(buffer);
		}

		public static byte[]		ReturnBBuffer(ByteBuffer buffer)
		{
			byte[]	result = buffer.Flush();
			buffer.Clear();
			Utility.poolBBuffers.Push(buffer);
			return result;
		}
		#endregion

		public static Type	GetArraySubType(Type arrayType)
		{
			return InnerUtility.GetArraySubType(arrayType);
		}

		private static List<object>	tempList = new List<object>();

		public static T[]				CreateInstancesOf<T>(params object[] args) where T : class
		{
			Utility.tempList.Clear();

			foreach (Type type in Utility.EachSubClassesOf(typeof(T)))
				Utility.tempList.Add(Activator.CreateInstance(type, args));

			T[]	result = new T[Utility.tempList.Count];

			for (int i = 0; i < Utility.tempList.Count; i++)
				result[i] = Utility.tempList[i] as T;

			return result;
		}

		public static Type[]			GetSubClassesOf(Type baseType)
		{
			if (Utility.assemblyTypes == null)
				Utility.assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

			return Utility.assemblyTypes.Where(t => t.IsSubclassOf(baseType)).ToArray();
		}

		public static Type[]			GetAllSubClassesOf(Type baseType)
		{
			List<Type>	result = new List<Type>();

			if (Utility.allAssemblies == null)
				Utility.InitiateAssemblies();

			for (int i = 0; i < Utility.allAssemblies.Length; i++)
			{
				foreach (Type t in Utility.allTypes[i])
				{
					if (t.IsSubclassOf(baseType) == true && result.Contains(t) == false)
						result.Add(t);
				}
			}

			return result.ToArray();
		}

		public static IEnumerable<Type>	EachAssignableFrom(Type baseType, Func<Type, bool> match = null)
		{
			if (Utility.assemblyTypes == null)
				Utility.assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

			for (int i = 0; i < Utility.assemblyTypes.Length; i++)
			{
				if (baseType.IsAssignableFrom(Utility.assemblyTypes[i]) == true &&
					Utility.assemblyTypes[i].UnderlyingSystemType != baseType &&
					(match == null || match(Utility.assemblyTypes[i]) == true))
				{
					yield return Utility.assemblyTypes[i];
				}
			}
		}

		public static IEnumerable<Type>	EachSubClassesOf(Type baseType, Func<Type, bool> match = null)
		{
			if (Utility.assemblyTypes == null)
				Utility.assemblyTypes = Assembly.GetExecutingAssembly().GetTypes();

			for (int i = 0; i < Utility.assemblyTypes.Length; i++)
			{
				if (Utility.assemblyTypes[i].IsSubclassOf(baseType) == true &&
					(match == null || match(Utility.assemblyTypes[i]) == true))
				{
					yield return Utility.assemblyTypes[i];
				}
			}
		}

		public static IEnumerable<Type>	EachAllSubClassesOf(Type baseType)
		{
			if (Utility.allAssemblies == null)
				Utility.InitiateAssemblies();

			for (int i = 0; i < Utility.allAssemblies.Length; i++)
			{
				foreach (Type t in Utility.allTypes[i])
				{
					if (t.IsSubclassOf(baseType) == true)
						yield return t;
				}
			}
		}

		private static Assembly[]	allAssemblies;
		private static Type[][]		allTypes;

		/// <summary>
		/// Searches for a type looking in all assemblies from Editor and Engine.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static Type	GetType(string className)
		{
			// Remove generic symbols.
			int	n = className.IndexOf("[");

			if (n != -1)
				className = className.Substring(0, n);

			if (Utility.allAssemblies == null)
				Utility.InitiateAssemblies();

			for (int i = 0; i < Utility.allAssemblies.Length; i++)
			{
				Type	classType = Utility.allAssemblies[i].GetType(className);

				if (classType != null)
					return classType;

				foreach (Type t in Utility.allTypes[i])
				{
					if (t.FullName == className ||
						t.Name == className)
					{
						return t;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Searches for a type looking in all assemblies from Editor and Engine.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public static Type	GetType(string @namespace, string className)
		{
			// Remove generic symbols.
			int	n = className.IndexOf("[");

			if (n != -1)
				className = className.Substring(0, n);

			if (Utility.allAssemblies == null)
				Utility.InitiateAssemblies();

			string	fullName = @namespace + '.' + className;

			for (int i = 0; i < Utility.allAssemblies.Length; i++)
			{
				Type	classType = Utility.allAssemblies[i].GetType(className);

				if (classType != null)
					return classType;

				foreach (Type t in Utility.allTypes[i])
				{
					if ((t.Namespace == @namespace &&
						 t.Name == className) ||
						t.FullName == fullName)
					{
						return t;
					}
				}
			}

			return null;
		}

		private static void InitiateAssemblies()
		{
			// Look into editor assemblies.
			Assembly[] editorAssemblies = AppDomain.CurrentDomain.GetAssemblies();

			Utility.allAssemblies = new Assembly[editorAssemblies.Length + 1];
			Utility.allTypes = new Type[editorAssemblies.Length + 1][];
			// Look into game assembly.
			Utility.allAssemblies[0] = typeof(MonoBehaviour).Assembly;
			Utility.allTypes[0] = typeof(MonoBehaviour).Assembly.GetTypes();

			for (int i = 0; i < editorAssemblies.Length; i++)
			{
				Utility.allAssemblies[i + 1] = editorAssemblies[i];
				Utility.allTypes[i + 1] = editorAssemblies[i].GetTypes();
			}
		}

		public static T		LoadAssetAtPath<T>(string assetPath) where T : Object
		{
			return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
		}

		public static Rect	GetRectFrom(ref Rect r, float width)
		{
			r.width -= width;
			return new Rect(r.x + r.width, r.y, width, r.height);
		}

		public static List<FieldInfo>			GetFieldsHierarchyOrdered(Type t, Type stopType, BindingFlags flags)
		{
			var	inheritances = new Stack<Type>();
			var	fields = new List<FieldInfo>();

			inheritances.Push(t);
			if (t.BaseType != null)
			{
				while (t.BaseType != stopType)
				{
					inheritances.Push(t.BaseType);
					t = t.BaseType;
				}
			}

			foreach (var i in inheritances)
				fields.AddRange(i.GetFields(flags | BindingFlags.DeclaredOnly));

			return fields;
		}

		public static IEnumerable<FieldInfo>	EachFieldHierarchyOrdered(Type t, Type stopType, BindingFlags flags)
		{
			var	inheritances = new Stack<Type>();

			inheritances.Push(t);

			if (t.BaseType != null)
			{
				while (t.BaseType != stopType)
				{
					inheritances.Push(t.BaseType);
					t = t.BaseType;
				}
			}

			foreach (var type in inheritances)
			{
				FieldInfo[]	fields = type.GetFields(flags | BindingFlags.DeclaredOnly);

				for (int i = 0; i < fields.Length; i++)
					yield return fields[i];
			}
		}

		public static List<PropertyInfo>		GetPropertiesHierarchyOrdered(Type t, Type stopType, BindingFlags flags)
		{
			var	inheritances = new Stack<Type>();
			var	properties = new List<PropertyInfo>();

			inheritances.Push(t);
			if (t.BaseType != null)
			{
				while (t.BaseType != stopType)
				{
					inheritances.Push(t.BaseType);
					t = t.BaseType;
				}
			}

			foreach (var i in inheritances)
				properties.AddRange(i.GetProperties(flags | BindingFlags.DeclaredOnly));

			return properties;
		}

		public static IEnumerable<PropertyInfo>	EachPropertyHierarchyOrdered(Type t, Type stopType, BindingFlags flags)
		{
			var	inheritances = new Stack<Type>();

			inheritances.Push(t);
			if (t.BaseType != null)
			{
				while (t.BaseType != stopType)
				{
					inheritances.Push(t.BaseType);
					t = t.BaseType;
				}
			}

			foreach (var type in inheritances)
			{
				PropertyInfo[]	properties = type.GetProperties(flags | BindingFlags.DeclaredOnly);

				for (int i = 0; i < properties.Length; i++)
					yield return properties[i];
			}
		}

		private static EditorPrefType[]	editorPrefInstances;

		/// <summary>
		/// <para>Saves the given <paramref name="instance"/> in EditorPrefs.</para>
		/// <para>Only works on integers, unsigned integers, float, double, decimal, bool, char, byte, sbyte, byte, sbyte, string, Vector2, Vector3, Vector4, Rect, Quaternion, Color, GUIStyle, GUIStyleState, enum, Array, IList<>, Object, struct and class.</para>
		/// <para>Use NonSerializedAttribute to prevent serializing.</para>
		/// <para>Use SerializeField to serialize protected or private fields.</para>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="type"></param>
		/// <param name="path"></param>
		public static void	DirectSaveEditorPref(object instance, Type type, string path = "")
		{
			if (Utility.editorPrefInstances == null)
				Utility.CreateEditorPrefInstances();

			for (int j = 0; j < Utility.editorPrefInstances.Length; j++)
			{
				if (Utility.editorPrefInstances[j].CanHandle(type) == true)
				{
					Utility.editorPrefInstances[j].DirectSave(instance, type, path);
					break;
				}
			}
		}

		/// <summary>
		/// <para>Saves all public non-static fields of the given <paramref name="instance"/> or the <paramref name="instance"/> itself if primitive in EditorPrefs.</para>
		/// <para>Only works on integers, unsigned integers, float, double, decimal, bool, char, byte, sbyte, string, Vector2, Vector3, Vector4, Rect, Quaternion, Color, GUIStyle, GUIStyleState, enum, Array, IList<>, Object, struct and class.</para>
		/// <para>Use NonSerializedAttribute to prevent serializing.</para>
		/// <para>Use SerializeField to serialize protected or private fields.</para>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="prefix"></param>
		public static void	SaveEditorPref(object instance, string prefix = "")
		{
			if (Utility.editorPrefInstances == null)
				Utility.CreateEditorPrefInstances();

			Type	type = instance.GetType();

			for (int j = 0; j < Utility.editorPrefInstances.Length; j++)
			{
				if (Utility.editorPrefInstances[j].CanHandle(type) == true)
				{
					Utility.editorPrefInstances[j].Save(instance, type, prefix);
					break;
				}
			}
		}

		/// <summary>
		/// <para>Fetches the value from EditorPrefs.</para>
		/// <para>Only works on integers, unsigned integers, float, double, decimal, bool, char, byte, sbyte, string, Vector2, Vector3, Vector4, Rect, Quaternion, Color, GUIStyle, GUIStyleState, enum, Array, IList<>, Object, struct and class.</para>
		/// <para>Use NonSerializedAttribute to prevent serializing.</para>
		/// <para>Use SerializeField to serialize protected or private fields.</para>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="type"></param>
		/// <param name="prefix"></param>
		/// <returns></returns>
		public static object	LoadEditorPref(object instance, Type type, string prefix = "")
		{
			if (Utility.editorPrefInstances == null)
				Utility.CreateEditorPrefInstances();

			for (int j = 0; j < Utility.editorPrefInstances.Length; j++)
			{
				if (Utility.editorPrefInstances[j].CanHandle(type) == true)
					return Utility.editorPrefInstances[j].Fetch(instance, type, prefix);
			}

			return instance;
		}

		/// <summary>
		/// <para>Restores values from EditorPrefs to all public non-static fields.</para>
		/// <para>Only works on integers, unsigned integers, float, double, decimal, bool, char, byte, sbyte, string, Vector2, Vector3, Vector4, Rect, Quaternion, Color, GUIStyle, GUIStyleState, enum, Array, IList<>, Object, struct and class.</para>
		/// <para>Use NonSerializedAttribute to prevent serializing.</para>
		/// <para>Use SerializeField to serialize protected or private fields.</para>
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="prefix"></param>
		public static void	LoadEditorPref(object instance, string prefix = "")
		{
			if (Utility.editorPrefInstances == null)
				Utility.CreateEditorPrefInstances();

			Type	type = instance.GetType();

			for (int j = 0; j < Utility.editorPrefInstances.Length; j++)
			{
				if (Utility.editorPrefInstances[j].CanHandle(type) == true)
				{
					Utility.editorPrefInstances[j].Load(instance, type, prefix);
					break;
				}
			}
		}

		private static void	CreateEditorPrefInstances()
		{
			Type[]	types = Utility.GetSubClassesOf(typeof(EditorPrefType));

			Utility.editorPrefInstances = new EditorPrefType[types.Length];

			for (int i = 0, j = 0; i < types.Length; i++)
			{
				if (types[i] != typeof(EditorPrefClass) && types[i] != typeof(EditorPrefStruct))
				{
					Utility.editorPrefInstances[j] = Activator.CreateInstance(types[i]) as EditorPrefType;
					++j;
				}
			}

			Utility.editorPrefInstances[Utility.editorPrefInstances.Length - 2] = Activator.CreateInstance<EditorPrefStruct>();
			Utility.editorPrefInstances[Utility.editorPrefInstances.Length - 1] = Activator.CreateInstance<EditorPrefClass>();
		}

		/// <summary>Checks if a symbol exists in the active build target.</summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static bool	ExistSymbol(string symbol)
		{
			string	rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(Utility.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));
			int		i = rawSymbols.IndexOf(symbol);

			// Symbol not found, add it.
			if (i == -1)
				return false;

			// Check if symbol is starting.
			if (i > 0 && rawSymbols[i - 1] != ';')
				return false;

			// Check if symbol is ending.
			if (i + symbol.Length < rawSymbols.Length &&
				rawSymbols[i + symbol.Length] != ';')
				return false;

			return true;
		}

		/// <summary>Appends a symbol to the active build target.</summary>
		/// <param name="symbol"></param>
		public static void	AppendSymbol(string symbol)
		{
			BuildTargetGroup	target = Utility.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			string				rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols + ";" + symbol);
		}

		/// <summary>Removes a symbol from the active build target.</summary>
		/// <param name="symbol"></param>
		public static void	RemoveSymbol(string symbol)
		{
			BuildTargetGroup	target = Utility.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			string				rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
			int					i = rawSymbols.IndexOf(symbol);

			// Symbol not found, add it.
			if (i != -1)
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols.Substring(0, i) + rawSymbols.Substring(i + symbol.Length));
		}

		/// <summary>Toggles a symbol in the active build target.</summary>
		/// <param name="symbol"></param>
		public static void	ToggleSymbol(string symbol)
		{
			BuildTargetGroup	target = Utility.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
			string				rawSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
			int					i = rawSymbols.IndexOf(symbol);

			// Symbol not found, add it.
			if (i == -1)
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols + ";" + symbol);
			else
				PlayerSettings.SetScriptingDefineSymbolsForGroup(target, rawSymbols.Substring(0, i) + rawSymbols.Substring(i + symbol.Length));
		}

		public static BuildTargetGroup	GetBuildTargetGroup(BuildTarget buildTarget)
		{
			if (buildTarget == BuildTarget.Android)
				return BuildTargetGroup.Android;
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
			if (buildTarget == BuildTarget.BlackBerry)
				return BuildTargetGroup.BlackBerry;
#endif
#if UNITY_4
			if (buildTarget == BuildTarget.FlashPlayer)
				return BuildTargetGroup.FlashPlayer;
#endif
#if !UNITY_4
			if (buildTarget == BuildTarget.iOS)
				return BuildTargetGroup.iOS;
#endif
#if UNITY_4
			if (buildTarget == BuildTarget.iPhone)
				return BuildTargetGroup.iPhone;
			if (buildTarget == BuildTarget.MetroPlayer)
				return BuildTargetGroup.Metro;
			if (buildTarget == BuildTarget.NaCl)
				return BuildTargetGroup.NaCl;
#endif

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			if (buildTarget == BuildTarget.Nintendo3DS)
				return BuildTargetGroup.Nintendo3DS;
#elif UNITY_5_5 || UNITY_5_5_OR_NEWER
			if (buildTarget == BuildTarget.N3DS)
				return BuildTargetGroup.N3DS;
#endif
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			if (buildTarget == BuildTarget.PS3)
				return BuildTargetGroup.PS3;
#endif
			if (buildTarget == BuildTarget.PS4)
				return BuildTargetGroup.PS4;
			if (buildTarget == BuildTarget.PSM)
				return BuildTargetGroup.PSM;
			if (buildTarget == BuildTarget.PSP2)
				return BuildTargetGroup.PSP2;
			if (buildTarget == BuildTarget.SamsungTV)
				return BuildTargetGroup.SamsungTV;
			if (
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
				buildTarget == BuildTarget.StandaloneGLESEmu ||
#endif
				buildTarget == BuildTarget.StandaloneLinux ||
				buildTarget == BuildTarget.StandaloneLinux64 ||
				buildTarget == BuildTarget.StandaloneLinuxUniversal ||
				buildTarget == BuildTarget.StandaloneOSXIntel ||
				buildTarget == BuildTarget.StandaloneOSXIntel64 ||
				buildTarget == BuildTarget.StandaloneOSXUniversal ||
				buildTarget == BuildTarget.StandaloneWindows ||
				buildTarget == BuildTarget.StandaloneWindows64)
			{
				return BuildTargetGroup.Standalone;
			}
			if (buildTarget == BuildTarget.Tizen)
				return BuildTargetGroup.Tizen;
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
			if (buildTarget == BuildTarget.tvOS)
				return BuildTargetGroup.tvOS;
#endif
#if !UNITY_4
			if (buildTarget == BuildTarget.WebGL)
				return BuildTargetGroup.WebGL;
#endif
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
			if (buildTarget == BuildTarget.WebPlayer ||
				buildTarget == BuildTarget.WebPlayerStreamed)
			{
				return BuildTargetGroup.WebPlayer;
			}
#endif
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1
			if (buildTarget == BuildTarget.WiiU)
				return BuildTargetGroup.WiiU;
#endif
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3
			if (buildTarget == BuildTarget.WP8Player)
				return BuildTargetGroup.WP8;
#endif
#if !UNITY_4
			if (buildTarget == BuildTarget.WSAPlayer)
				return BuildTargetGroup.WSA;
#endif
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			if (buildTarget == BuildTarget.XBOX360)
				return BuildTargetGroup.XBOX360;
#endif
			if (buildTarget == BuildTarget.XboxOne)
				return BuildTargetGroup.XboxOne;

			return BuildTargetGroup.Unknown;
		}

		public static void	Append(this StringBuilder sb, string content, Color color)
		{
			sb.AppendStartColor(color);
			sb.Append(content);
			sb.AppendEndColor();
		}

		public static void	AppendStartColor(this StringBuilder sb, Color color)
		{
			sb.Append("<color=#");
			sb.Append(((int)(color.r * 255)).ToString("X2"));
			sb.Append(((int)(color.g * 255)).ToString("X2"));
			sb.Append(((int)(color.b * 255)).ToString("X2"));
			sb.Append(((int)(color.a * 255)).ToString("X2"));
			sb.Append('>');
		}

		public static void	AppendEndColor(this StringBuilder sb)
		{
			sb.Append("</color>");
		}

		public static string	Color(string content, Color color)
		{
			return "<color=#" + ((int)(color.r * 255)).ToString("X2") + ((int)(color.g * 255)).ToString("X2") + ((int)(color.b * 255)).ToString("X2") + ((int)(color.a * 255)).ToString("X2") + ">" + content + "</color>";
		}

		private static string	cachedConsolePath = string.Empty;

		/// <summary>
		/// Gets the relative path of NG Tools folder from the project.
		/// </summary>
		/// <returns></returns>
		public static string	GetPackagePath()
		{
			if (string.IsNullOrEmpty(Utility.cachedConsolePath) == true)
			{
				string[]	dirs = Directory.GetDirectories("Assets", Constants.RootFolderName, SearchOption.AllDirectories);

				for (int i = 0; i < dirs.Length; i++)
				{
					int	chances = 0;

					if (Directory.Exists(Path.Combine(dirs[i], Constants.RelativeLocaleFolder)) == true)
						++chances;
					if (Directory.Exists(Path.Combine(dirs[i], "NGConsole")) == true)
						++chances;
					if (chances < 2 && Directory.Exists(Path.Combine(dirs[i], "NGGameConsole")) == true)
						++chances;
					if (chances < 2 && Directory.Exists(Path.Combine(dirs[i], "Test")) == true)
						++chances;

					// Set the path anyway.
					Utility.cachedConsolePath = dirs[i].Replace('\\', '/');

					// But break on the highest potential.
					if (chances >= 2)
						break;
				}
			}

			return Utility.cachedConsolePath;
		}

		/// <summary>
		/// <para>Returns the given <paramref name="texture"/> if not null.</para>
		/// <para>Otherwise gets a Texture asset from AssetDatabase using <paramref name="settings"/>, <paramref name="folder"/> and <paramref name="fileName"/>.</para>
		/// <para>If none is found, it calls the callback <paramref name="defaultTexture"/> to generate a new texture and saves it to AssetDatabse and returns it.</para>
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="texture">A texture.</param>
		/// <param name="folder">Path of the texture to save on disk. Relative to the given ConsoleSetting.</param>
		/// <param name="fileName">Filename of the texture without extension. ".asset" will be appended.</param>
		/// <param name="defaultTexture">A function generating a Texture if the requesting asset is not found.</param>
		/// <returns></returns>
		public static Texture2D	GetRefFromAssetDatabase(NGSettings settings, Texture2D texture, string folder, string fileName, Func<Texture2D> defaultTexture)
		{
			if (texture == null)
			{
				try
				{
					string	assetPath = AssetDatabase.GetAssetPath(settings);

					if (string.IsNullOrEmpty(assetPath) == true)
						throw new Exception("The given instance of NGSettings is not an asset in the project.");

					string	texturePath = Directory.GetParent(assetPath).FullName.Substring(Application.dataPath.Length - 6) +
						Path.DirectorySeparatorChar +
						folder +
						Path.DirectorySeparatorChar;

					if (Directory.Exists(texturePath) == false)
						Directory.CreateDirectory(texturePath);

					texturePath = Path.Combine(texturePath, fileName + ".asset");

					// Find texture if exists.
					texture = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
					if (texture == null)
					{
						// Generates an asset with the default texture.
						texture = defaultTexture();

						AssetDatabase.CreateAsset(texture, texturePath);

						var	asset = AssetDatabase.LoadMainAssetAtPath(texturePath);

						texture = EditorUtility.InstanceIDToObject(asset.GetInstanceID()) as Texture2D;
					}
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
					texture = new Texture2D(1, 1);
					texture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			return texture;
		}

		/// <summary>
		/// Converts a base-64 raw string into a Texture2D.
		/// </summary>
		/// <param name="raw"></param>
		/// <returns></returns>
		public static Texture2D	CreateTexture(string raw)
		{
			Texture2D	texture = new Texture2D(1, 1);
			texture.LoadImage(Convert.FromBase64String(raw));
			return texture;
		}

		public static Texture2D	CreateDotTexture(float r, float g, float b, float a)
		{
			Texture2D	texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, new Color(r, g, b, a));
			texture.Apply();
			return texture;
		}

		/// <summary>
		/// Returns a Texture2D from the given <paramref name="name"/> using method LoadIcon from EditorGUIUtility.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Texture2D	GetConsoleIcon(string name)
		{
			return typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { name }) as Texture2D;
		}

		/// <summary>Launches an executable with arguments.</summary>
		/// <param name="editorPath"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public static bool	OpenFileLine(string editorPath, string arguments, ProcessWindowStyle windowStyle = ProcessWindowStyle.Hidden, bool createNoWindow = true)
		{
			try
			{
				Process	myProcess = new Process();

				myProcess.StartInfo.WindowStyle = windowStyle;
				myProcess.StartInfo.CreateNoWindow = createNoWindow;
				myProcess.StartInfo.UseShellExecute = false;
				myProcess.StartInfo.FileName = editorPath;
				myProcess.StartInfo.Arguments = arguments;
				myProcess.Start();

				return true;
			}
			catch (Exception e)
			{
				InternalNGDebug.LogException(e);
			}
			return false;
		}

		private static Type	consoleWindowType;

		/// <summary>Calls Repaint from Unity's console if available.</summary>
		public static void	RepaintConsoleWindow()
		{
			if (Utility.consoleWindowType == null)
				Utility.consoleWindowType = typeof(InternalEditorUtility).Assembly.GetType("UnityEditor.ConsoleWindow");

			if (Utility.consoleWindowType != null)
				Utility.RepaintEditorWindow(Utility.consoleWindowType);
		}

		public static void	RepaintEditorWindow(Type windowType)
		{
			Object[]	windows = Resources.FindObjectsOfTypeAll(windowType);

			for (int i = 0; i < windows.Length; i++)
				(windows[i] as EditorWindow).Repaint();
		}

		private static Type	preferencesWindowType;

		/// <summary>Opens Preferences Window at the given <paramref name="preferenceItem"/>.</summary>
		/// <param name="preferenceItem">The title of the Preferences Window used in attribute PreferenceItem.</param>
		public static void	ShowPreferencesWindowAt(string preferenceItem)
		{
			if (Utility.preferencesWindowType == null)
				Utility.preferencesWindowType = typeof(InternalEditorUtility).Assembly.GetType("UnityEditor.PreferencesWindow");

			if (Utility.preferencesWindowType != null)
			{
				EditorWindow.GetWindow(Utility.preferencesWindowType, true, "Unity Preferences");
				Object[]	windows = Resources.FindObjectsOfTypeAll(Utility.preferencesWindowType);

				if (windows.Length > 0)
				{
					FieldInfo	m_RefreshCustomPreferences = Utility.preferencesWindowType.GetField("m_RefreshCustomPreferences", BindingFlags.Instance | BindingFlags.NonPublic);

					if (m_RefreshCustomPreferences != null)
					{
						// Force PreferencesWindow to load custom sections before setting m_SelectedSectionIndex.
						if ((bool)m_RefreshCustomPreferences.GetValue(windows[0]) == true)
						{
							MethodInfo	AddCustomSections = Utility.preferencesWindowType.GetMethod("AddCustomSections", BindingFlags.Instance | BindingFlags.NonPublic);
							if (AddCustomSections != null)
							{
								AddCustomSections.Invoke(windows[0], new object[] { });
								m_RefreshCustomPreferences.SetValue(windows[0], false);
							}
							else
								return;
						}
					}

					FieldInfo	m_Sections = Utility.preferencesWindowType.GetField("m_Sections", BindingFlags.Instance | BindingFlags.NonPublic);
					if (m_Sections != null)
					{
						IEnumerable	sections = m_Sections.GetValue(windows[0]) as IEnumerable;
						if (sections != null)
						{
							int	i = 0;
							foreach (object element in sections)
							{
								FieldInfo	contentField = element.GetType().GetField("content", BindingFlags.Instance | BindingFlags.Public);
								GUIContent	content = contentField.GetValue(element) as GUIContent;

								if (content.text == preferenceItem)
								{
									contentField = Utility.preferencesWindowType.GetField("m_SelectedSectionIndex", BindingFlags.Instance | BindingFlags.NonPublic);
									contentField.SetValue(windows[0], i);
									break;
								}
								++i;
							}
						}
					}
				}
			}
		}

		public static IEnumerable<T>	GetCustomAttributesIncludingBaseInterfaces<T>(this Type type)
		{
			var	attributeType = typeof(T);
			return type.GetCustomAttributes(attributeType, true).Union(type.GetInterfaces().SelectMany(interfaceType => interfaceType.GetCustomAttributes(attributeType, true))).Distinct().Cast<T>();
		}

		public static byte[]	SerializeField(object field)
		{
			using (MemoryStream	ms = new MemoryStream())
			{
				BinaryFormatter	bin = new BinaryFormatter();
				bin.Serialize(ms, field);
				return ms.ToArray();
			}
		}

		public static T			DeserializeField<T>(byte[] raw)
		{
			using (MemoryStream	ms = new MemoryStream(raw))
			{
				BinaryFormatter	bin = new BinaryFormatter();
				return (T)bin.Deserialize(ms);
			}
		}

		public static object	DeserializeField(byte[] raw)
		{
			using (MemoryStream	ms = new MemoryStream(raw))
			{
				BinaryFormatter	bin = new BinaryFormatter();
				return bin.Deserialize(ms);
			}
		}

		private static Texture2D	prefabIcon;

		/// <summary>
		/// Gets the cached icon or tries to find it. The returned icon might be null.
		/// </summary>
		/// <returns></returns>
		public static Texture2D	GetIcon(int instanceID)
		{
			if (instanceID != 0)
			{
				Object	asset = EditorUtility.InstanceIDToObject(instanceID);

				if (asset != null)
				{
					Texture2D	icon = null;

					// Unfortunately, GameObject does not have an proper icon.
					if (asset is GameObject)
					{
						if (Utility.prefabIcon == null)
							Utility.prefabIcon = InternalEditorUtility.GetIconForFile(".prefab");
						if (Utility.prefabIcon != null)
							return Utility.prefabIcon;
						//Debug.Log("GetIconForFile(prefab)");
					}

					icon = AssetPreview.GetMiniThumbnail(asset);
					//Debug.Log("AssetPreview.GetMiniThumbnail");
					if (icon != null && icon.name != "DefaultAsset Icon")
						return icon;

					string	path = AssetDatabase.GetAssetPath(asset);

					if (string.IsNullOrEmpty(path) == false)
					{
						icon = InternalEditorUtility.GetIconForFile(path);
						//Debug.Log("GetIconForFile(path)");
						if (icon != null)
							return icon;
					}

					icon = EditorGUIUtility.ObjectContent(asset, asset.GetType()).image as Texture2D;
					//Debug.Log("EditorGUIUtility.ObjectContent");
					if (icon != null)
						return icon;

					MethodInfo	method = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

					if (method != null)
					{
						object	obj = method.Invoke(null, new object[] { asset });
						//Debug.Log("Method" + " " + obj);
						icon = (Texture2D)obj;
						if (icon != null)
							return icon;
					}
				}
			}

			return null;
		}

		[SetColor(.4F, .4F, .0F, .95F, .9F, .9F, .9F, .95F)]
		private static Color	dropZoneOutline = default(Color);
		[SetColor(.2F, .2F, .2F, .95F, .7F, .7F, .7F, .95F)]
		private static Color	dropZoneBackground = default(Color);
		private static GUIStyle	dynamicFontSizeCenterText;

		/// <summary>
		/// Draws a drop area where drag&drop should be handled.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="message"></param>
		public static void		DropZone(Rect r, string message)
		{
			if (Utility.dynamicFontSizeCenterText == null)
			{
				Utility.dynamicFontSizeCenterText = new GUIStyle(GUI.skin.label);
				Utility.dynamicFontSizeCenterText.wordWrap = true;
				Utility.dynamicFontSizeCenterText.alignment = TextAnchor.MiddleCenter;
			}

			EditorGUI.DrawRect(r, Utility.dropZoneBackground);

			Rect	r2 = r;
			r2.xMin += 2F;
			r2.xMax -= 2F;
			r2.yMin += 2F;
			r2.yMax -= 2F;

			Utility.DrawUnfillRect(r2, Utility.dropZoneOutline);

			Utility.dynamicFontSizeCenterText.fontSize = 15;

			// Shrink title to fit the space.
			Utility.content.text = message;
			while (Utility.dynamicFontSizeCenterText.CalcSize(Utility.content).x >= r.width &&
				   Utility.dynamicFontSizeCenterText.fontSize > 9)
			{
				--Utility.dynamicFontSizeCenterText.fontSize;
			}

			using (ColorContentRestorer.Get(UnityEngine.Color.cyan))
			{
				GUI.Label(r, message, Utility.dynamicFontSizeCenterText);
			}
		}

		/// <summary>Processes width of one element in a field with many, relying on EditorGUIUtility.labelWidth.</summary>
		/// <param name="totalWidth">Width of the whole field in inspector.</param>
		/// <param name="elementMinWidth">Minimum width an element can have.</param>
		/// <param name="elementCount">Number of sub-elements.</param>
		/// <param name="labelWidth">Output label width.</param>
		/// <param name="subElementsWidth">Output total sub-elements width.</param>
		public static void	CalculSubFieldsWidth(float totalWidth, float elementMinWidth, int elementCount, out float labelWidth, out float subElementsWidth)
		{
			float	totalElementsWidth = elementMinWidth * elementCount;

			if (totalWidth < EditorGUIUtility.labelWidth + totalElementsWidth)
			{
				labelWidth = totalWidth - totalElementsWidth;
				subElementsWidth = elementMinWidth;
			}
			else
			{
				labelWidth = EditorGUIUtility.labelWidth;
				subElementsWidth = (totalWidth - labelWidth) / elementCount;
			}
		}

		/// <summary>
		/// Creates a new inspector window instance and locks it to inspect the specified target
		/// </summary>
		/// <remarks>Thank to vexe at: http://answers.unity3d.com/questions/36131/editor-multiple-inspectors.html</remarks>
		public static EditorWindow	InspectTarget(Object target)
		{
			// Get a reference to the `InspectorWindow` type object
			var	inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
			// Create an InspectorWindow instance
			var	inspectorInstance = ScriptableObject.CreateInstance(inspectorType) as EditorWindow;
			// We display it - currently, it will inspect whatever gameObject is currently selected
			// So we need to find a way to let it inspect/aim at our target GO that we passed
			// For that we do a simple trick:
			// 1- Cache the current selected gameObject
			// 2- Set the current selection to our target GO (so now all inspectors are targeting it)
			// 3- Lock our created inspector to that target
			// 4- Fallback to our previous selection
			//inspectorInstance.Show(false);
			// Cache previous selected gameObject
			var	prevSelection = Selection.activeGameObject;
			// Set the selection to GO we want to inspect
			Selection.activeObject = target;
			// Get a ref to the "locked" property, which will lock the state of the inspector to the current inspected target
			var	isLocked = inspectorType.GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
			// Invoke `isLocked` setter method passing 'true' to lock the inspector
			isLocked.GetSetMethod().Invoke(inspectorInstance, new object[] { true });
			// Finally revert back to the previous selection so that other inspectors continue to inspect whatever they were inspecting...
			Selection.activeGameObject = prevSelection;

			return inspectorInstance;
		}

		public static string	GetHierarchyStringified(Transform transform)
		{
			StringBuilder	buffer = Utility.GetBuffer();
			Stack<string>	hierarchy = new Stack<string>(4);

			while (transform != null)
			{
				hierarchy.Push(transform.name);
				transform = transform.parent;
			}

			while (hierarchy.Count > 0)
			{
				buffer.Append(hierarchy.Pop());
				buffer.Append('/');
			}

			buffer.Length -= 1;
			buffer.Append(Environment.NewLine);

			if (buffer.Length > Environment.NewLine.Length)
				buffer.Length -= Environment.NewLine.Length;

			return Utility.ReturnBuffer(buffer);
		}

		public static void		StartBackgroundTask(IEnumerator update, Action end = null)
		{
			EditorApplication.CallbackFunction	closureCallback = null;

			closureCallback = () =>
			{
				try
				{
					if (update.MoveNext() == false)
					{
						if (end != null)
							end();
						EditorApplication.update -= closureCallback;
					}
				}
				catch (Exception ex)
				{
					if (end != null)
						end();
					InternalNGDebug.LogException(ex);
					EditorApplication.update -= closureCallback;
				}
			};

			EditorApplication.update += closureCallback;
		}

		private class ProgressBarTask
		{
			public Action<object>	action;
			public string			content;
			public double			endTime;
			public float			lifetime;
			public string			abortMessage;
			public Thread			thread;
		}

		private static List<ProgressBarTask>	progressBarTasks = new List<ProgressBarTask>();

		private  static void	UpdateProgressBars()
		{
			StringBuilder	buffer = Utility.GetBuffer();
			float			globalProgress = 0F;
			int				total = 0;

			// Around 32 chars max, smartly share the amount between tasks.

			lock (Utility.progressBarTasks)
			{
				for (int i = 0; i < Utility.progressBarTasks.Count; i++)
				{
					var	task = Utility.progressBarTasks[i];

					Utility.AsyncProgressBarDisplay(task.content, 0F);

					if (task.thread.IsAlive == true && task.endTime > EditorApplication.timeSinceStartup)
					{
						if (i > 0)
							buffer.Append(" | ");

						buffer.Append(task.content);

						float	rate = 1F - ((float)(task.endTime - EditorApplication.timeSinceStartup) / task.lifetime);

						if (Utility.progressBarTasks.Count >= 2)
							buffer.Append((rate * 100F).ToString(" ###\\%"));

						globalProgress += rate;
						++total;
					}
					else
					{
						Utility.progressBarTasks.RemoveAt(i);

						if (Utility.progressBarTasks.Count == 0)
						{
							Utility.AsyncProgressBarClear();
							EditorApplication.update -= Utility.UpdateProgressBars;
							return;
						}

						if (task.thread.IsAlive == true)
						{
							if (string.IsNullOrEmpty(task.abortMessage) == false)
								InternalNGDebug.LogWarning(task.abortMessage);

							task.thread.Abort();
							task.thread.Join();
						}
					}
				}
			}

			Utility.AsyncProgressBarDisplay(Utility.ReturnBuffer(buffer), globalProgress / total);
		}

		public static void	StartAsyncBackgroundTask(Action<object> callback, string progressBarString, float lifetime, string abortMessage = null)
		{
			lock (Utility.progressBarTasks)
			{
				if (Utility.progressBarTasks.Count == 0)
					EditorApplication.update += Utility.UpdateProgressBars;

				ProgressBarTask	task = new ProgressBarTask() {
					action = callback,
					content = progressBarString,
					endTime = EditorApplication.timeSinceStartup + lifetime,
					lifetime = lifetime,
					abortMessage = abortMessage,
					thread = new Thread(new ParameterizedThreadStart(callback))
				};

				Utility.progressBarTasks.Add(task);

				task.thread.Start(task);
				//Utility.StartBackgroundTask(Utility.StartThreadedCallback(task));
			}
		}

		private static IEnumerator	StartThreadedCallback(ProgressBarTask task)
		{
			Utility.AsyncProgressBarDisplay(task.content, 0F);

			//task.thread.Start();

			while (task.thread.IsAlive == true && task.endTime > EditorApplication.timeSinceStartup)
			{
				Debug.Log(Utility.GetAsyncProgressBarInfo());
				Utility.AsyncProgressBarDisplay(task.content, 1F - ((float)(task.endTime - EditorApplication.timeSinceStartup) / task.lifetime));
				yield return null;
			}

			Utility.AsyncProgressBarClear();
			
			lock (Utility.progressBarTasks)
			{
				if (Utility.progressBarTasks.Count == 1)
				{
					Utility.progressBarTasks.Clear();
					EditorApplication.update -= Utility.UpdateProgressBars;
				}

				if (task.thread.IsAlive == true)
				{
					if (string.IsNullOrEmpty(task.abortMessage) == false)
						InternalNGDebug.LogWarning(task.abortMessage);

					task.thread.Abort();
					task.thread.Join();
				}
			}
		}

		private class CallbackSchedule
		{
			public Action	action;
			public int		intervalTicks;
			public int		ticksLeft;
		}

		private static List<CallbackSchedule>	schedules = new List<CallbackSchedule>();

		public static void	RegisterIntervalCallback(Action action, int ticks)
		{
			if (Utility.schedules == null || Utility.schedules.Count == 0)
				EditorApplication.update += Utility.UpdateWindows;

			for (int i = 0; i < Utility.schedules.Count; i++)
			{
				if (Utility.schedules[i].action == action)
				{
					Utility.schedules[i].ticksLeft = ticks;
					Utility.schedules[i].intervalTicks = ticks;
					return;
				}
			}

			Utility.schedules.Add(new CallbackSchedule() { action = action, ticksLeft = ticks, intervalTicks = ticks });
		}

		public static void	UnregisterIntervalCallback(Action action)
		{
			for (int i = 0; i < Utility.schedules.Count; i++)
			{
				if (Utility.schedules[i].action == action)
				{
					Utility.schedules.RemoveAt(i);

					if (Utility.schedules.Count == 0)
						EditorApplication.update -= Utility.UpdateWindows;

					break;
				}
			}
		}

		private static void	UpdateWindows()
		{
			for (int i = 0; i < Utility.schedules.Count; i++)
			{
				if (--Utility.schedules[i].ticksLeft <= 0)
				{
					Utility.schedules[i].ticksLeft = Utility.schedules[i].intervalTicks;
					Utility.schedules[i].action();
				}
			}
		}

		private static string[]		cachedMenuItems;
		private static List<string>	customMenuItems = new List<string>();

		public static void	AddMenuItemPicker(string menuItem)
		{
			Utility.customMenuItems.Add(menuItem);
			Utility.cachedMenuItems = null;
		}

		public static void	RemoveMenuItemPicker(string menuItem)
		{
			Utility.customMenuItems.Remove(menuItem);
			Utility.cachedMenuItems = null;
		}

		public static string[]	GetAllMenuItems()
		{
			if (Utility.cachedMenuItems == null)
			{
				List<string>	menuItems = new List<string>();

				// Preload items.

				//menuItems.Add("File/New Scene");
				//menuItems.Add("File/Open Scene");
				//menuItems.Add("File/Save Scene");
				//menuItems.Add("File/Save Scene as...");
				//menuItems.Add("File/New Project...");
				//menuItems.Add("File/Open Project...");
				//menuItems.Add("File/Save Project");
				//menuItems.Add("File/Build Settings...");
				//menuItems.Add("File/Build & Run");
				//menuItems.Add("File/Exit");
				//menuItems.Add("Edit/Cut");
				//menuItems.Add("Edit/Copy");
				//menuItems.Add("Edit/Paste");
				//menuItems.Add("Edit/Duplicate");
				//menuItems.Add("Edit/Delete");
				//menuItems.Add("Edit/Frame Selected");
				//menuItems.Add("Edit/Lock View to Selected");
				//menuItems.Add("Edit/Find");
				//menuItems.Add("Edit/Select All");
				//menuItems.Add("Edit/Preferences...");
				//menuItems.Add("Edit/Modules...");
				menuItems.Add("Edit/Play");
				menuItems.Add("Edit/Pause");
				menuItems.Add("Edit/Step");
#if UNITY_4
				menuItems.Add("Edit/Render Settings");
#endif
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("Edit/Sign in...");
				menuItems.Add("Edit/Sign out");
#endif

				menuItems.Add("Edit/Selection/Load Selection 1");
				menuItems.Add("Edit/Selection/Load Selection 2");
				menuItems.Add("Edit/Selection/Load Selection 3");
				menuItems.Add("Edit/Selection/Load Selection 4");
				menuItems.Add("Edit/Selection/Load Selection 5");
				menuItems.Add("Edit/Selection/Load Selection 6");
				menuItems.Add("Edit/Selection/Load Selection 7");
				menuItems.Add("Edit/Selection/Load Selection 8");
				menuItems.Add("Edit/Selection/Load Selection 9");
				menuItems.Add("Edit/Selection/Save Selection 1");
				menuItems.Add("Edit/Selection/Save Selection 2");
				menuItems.Add("Edit/Selection/Save Selection 3");
				menuItems.Add("Edit/Selection/Save Selection 4");
				menuItems.Add("Edit/Selection/Save Selection 5");
				menuItems.Add("Edit/Selection/Save Selection 6");
				menuItems.Add("Edit/Selection/Save Selection 7");
				menuItems.Add("Edit/Selection/Save Selection 8");
				menuItems.Add("Edit/Selection/Save Selection 9");
				menuItems.Add("Edit/Selection/Save Selection 0");

				menuItems.Add("Edit/Project Settings/Input");
				menuItems.Add("Edit/Project Settings/Tags and Layers");
				menuItems.Add("Edit/Project Settings/Audio");
				menuItems.Add("Edit/Project Settings/Time");
				menuItems.Add("Edit/Project Settings/Player");
				menuItems.Add("Edit/Project Settings/Physics");
				menuItems.Add("Edit/Project Settings/Physics 2D");
				menuItems.Add("Edit/Project Settings/Quality");
				menuItems.Add("Edit/Project Settings/Graphics");
				menuItems.Add("Edit/Project Settings/Network");
				menuItems.Add("Edit/Project Settings/Editor");
				menuItems.Add("Edit/Project Settings/Script Execution Order");

				menuItems.Add("Edit/Network Emulation/None");
				menuItems.Add("Edit/Network Emulation/Broadband");
				menuItems.Add("Edit/Network Emulation/DSL");
				menuItems.Add("Edit/Network Emulation/ISDN");
				menuItems.Add("Edit/Network Emulation/Dial-Up");

				menuItems.Add("Edit/Graphics Emulation/No Emulation");
				menuItems.Add("Edit/Graphics Emulation/Shader Model 2");
				menuItems.Add("Edit/Graphics Emulation/Shader Model 3");
#if UNITY_5_4
				menuItems.Add("Edit/Graphics Emulation/Shader Hardware Tier 1");
				menuItems.Add("Edit/Graphics Emulation/Shader Hardware Tier 2");
				menuItems.Add("Edit/Graphics Emulation/Shader Hardware Tier 3");
#endif
				menuItems.Add("Edit/Snap Settings...");

				menuItems.Add("Assets/Create/Folder");
				menuItems.Add("Assets/Create/C# Script");
				menuItems.Add("Assets/Create/Javascript");
#if UNITY_4
				menuItems.Add("Assets/Create/Boo Script");
#endif
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Create/Editor Test C# Script");
#endif
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Create/Shader/Standard Surface Shader");
#if UNITY_5_4
				menuItems.Add("Assets/Create/Shader/Standard Surface Shader (Instanced)");
#endif
				menuItems.Add("Assets/Create/Shader/Unlit Shader");
				menuItems.Add("Assets/Create/Shader/Image Effect Shader");
				menuItems.Add("Assets/Create/Shader/Compute Shader");
#if UNITY_5_4
				menuItems.Add("Assets/Create/Shader/Shader Variant Collection");
#endif
#else
				menuItems.Add("Assets/Create/Shader");
				menuItems.Add("Assets/Create/Compute Shader");
#endif
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Create/Scene");
#endif
				menuItems.Add("Assets/Create/Prefab");
#if UNITY_5
				menuItems.Add("Assets/Create/Audio Mixer");
#endif
				menuItems.Add("Assets/Create/Material");
#if UNITY_4
				menuItems.Add("Assets/Create/Cubemap");
#endif
				menuItems.Add("Assets/Create/Lens Flare");
				menuItems.Add("Assets/Create/Render Texture");
#if UNITY_5
				menuItems.Add("Assets/Create/Lightmap Parameters");
#endif
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Create/Sprites/Square");
				menuItems.Add("Assets/Create/Sprites/Triangle");
				menuItems.Add("Assets/Create/Sprites/Diamond");
				menuItems.Add("Assets/Create/Sprites/Hexagon");
				menuItems.Add("Assets/Create/Sprites/Circle");
				menuItems.Add("Assets/Create/Sprites/Polygon");
#endif
				menuItems.Add("Assets/Create/Animator Controller");
				menuItems.Add("Assets/Create/Animation");
				menuItems.Add("Assets/Create/Animator Override Controller");
				menuItems.Add("Assets/Create/Avatar Mask");
				menuItems.Add("Assets/Create/Physic Material");
				menuItems.Add("Assets/Create/Physics2D Material");
				menuItems.Add("Assets/Create/GUI Skin");
				menuItems.Add("Assets/Create/Custom Font");
#if UNITY_5
				menuItems.Add("Assets/Create/Shader Variant Collection");
				menuItems.Add("Assets/Create/Legacy/Cubemap");
#endif
				menuItems.Add("Assets/Show in Explorer");
				menuItems.Add("Assets/Open");
				menuItems.Add("Assets/Delete");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Open Scene Additive");
#endif
				menuItems.Add("Assets/Import New Asset...");
				menuItems.Add("Assets/Import Package/Custom Package...");
				menuItems.Add("Assets/Export Package...");
				menuItems.Add("Assets/Find References In Scene");
				menuItems.Add("Assets/Select Dependencies");
				menuItems.Add("Assets/Refresh");
				menuItems.Add("Assets/Reimport");
				menuItems.Add("Assets/Reimport All");
#if UNITY_5
				menuItems.Add("Assets/Run API Updater...");
#endif
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Assets/Open C# Project");
#else
				menuItems.Add("Assets/Sync MonoDevelop Project");
#endif
				menuItems.Add("GameObject/Create Empty");
				menuItems.Add("GameObject/Create Empty Child");

#if UNITY_4_5
				menuItems.Add("GameObject/Create Other/Particle System");
				menuItems.Add("GameObject/Create Other/Camera");
				menuItems.Add("GameObject/Create Other/GUI Text");
				menuItems.Add("GameObject/Create Other/GUI Texture");
				menuItems.Add("GameObject/Create Other/3D Text");
				menuItems.Add("GameObject/Create Other/Directional Light");
				menuItems.Add("GameObject/Create Other/Point Light");
				menuItems.Add("GameObject/Create Other/Spotlight");
				menuItems.Add("GameObject/Create Other/Area Light");
				menuItems.Add("GameObject/Create Other/Cube");
				menuItems.Add("GameObject/Create Other/Sphere");
				menuItems.Add("GameObject/Create Other/Capsule");
				menuItems.Add("GameObject/Create Other/Cylinder");
				menuItems.Add("GameObject/Create Other/Plane");
				menuItems.Add("GameObject/Create Other/Quad");
				menuItems.Add("GameObject/Create Other/Sprite");
				menuItems.Add("GameObject/Create Other/Cloth");
				menuItems.Add("GameObject/Create Other/Audio Reverb Zone");
				menuItems.Add("GameObject/Create Other/Terrain");
				menuItems.Add("GameObject/Create Other/Ragdoll...");
				menuItems.Add("GameObject/Create Other/Tree");
				menuItems.Add("GameObject/Create Other/Wind Zone");
#else
				menuItems.Add("GameObject/3D Object/Cube");
				menuItems.Add("GameObject/3D Object/Sphere");
				menuItems.Add("GameObject/3D Object/Capsule");
				menuItems.Add("GameObject/3D Object/Cylinder");
				menuItems.Add("GameObject/3D Object/Plane");
				menuItems.Add("GameObject/3D Object/Quad");
				menuItems.Add("GameObject/3D Object/Ragdoll...");
#if UNITY_4
				menuItems.Add("GameObject/3D Object/Cloth");
#endif
				menuItems.Add("GameObject/3D Object/Terrain");
				menuItems.Add("GameObject/3D Object/Tree");
				menuItems.Add("GameObject/3D Object/Wind Zone");
#if UNITY_5
				menuItems.Add("GameObject/3D Object/3D Text");
#endif

				menuItems.Add("GameObject/2D Object/Sprite");

				menuItems.Add("GameObject/Light/Directional Light");
				menuItems.Add("GameObject/Light/Point Light");
				menuItems.Add("GameObject/Light/Spotlight");
				menuItems.Add("GameObject/Light/Area Light");
#if UNITY_5
				menuItems.Add("GameObject/Light/Reflection Probe");
				menuItems.Add("GameObject/Light/Light Probe Group");
#endif

				menuItems.Add("GameObject/Audio/Audio Source");
				menuItems.Add("GameObject/Audio/Audio Reverb Zone");

				menuItems.Add("GameObject/UI/Text");
				menuItems.Add("GameObject/UI/Image");
				menuItems.Add("GameObject/UI/Raw Image");
				menuItems.Add("GameObject/UI/Button");
				menuItems.Add("GameObject/UI/Toggle");
				menuItems.Add("GameObject/UI/Slider");
				menuItems.Add("GameObject/UI/Scrollbar");
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("GameObject/UI/Dropdown");
#endif
#if UNITY_4
				menuItems.Add("GameObject/UI/InputField");
#endif
#if UNITY_5
				menuItems.Add("GameObject/UI/Input Field");
#endif
				menuItems.Add("GameObject/UI/Canvas");
				menuItems.Add("GameObject/UI/Panel");
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("GameObject/UI/Scroll View");
#endif
				menuItems.Add("GameObject/UI/Event System");

				menuItems.Add("GameObject/Particle System");
				menuItems.Add("GameObject/Camera");
#endif
				menuItems.Add("GameObject/Center On Children");
				menuItems.Add("GameObject/Make Parent");
				menuItems.Add("GameObject/Clear Parent");
				menuItems.Add("GameObject/Apply Changes To Prefab");
				menuItems.Add("GameObject/Break Prefab Instance");
				menuItems.Add("GameObject/Set as first sibling");
				menuItems.Add("GameObject/Set as last sibling");
				menuItems.Add("GameObject/Move To View");
				menuItems.Add("GameObject/Align With View");
				menuItems.Add("GameObject/Align View to Selected");
				menuItems.Add("GameObject/Toggle Active State");

				menuItems.Add("Component/Add...");
				menuItems.Add("Component/Mesh/Mesh Filter");
				menuItems.Add("Component/Mesh/Text Mesh");
				menuItems.Add("Component/Mesh/Mesh Renderer");
#if UNITY_5
				menuItems.Add("Component/Mesh/Skinned Mesh Renderer");
#endif

				menuItems.Add("Component/Effects/Particle System");
				menuItems.Add("Component/Effects/Trail Renderer");
				menuItems.Add("Component/Effects/Line Renderer");
				menuItems.Add("Component/Effects/Lens Flare");
				menuItems.Add("Component/Effects/Halo");
				menuItems.Add("Component/Effects/Projector");

				menuItems.Add("Component/Effects/Legacy Particles/Ellipsoid Particle Emitter");
				menuItems.Add("Component/Effects/Legacy Particles/Mesh Particle Emitter");
				menuItems.Add("Component/Effects/Legacy Particles/Particle Animator");
				menuItems.Add("Component/Effects/Legacy Particles/World Particle Collider");
				menuItems.Add("Component/Effects/Legacy Particles/Particle Renderer");

				menuItems.Add("Component/Physics/Rigidbody");
				menuItems.Add("Component/Physics/Character Controller");
				menuItems.Add("Component/Physics/Box Collider");
				menuItems.Add("Component/Physics/Sphere Collider");
				menuItems.Add("Component/Physics/Capsule Collider");
				menuItems.Add("Component/Physics/Mesh Collider");
				menuItems.Add("Component/Physics/Wheel Collider");
				menuItems.Add("Component/Physics/Terrain Collider");
#if UNITY_5
				menuItems.Add("Component/Physics/Cloth");
#else
				menuItems.Add("Component/Physics/Interactive Cloth");
				menuItems.Add("Component/Physics/Skinned Cloth");
				menuItems.Add("Component/Physics/Cloth Renderer");
#endif
				menuItems.Add("Component/Physics/Hinge Joint");
				menuItems.Add("Component/Physics/Fixed Joint");
				menuItems.Add("Component/Physics/Spring Joint");
				menuItems.Add("Component/Physics/Character Joint");
				menuItems.Add("Component/Physics/Configurable Joint");
				menuItems.Add("Component/Physics/Constant Force");

				menuItems.Add("Component/Physics 2D/Rigidbody 2D");
				menuItems.Add("Component/Physics 2D/Box Collider 2D");
				menuItems.Add("Component/Physics 2D/Circle Collider 2D");
				menuItems.Add("Component/Physics 2D/Edge Collider 2D");
				menuItems.Add("Component/Physics 2D/Polygon Collider 2D");
				menuItems.Add("Component/Physics 2D/Distance Joint 2D");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/Physics 2D/Fixed Joint 2D");
				menuItems.Add("Component/Physics 2D/Friction Joint 2D");
#endif
				menuItems.Add("Component/Physics 2D/Hinge Joint 2D");
#if UNITY_5
				menuItems.Add("Component/Physics 2D/Relative Joint 2D");
#endif
				menuItems.Add("Component/Physics 2D/Slider Joint 2D");
				menuItems.Add("Component/Physics 2D/Spring Joint 2D");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/Physics 2D/Target Joint 2D");
#endif
				menuItems.Add("Component/Physics 2D/Wheel Joint 2D");
#if UNITY_5
				menuItems.Add("Component/Physics 2D/Area Effector 2D");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/Physics 2D/Buoyancy Effector 2D");
#endif
				menuItems.Add("Component/Physics 2D/Point Effector 2D");
				menuItems.Add("Component/Physics 2D/Platform Effector 2D");
				menuItems.Add("Component/Physics 2D/Surface Effector 2D");
				menuItems.Add("Component/Physics 2D/Constant Force 2D");
#endif
				menuItems.Add("Component/Navigation/Nav Mesh Agent");
				menuItems.Add("Component/Navigation/Off Mesh Link");
				menuItems.Add("Component/Navigation/Nav Mesh Obstacle");

				menuItems.Add("Component/Audio/Audio Listener");
				menuItems.Add("Component/Audio/Audio Source");
				menuItems.Add("Component/Audio/Audio Reverb Zone");
				menuItems.Add("Component/Audio/Audio Low Pass Filter");
				menuItems.Add("Component/Audio/Audio High Pass Filter");
				menuItems.Add("Component/Audio/Audio Echo Filter");
				menuItems.Add("Component/Audio/Audio Distortion Filter");
				menuItems.Add("Component/Audio/Audio Reverb Filter");
				menuItems.Add("Component/Audio/Audio Chorus Filter");

				menuItems.Add("Component/Rendering/Camera");
				menuItems.Add("Component/Rendering/Skybox");
				menuItems.Add("Component/Rendering/Flare Layer");
#if UNITY_4
				menuItems.Add("Component/Rendering/GUILayer");
#else
				menuItems.Add("Component/Rendering/GUI Layer");
#endif
				menuItems.Add("Component/Rendering/Light");
				menuItems.Add("Component/Rendering/Light Probe Group");
#if UNITY_5
				menuItems.Add("Component/Rendering/Reflection Probe");
#endif
				menuItems.Add("Component/Rendering/Occlusion Area");
				menuItems.Add("Component/Rendering/Occlusion Portal");
#if UNITY_4
				menuItems.Add("Component/Rendering/LODGroup");
#else
				menuItems.Add("Component/Rendering/LOD Group");
#endif
				menuItems.Add("Component/Rendering/Sprite Renderer");
#if UNITY_4
#if UNITY_4_6 || UNITY_4_7
				menuItems.Add("Component/Rendering/Canvas Renderer");
#endif
				menuItems.Add("Component/Rendering/GUITexture");
				menuItems.Add("Component/Rendering/GUIText");
#else
				menuItems.Add("Component/Rendering/GUI Texture");
				menuItems.Add("Component/Rendering/GUI Text");
#endif

#if UNITY_4_6 || UNITY_5
				menuItems.Add("Component/Layout/Rect Transform");
				menuItems.Add("Component/Layout/Canvas");
				menuItems.Add("Component/Layout/Canvas Group");
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_1 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/Layout/Canvas Scaler");
				menuItems.Add("Component/Layout/Layout Element");
				menuItems.Add("Component/Layout/Content Size Fitter");
				menuItems.Add("Component/Layout/Aspect Ratio Fitter");
				menuItems.Add("Component/Layout/Horizontal Layout Group");
				menuItems.Add("Component/Layout/Vertical Layout Group");
				menuItems.Add("Component/Layout/Grid Layout Group");
#endif
#endif
				menuItems.Add("Component/Miscellaneous/Animator");
				menuItems.Add("Component/Miscellaneous/Animation");
				menuItems.Add("Component/Miscellaneous/Network View");
				menuItems.Add("Component/Miscellaneous/Wind Zone");
#if UNITY_4_5
				menuItems.Add("Component/Miscellaneous/Canvas");
				menuItems.Add("Component/Miscellaneous/Rect Transform");
#endif
#if UNITY_5
				menuItems.Add("Component/Miscellaneous/Terrain");
				menuItems.Add("Component/Miscellaneous/Billboard Renderer");
#endif
#if UNITY_5_1
				menuItems.Add("Component/Miscellaneous/Cloud Service Handler Behaviour");
#endif
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_1 || UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/Event/Event System");
				menuItems.Add("Component/Event/Event Trigger");
				menuItems.Add("Component/Event/Physics 2D Raycaster");
				menuItems.Add("Component/Event/Physics Raycaster");
				menuItems.Add("Component/Event/Standalone Input Module");
				menuItems.Add("Component/Event/Touch Input Module");
				menuItems.Add("Component/Event/Graphic Raycaster");

#if UNITY_5
				menuItems.Add("Component/Network/NetworkAnimator");
				menuItems.Add("Component/Network/NetworkDiscovery");
				menuItems.Add("Component/Network/NetworkIdentity");
				menuItems.Add("Component/Network/NetworkLobbyManager");
				menuItems.Add("Component/Network/NetworkLobbyPlayer");
				menuItems.Add("Component/Network/NetworkManager");
				menuItems.Add("Component/Network/NetworkManagerHUD");
				menuItems.Add("Component/Network/NetworkMigrationManager");
				menuItems.Add("Component/Network/NetworkProximityChecker");
				menuItems.Add("Component/Network/NetworkStartPosition");
				menuItems.Add("Component/Network/NetworkTransform");
				menuItems.Add("Component/Network/NetworkTransformChild");
				menuItems.Add("Component/Network/NetworkTransformVisualizer");
#endif
				menuItems.Add("Component/UI/Effects/Shadow");
				menuItems.Add("Component/UI/Effects/Outline");
				menuItems.Add("Component/UI/Effects/Position As UV1");
				menuItems.Add("Component/UI/Text");
				menuItems.Add("Component/UI/Image");
				menuItems.Add("Component/UI/Raw Image");
				menuItems.Add("Component/UI/Mask");
#if UNITY_5_3
				menuItems.Add("Component/UI/2D Rect Mask");
#elif UNITY_5_4
				menuItems.Add("Component/UI/Rect Mask 2D");
#endif
				menuItems.Add("Component/UI/Button");
				menuItems.Add("Component/UI/Input Field");
				menuItems.Add("Component/UI/Toggle");
				menuItems.Add("Component/UI/Toggle Group");
				menuItems.Add("Component/UI/Slider");
				menuItems.Add("Component/UI/Scrollbar");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Component/UI/Dropdown");
#endif
				menuItems.Add("Component/UI/Scroll Rect");
				menuItems.Add("Component/UI/Selectable");
#endif
				//menuItems.Add("Window/Next Window");
				//menuItems.Add("Window/Previous Window");
				menuItems.Add("Window/Layouts/2 by 3");
				menuItems.Add("Window/Layouts/4 Split");
				menuItems.Add("Window/Layouts/Default");
				menuItems.Add("Window/Layouts/Tall");
				menuItems.Add("Window/Layouts/Wide");
				menuItems.Add("Window/Layouts/Save Layout...");
				menuItems.Add("Window/Layouts/Delete Layout...");
				menuItems.Add("Window/Layouts/Revert Factory Settings...");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Window/Services");
#elif UNITY_5_2
				menuItems.Add("Window/Unity Services");
#endif
				menuItems.Add("Window/Scene");
				menuItems.Add("Window/Game");
				menuItems.Add("Window/Inspector");
				menuItems.Add("Window/Hierarchy");
				menuItems.Add("Window/Project");
				menuItems.Add("Window/Animation");
				menuItems.Add("Window/Profiler");
#if UNITY_5
				menuItems.Add("Window/Audio Mixer");
#endif
				menuItems.Add("Window/Asset Store");
				menuItems.Add("Window/Version Control");
				menuItems.Add("Window/Animator");
#if UNITY_5
				menuItems.Add("Window/Animator Parameter");
#endif
				menuItems.Add("Window/Sprite Packer");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Window/Editor Tests Runner");
#endif
#if UNITY_4
				menuItems.Add("Window/Lightmapping");
#else
				menuItems.Add("Window/Lighting");
#endif
				menuItems.Add("Window/Occlusion Culling");
#if UNITY_5
				menuItems.Add("Window/Frame Debugger");
#endif
				menuItems.Add("Window/Navigation");
				menuItems.Add("Window/Console");

				//menuItems.Add("Help/About Unity...");
				//menuItems.Add("Help/Manage License...");
				menuItems.Add("Help/Unity Manual");
				menuItems.Add("Help/Scripting Reference");
#if UNITY_5_3 || UNITY_5_4
				menuItems.Add("Help/Unity Services");
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				menuItems.Add("Help/Unity Connect");
#endif
				menuItems.Add("Help/Unity Forum");
				menuItems.Add("Help/Unity Answers");
				menuItems.Add("Help/Unity Feedback");
#if UNITY_4
				menuItems.Add("Help/Welcome Screen");
#endif
				menuItems.Add("Help/Check for Updates");
				menuItems.Add("Help/Download Beta...");
				menuItems.Add("Help/Release Notes");
				menuItems.Add("Help/Report a Bug...");
				menuItems.AddRange(Utility.customMenuItems);

				//foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				//{
				//	foreach (var type in asm.GetTypes())
				//	{
				//		foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
				//		{
				//			MenuItem[]	attributes = method.GetCustomAttributes(typeof(MenuItem), false) as MenuItem[];

				//			if (attributes.Length > 0 && attributes[0].validate == false && attributes[0].menuItem.StartsWith("CONTEXT/") == false)
				//			{
				//				int	n = attributes[0].menuItem.LastIndexOf(" ");

				//				if (n != -1)
				//				{
				//					if (attributes[0].menuItem[n + 1] == '%' ||
				//						attributes[0].menuItem[n + 1] == '#' ||
				//						attributes[0].menuItem[n + 1] == '&' ||
				//						attributes[0].menuItem[n + 1] == '_')
				//					{
				//						menuItems.Add(attributes[0].menuItem.Substring(0, n));
				//					}
				//					else
				//						menuItems.Add(attributes[0].menuItem);
				//				}
				//				else
				//					menuItems.Add(attributes[0].menuItem);
				//			}
				//		}
				//	}
				//}

				//EditorApplication.ExecuteMenuItem(item);

				Utility.cachedMenuItems = menuItems.ToArray();
			}

			return Utility.cachedMenuItems;
		}

		public static Rect	FullRect = new Rect(0F, 0F, 1F, 1F);

		private static Material blendMaterial = Utility.GetBlendMaterial();

		private static Material	GetBlendMaterial()
		{
			EditorApplication.delayCall += () => {
				Utility.blendMaterial = typeof(GUI).GetProperty("blendMaterial", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).GetValue(null, null) as Material;
			};
			return null;
		}

		public static void	DrawLine(Vector2 a, Vector2 b, Color c)
		{
			using (HandlesColorRestorer.Get(c))
			{
				Handles.BeginGUI();
				Handles.DrawLine(a, b);
				Handles.EndGUI();
			}
		}

		public static void	DrawRect(Rect r, Color c)
		{
			Graphics.DrawTexture(r, EditorGUIUtility.whiteTexture, Utility.FullRect, 0, 0, 0, 0, c, Utility.blendMaterial);
		}

		public static void	DrawUnfillRect(Rect r, Color c)
		{
			float	x = r.x;
			float	xMax = r.xMax;
			float	w = r.width;
			float	h = r.height;

			// Top border
			r.height = 1F;
			Graphics.DrawTexture(r, EditorGUIUtility.whiteTexture, Utility.FullRect, 0, 0, 0, 0, c, Utility.blendMaterial);

			// Left border
			r.width = 1F;
			r.height = h;
			Graphics.DrawTexture(r, EditorGUIUtility.whiteTexture, Utility.FullRect, 0, 0, 0, 0, c, Utility.blendMaterial);

			// Right border
			r.x = xMax - 1F;
			Graphics.DrawTexture(r, EditorGUIUtility.whiteTexture, Utility.FullRect, 0, 0, 0, 0, c, Utility.blendMaterial);

			// Bottom border
			r.x = x;
			r.y += r.height - 1F;
			r.width = w;
			r.height = 1F;
			Graphics.DrawTexture(r, EditorGUIUtility.whiteTexture, Utility.FullRect, 0, 0, 0, 0, c, Utility.blendMaterial);
		}

		public const float			HeaderHeight = 43F; // Double of the real height, don't know why.
		public const float			Space = .02F;
		private static Vector3[]	Positions = new Vector3[] { new Vector3(), new Vector3(), new Vector3(), new Vector3() };
#if !UNITY_4 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
		private static int[]		PositionIndexes = new int[] { 0, 1, 1, 2, 2, 3, 3, 0 };
#endif

		public static void	DrawRectDotted(Rect r, Rect position, Color c, float space = Utility.Space, float headerHeight = Utility.HeaderHeight)
		{
			Handles.BeginGUI();
			{
				float	heightRate = (position.height - headerHeight) / position.height;
				float	YMax = 1F + heightRate;

				float	x = -1 + (r.x * 2 / position.width);
				float	y = heightRate - (r.y * YMax / position.height);
				float	w = -1 + ((r.x + r.width) * 2 / position.width);
				float	h = heightRate - ((r.y + r.height) * YMax / position.height);

				using (HandlesColorRestorer.Get(c))
				using (HandlesMatrix4x4Restorer.Get(Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one)))
				{
					Utility.Positions[0].x = x;
					Utility.Positions[0].y = y;
					Utility.Positions[1].x = x;
					Utility.Positions[1].y = h;
					Utility.Positions[2].x = w;
					Utility.Positions[2].y = h;
					Utility.Positions[3].x = w;
					Utility.Positions[3].y = y;
#if UNITY_4 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					Handles.DrawDottedLine(Utility.Positions[0], Utility.Positions[1], Utility.Space);
					Handles.DrawDottedLine(Utility.Positions[1], Utility.Positions[2], Utility.Space);
					Handles.DrawDottedLine(Utility.Positions[2], Utility.Positions[3], Utility.Space);
					Handles.DrawDottedLine(Utility.Positions[3], Utility.Positions[0], Utility.Space);
#else
					Handles.DrawDottedLines(Utility.Positions, Utility.PositionIndexes, Utility.Space);
#endif
				}
			}
			Handles.EndGUI();
		}

		public static Type[]	GetAllDerivedTypes(this AppDomain aAppDomain, Type aType)
		{
			var	result = new List<Type>();
			var	assemblies = aAppDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var	types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.IsSubclassOf(aType))
						result.Add(type);
				}
			}
			return result.ToArray();
		}

		private static bool			loadedOnce;
		private static Type			containerWinType;
		private static FieldInfo	showModeField;
		private static PropertyInfo	positionProperty;

		private static object	mainWindow;

		public static Rect	GetEditorMainWindowPos()
		{
			if (loadedOnce == false)
			{
				loadedOnce = true;
				containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).Where(t => t.Name == "ContainerWindow").FirstOrDefault();
				if (containerWinType == null)
					throw new MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");

				showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
				if (showModeField == null)
					throw new MissingFieldException("Can't find internal fields 'm_ShowMode'. Maybe something has changed inside Unity");

				positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
				if (positionProperty == null)
					throw new MissingFieldException("Can't find internal fields 'position'. Maybe something has changed inside Unity");
			}

			if (containerWinType == null || showModeField == null || positionProperty == null)
				return default(Rect);

			if (mainWindow == null || Object.Equals(mainWindow, "null") == true)
			{
				var	windows = Resources.FindObjectsOfTypeAll(containerWinType);
				foreach (var win in windows)
				{
					var	showmode = (int)showModeField.GetValue(win);
					if (showmode == 4) // main window
					{
						mainWindow = win;
						break;
					}
				}
			}

			if (mainWindow != null)
				return (Rect)positionProperty.GetValue(mainWindow, null);

			throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
		}

		public static void	CenterOnMainWin(this UnityEditor.EditorWindow aWin)
		{
			var	main = GetEditorMainWindowPos();
			var	pos = aWin.position;
			float	w = (main.width - pos.width) * 0.5f;
			float	h = (main.height - pos.height) * 0.5f;
			pos.x = main.x + w;
			pos.y = main.y + h;
			aWin.position = pos;
		}

		public static void	SetTitle(this EditorWindow window, string title)
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				window.title = title;
#else
				window.titleContent = new GUIContent(title);
#endif
		}

		public static string	GetTitle(this EditorWindow window)
		{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0
				return window.title;
#else
				return window.titleContent.text;
#endif
		}

		public static void	ShowExplorer(string itemPath)
		{
			EditorUtility.RevealInFinder(itemPath);
		}

		public static void	AddNGMenuItems(GenericMenu menu, EditorWindow window, string helpLabel, string helpURL, bool isNGSettings = false)
		{
			menu.AddItem(new GUIContent(Preferences.Title), false, () => Utility.ShowPreferencesWindowAt(Constants.PackageTitle));
			if (isNGSettings == false)
			{
				menu.AddItem(new GUIContent(NGSettingsWindow.Title), false, () => {
#if UNITY_5
					EditorWindow.GetWindow<NGSettingsWindow>(NGSettingsWindow.Title, true, window.GetType()).Focus(window.GetTitle());
#else
					EditorWindow.GetWindow<NGSettingsWindow>(NGSettingsWindow.Title, true).Focus(window.GetTitle());
#endif
				});
			}

			if (string.IsNullOrEmpty(helpURL) == false)
				menu.AddItem(new GUIContent("Help for " + helpLabel), false, () => Application.OpenURL(helpURL));

			if (Conf.DebugMode != Conf.DebugModes.None)
				menu.AddItem(new GUIContent("Open NGRealTimeEditorDebug"), false, () => EditorWindow.GetWindow<NGRealTimeEditorDebug>(NGRealTimeEditorDebug.Title));
		}

		/// <summary>
		/// Simply searches a keyword into the line starting at i.
		/// </summary>
		/// <param name="line"></param>
		/// <param name="keyword"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		private static bool	Compare(string line, string keyword, int i)
		{
			int	j = 0;

			for (; i < line.Length && j < keyword.Length; i++, j++)
			{
				if (line[i] != keyword[j])
					return false;
			}

			// Check if keyword matches.
			if (j == keyword.Length)
			{
				// Check if keyword is a whole word.
				if (('a' <= keyword[0] && keyword[0] <= 'z') ||
					('A' <= keyword[0] && keyword[0] <= 'Z'))
				{
					// Check char after keyword's end.
					if (i < line.Length)
					{
						if ((line[i] < 'a' || line[i] > 'z') &&
							(line[i] < 'A' || line[i] > 'Z') &&
							(line[i] < '0' || line[i] > '9') &&
							line[i] != '_')
						{
							// Then check char before.
							if (i - j - 1 >= 0)
							{
								i -= j + 1;
								return ((line[i] < 'a' || line[i] > 'z') &&
										(line[i] < 'A' || line[i] > 'Z') &&
										(line[i] < '0' || line[i] > '9') &&
										line[i] != '_');
							}
						}
						else
							return false;
					}
					// Otherwise only check before.
					else if (i - j - 1 >= 0)
					{
						i -= j + 1;
						return ((line[i] < 'a' || line[i] > 'z') &&
								(line[i] < 'A' || line[i] > 'Z') &&
								(line[i] < '0' || line[i] > '9') &&
								line[i] != '_');
					}
				}

				return true;
			}

			return false;
		}

		// Thanks to Dave Swersky at http://stackoverflow.com/questions/3354893/how-can-i-convert-a-datetime-to-the-number-of-seconds-since-1970
		public static DateTime	ConvertFromUnixTimestamp(double timestamp)
		{
			DateTime	origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return origin.AddSeconds(timestamp);
		}

		public static double	ConvertToUnixTimestamp(DateTime date)
		{
			DateTime	origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan	diff = date.ToUniversalTime() - origin;
			return Math.Floor(diff.TotalSeconds);
		}

		public static bool	IsStruct(this Type t)
		{
			return InnerUtility.IsStruct(t);
		}

		private static PropertyInfo	AsyncProgressBarProgressInfo;
		private static MethodInfo	AsyncProgressBarDisplayMethod;
		private static MethodInfo	AsyncProgressBarClearMethod;

		public static string	GetAsyncProgressBarInfo()
		{
			if (Utility.AsyncProgressBarProgressInfo == null)
				Utility.AsyncProgressBarProgressInfo = typeof(EditorWindow).Assembly.GetType("UnityEditor.AsyncProgressBar").GetProperty("progressInfo", BindingFlags.Static | BindingFlags.Public);

			if (Utility.AsyncProgressBarProgressInfo != null)
				return Utility.AsyncProgressBarProgressInfo.GetValue(null, null) as string;
			return string.Empty;
		}

		public static void	AsyncProgressBarDisplay(string progressInfo, float progress)
		{
			if (Utility.AsyncProgressBarDisplayMethod == null)
				Utility.AsyncProgressBarDisplayMethod = typeof(EditorWindow).Assembly.GetType("UnityEditor.AsyncProgressBar").GetMethod("Display", BindingFlags.Static | BindingFlags.Public);

			if (Utility.AsyncProgressBarDisplayMethod != null)
				Utility.AsyncProgressBarDisplayMethod.Invoke(null, new object[] { progressInfo, progress });
		}

		public static void	AsyncProgressBarClear()
		{
			if (Utility.AsyncProgressBarClearMethod == null)
				Utility.AsyncProgressBarClearMethod = typeof(EditorWindow).Assembly.GetType("UnityEditor.AsyncProgressBar").GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);

			if (Utility.AsyncProgressBarClearMethod != null)
				Utility.AsyncProgressBarClearMethod.Invoke(null, null);
		}

		public static string	NicifyVariableName(string name)
		{
			if (string.IsNullOrEmpty(name) == true)
				return name;

			StringBuilder	buffer = Utility.GetBuffer();

			if (name[0] >= 'a' && name[0] <= 'z')
				buffer.Append((char)(name[0] + 'A' - 'a'));
			else
				buffer.Append(name[0]);

			for (int i = 1; i < name.Length; i++)
			{
				if (name[i] >= '1' && name[i] <= '9')
				{
					if (i > 0 && name[i - 1] >= 'a' && name[i - 1] <= 'z')
						buffer.Append(' ');
				}
				else if (name[i] >= 'A' && name[i] <= 'Z')
				{
					if (i + 1 == name.Length) // Last letter
					{
						if (i > 0 && name[i - 1] >= 'a' && name[i - 1] <= 'z')
							buffer.Append(' ');
					}
					else
					{
						if (name[i + 1] >= 'a' && name[i + 1] <= 'z')
						{
							if (((name[i - 1] >= 'a' && name[i - 1] <= 'z') || (name[i - 1] >= 'A' && name[i - 1] <= 'Z') || (name[i - 1] >= '0' && name[i - 1] <= '9')))
								buffer.Append(' ');
						}
						else if (name[i - 1] >= 'a' && name[i - 1] <= 'z')
							buffer.Append(' ');
					}
				}

				buffer.Append(name[i]);
			}

			return Utility.ReturnBuffer(buffer);
		}
	}
}