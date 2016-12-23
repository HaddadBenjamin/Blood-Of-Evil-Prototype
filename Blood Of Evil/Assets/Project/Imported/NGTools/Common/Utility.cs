﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NGTools
{
	using UnityEngine;

	public static class Utility
	{
		public const BindingFlags	ExposedBindingFlags = BindingFlags.Public/* | BindingFlags.NonPublic*/ | BindingFlags.Instance;

		public static ByteBuffer	sharedBBuffer = new ByteBuffer(128);
		public static StringBuilder	sharedBuffer = new StringBuilder(128);

		private static Type[]	assemblyTypes;

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

		/// <summary>
		/// Gets all fields exposable in Inspector.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="exposers"></param>
		/// <returns></returns>
		public static FieldInfo[]		GetExposedFields(Type type, ComponentExposer[] exposers)
		{
			Stack<Type>		inheritances = new Stack<Type>();
			List<FieldInfo>	fields = new List<FieldInfo>();

			inheritances.Push(type);
			while (type.BaseType != typeof(Component) && type.BaseType != typeof(object))
			{
				inheritances.Push(type.BaseType);
				type = type.BaseType;
			}

			foreach (Type ty in inheritances)
			{
				if (exposers != null)
				{
					int	j = 0;

					for (; j < exposers.Length; j++)
					{
						if (exposers[j].type == ty)
						{
							fields.AddRange(exposers[j].GetFieldInfos());
							break;
						}
					}

					if (j < exposers.Length)
						continue;
				}

				FieldInfo[]	fis = ty.GetFields(Utility.ExposedBindingFlags | BindingFlags.DeclaredOnly);

				for (int i = 0; i < fis.Length; i++)
				{
					if (fis[i].GetCustomAttributes(typeof(HideInInspector), false).Length > 0 ||
						Utility.CanExposeTypeInInspector(fis[i].FieldType) == false)
					{
						continue;
					}

					fields.Add(fis[i]);
				}
			}

			return fields.ToArray();
		}

		/// <summary>
		/// Gets all properties exposable in Inspector.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="exposers"></param>
		/// <returns></returns>
		public static PropertyInfo[]	GetExposedProperties(Type type, ComponentExposer[] exposers)
		{
			Stack<Type>			inheritances = new Stack<Type>();
			List<PropertyInfo>	properties = new List<PropertyInfo>();

			inheritances.Push(type);
			while (type.BaseType != typeof(Component) && type.BaseType != typeof(object))
			{
				inheritances.Push(type.BaseType);
				type = type.BaseType;
			}

			foreach (Type ty in inheritances)
			{
				if (exposers != null)
				{
					int	j = 0;

					for (; j < exposers.Length; j++)
					{
						if (exposers[j].type == ty)
						{
							properties.AddRange(exposers[j].GetPropertyInfos());
							break;
						}
					}

					if (j < exposers.Length)
						continue;
				}

				PropertyInfo[]	pis = ty.GetProperties(Utility.ExposedBindingFlags | BindingFlags.DeclaredOnly);

				for (int i = 0; i < pis.Length; i++)
				{
					if (pis[i].GetCustomAttributes(typeof(HideInInspector), false).Length > 0 ||
						pis[i].GetIndexParameters().Length != 0 || // Skip indexer.
						pis[i].CanRead == false || // Skip prop without both get/set.
						pis[i].CanWrite == false ||
						Utility.CanExposeTypeInInspector(pis[i].PropertyType) == false)
					{
						continue;
					}

					properties.Add(pis[i]);
				}
			}

			return properties.ToArray();
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
			float dir = Vector3.Dot(perp, up);

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
			// TODO Check if types from external plugins work.
			// TODO Test in Unix.
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
	}
}