using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NGTools
{
	using UnityEngine;

	public static partial class Utility
	{
		public const BindingFlags	ExposedBindingFlags = BindingFlags.Public | BindingFlags.Instance;

		public static ByteBuffer	sharedBBuffer = new ByteBuffer(128);
		public static StringBuilder	sharedBuffer = new StringBuilder(128);

		private static Dictionary<Assembly, Type[]>	assemblyTypes = new Dictionary<Assembly, Type[]>();

		public static IEnumerable<Type>	EachAssignableFrom(Type baseType, Func<Type, bool> match = null)
		{
			Type[]	types;

			if (Utility.assemblyTypes.TryGetValue(baseType.Assembly, out types) == false)
			{
				types = baseType.Assembly.GetTypes();
				Utility.assemblyTypes[baseType.Assembly] = types;
			}

			for (int i = 0; i < types.Length; i++)
			{
				if (baseType.IsAssignableFrom(types[i]) == true &&
					types[i].UnderlyingSystemType != baseType &&
					(match == null || match(types[i]) == true))
				{
					yield return types[i];
				}
			}
		}

		public static IEnumerable<Type>	EachSubClassesOf(Type baseType, Func<Type, bool> match = null)
		{
			Type[]	types;

			if (Utility.assemblyTypes.TryGetValue(baseType.Assembly, out types) == false)
			{
				types = baseType.Assembly.GetTypes();
				Utility.assemblyTypes[baseType.Assembly] = types;
			}

			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsSubclassOf(baseType) == true &&
					(match == null || match(types[i]) == true))
				{
					yield return types[i];
				}
			}
		}

		public static bool	CanExposeTypeInInspector(Type type)
		{
			if (type.IsInterface == false &&
				(type.IsPrimitive == true || // Any primitive types (int, float, byte, etc... not struct or decimal).
				 type == typeof(string) || // Built-in types.
				 type.IsEnum == true ||
				 type == typeof(Rect) ||
				 type == typeof(Vector3) ||
				 type == typeof(Color) ||
				 typeof(Object).IsAssignableFrom(type) == true || // Unity Object.
				 type == typeof(Object) ||
				 type == typeof(Vector2) ||
				 type == typeof(Vector4) ||
				 type == typeof(Quaternion) ||
				 type == typeof(AnimationCurve)))
			{
				return true;
			}

			if (type.GetInterface(typeof(IList<>).Name) != null) // IList<> with Serializable elements.
			{
				Type	subType = type.GetInterface(typeof(IList<>).Name).GetGenericArguments()[0];

				// Nested arrays are not supported.
				if (subType.GetInterface(typeof(IList<>).Name) == null)
					return Utility.CanExposeTypeInInspector(subType);
				return false;
			}

			if (typeof(IList).IsAssignableFrom(type) == true) // IList with Serializable elements.
			{
				return false;
				//PropertyInfo prop = type.GetProperty("Item", new Type[] { typeof(int) });
				//return (prop != null && Utility.CanExposeInInspector(prop.PropertyType) == true);
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) // Especially, we prevent Dictionary to be exposed.
				return false;

			// We need to check if type is a class or struct even after all the checks before. Because types like Decimal have Serializable and are not primitive. Thank you CLR and 64bits implementation limitation.
			if (type != typeof(Decimal) &&
				(type.IsClass == true || // A class.
				 type.IsStruct() == true) && // Or a struct.
				type.GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0) // With SerializableAttribute.
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Gets element's type of array supported by Unity Inspector.
		/// </summary>
		/// <param name="arrayType"></param>
		/// <returns></returns>
		/// <seealso cref="TypeExtension.IsUnityArray"/>
		public static Type	GetArraySubType(Type arrayType)
		{
			if (arrayType.IsArray == true)
				return arrayType.GetElementType();

			Type	@interface = arrayType.GetInterface(typeof(IList<>).Name);
			if (@interface != null) // IList<> with Serializable elements.
				return @interface.GetGenericArguments()[0];

			return null;
		}

		public static List<FieldInfo>	GetFieldsHierarchyOrdered(Type t, Type stopType, BindingFlags flags)
		{
			var	inheritances = new Stack<Type>();
			var	fields = new List<FieldInfo>();

			inheritances.Push(t);
			while (t.BaseType != stopType)
			{
				inheritances.Push(t.BaseType);
				t = t.BaseType;
			}

			foreach (var i in inheritances)
				fields.AddRange(i.GetFields(flags | BindingFlags.DeclaredOnly));

			return fields;
		}

#if UNITY_4_5 ||UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
		// Thanks to Baconaise @ http://answers.unity3d.com/questions/266244/how-can-i-add-copypaste-clipboard-support-to-my-ga.html
		private static PropertyInfo	m_systemCopyBufferProperty = null;
		private static PropertyInfo	GetSystemCopyBufferProperty()
		{
			 if (Utility.m_systemCopyBufferProperty == null)
			 {
				Utility. m_systemCopyBufferProperty = typeof(GUIUtility).GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
				if (Utility.m_systemCopyBufferProperty == null)
					throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
			 }
			 return Utility.m_systemCopyBufferProperty;
		}
#endif

		public static string	ClipBoard
		{
			get 
			{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
				return (string)GetSystemCopyBufferProperty().GetValue(null, null);
#else
				return GUIUtility.systemCopyBuffer;
#endif
			}
			set
			{
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
				GetSystemCopyBufferProperty().SetValue(null, value, null);
#else
				GUIUtility.systemCopyBuffer = value;
#endif
			}
		}

		public static float		RelativeAngle(Vector3 fwd, Vector3 targetDir, Vector3 upDir)
		{
			var	angle = Vector3.Angle(fwd, targetDir);

			if (Utility.AngleDirection(fwd, targetDir, upDir) == -1)
				return -angle;
			else
				return angle;
		}
		
		public static float		RelativeAngle(Vector2 fwd, Vector2 targetDir, Vector3 upDir)
		{
			var	angle = Vector2.Angle(fwd, targetDir);

			if (Utility.AngleDirection(fwd, targetDir, upDir) == -1)
				return -angle;
			else
				return angle;
		}

		public static float		AngleDirection(Vector3 fwd, Vector3 targetDir, Vector3 up)
		{
			Vector3	perp = Vector3.Cross(fwd, targetDir);
			float	dir = Vector3.Dot(perp, up);

			if (dir > 0F)
				return 1F;
			else if (dir < 0F)
				return -1F;
			else
				return 0F;
		}

		public static float		AngleDirection(Vector2 fwd, Vector2 targetDir, Vector3 up)
		{
			Vector3	perp = Vector3.Cross(new Vector3(fwd.x, 0f, fwd.y),
										 new Vector3(targetDir.x, 0f, targetDir.y));
			float	dir = Vector3.Dot(perp, up);

			if (dir > 0F)
				return 1F;
			else if (dir < 0F)
				return -1F;
			else
				return 0F;
		}

		/// <summary>
		/// Defines if a type is an array supported by Unity inspector.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool	IsUnityArray(this Type t)
		{
			return t.IsArray == true || typeof(IList).IsAssignableFrom(t) == true;
		}

		public static bool	IsStruct(this Type t)
		{
			return t.IsValueType == true && t.IsPrimitive == false && t.IsEnum == false && t != typeof(Decimal);
		}

		public static string	GetShortAssemblyType(this Type t)
		{
			if (t.Assembly.FullName.StartsWith("mscorlib") == false)
				return t.FullName + "," + t.Assembly.FullName.Substring(0, t.Assembly.FullName.IndexOf(','));
			return t.FullName;
		}

		/// <summary>
		/// Looks for a field or a property using <paramref name="name"/> in <paramref name="type"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name">The name of a field or a property.</param>
		/// <returns></returns>
		/// <exception cref="System.MissingFieldException"></exception>
		public static IFieldModifier	GetFieldInfo(Type type, string name)
		{
			FieldInfo	fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (fieldInfo != null)
				return new FieldModifier(fieldInfo);

			PropertyInfo	propertyInfo = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (propertyInfo != null)
				return new PropertyModifier(propertyInfo);

			throw new MissingFieldException("Field or property \"" + name + "\" was not found in type \"" + type.Name + "\".");
		}

		private static List<ICollectionModifier>	cachedCollectionModifiers = new List<ICollectionModifier>();

		public static ICollectionModifier	GetCollectionModifier(object rawArray)
		{
			Array	array = rawArray as Array;

			if (array != null)
			{
				for (int i = 0; i < Utility.cachedCollectionModifiers.Count; i++)
				{
					ArrayModifier	instance = Utility.cachedCollectionModifiers[i] as ArrayModifier;

					if (instance != null)
					{
						instance.array = array;
						Utility.cachedCollectionModifiers.RemoveAt(i);
						return instance;
					}
				}

				return new ArrayModifier(array);
			}

			IList	list = rawArray as IList;

			if (list != null)
			{
				for (int i = 0; i < Utility.cachedCollectionModifiers.Count; i++)
				{
					ListModifier	instance = Utility.cachedCollectionModifiers[i] as ListModifier;

					if (instance != null)
					{
						instance.list = list;
						Utility.cachedCollectionModifiers.RemoveAt(i);
						return instance;
					}
				}

				return new ListModifier(list);
			}

			throw new Exception("Collection of type \"" + rawArray.GetType() + "\" is not supported.");
		}

		public static void	ReturnCollectionModifier(ICollectionModifier modifier)
		{
			Utility.cachedCollectionModifiers.Add(modifier);
		}

		public static bool	IsComponentEnableable(Component component)
		{
			Type	componentType = component.GetType();

			if (component is Behaviour)
			{
				if (componentType.GetMethod("Start", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("FixedUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null)
				{
					return true;
				}
				else
					return false;
			}
			else
			{
				if (componentType.GetProperty("enabled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null)
					return true;
				else
					return false;
			}
		}
	}
}