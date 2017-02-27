using System;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	using UnityEngine;

	public enum FieldInnerValueType
	{
		Null,
		ValueType,
		RefType,
		StructType,
		UnityObject,
		ArrayValueType,
		ArrayRefType,
		ArrayUnityObject
	}

	internal static class TypeHandlersManager
	{
		private static readonly TypeHandler[]	typeHandlers;
		private static readonly ClassHandler	refTypeHandlers;
		private static readonly StructHandler	structTypeHandlers;
		private static readonly ArrayHandler	arrayRefTypeHandlers;

		static	TypeHandlersManager()
		{
			var		handlers = new List<TypeHandler>();

			foreach (Type t in Utility.EachSubClassesOf(typeof(TypeHandler)))
			{
				if (t == typeof(ClassHandler))
				{
					TypeHandlersManager.refTypeHandlers = new ClassHandler();
					handlers.Add(TypeHandlersManager.refTypeHandlers);
				}
				else if (t == typeof(StructHandler))
				{
					TypeHandlersManager.structTypeHandlers = new StructHandler();
					handlers.Add(TypeHandlersManager.structTypeHandlers);
				}
				else if (t == typeof(ArrayHandler))
				{
					TypeHandlersManager.arrayRefTypeHandlers = new ArrayHandler();
					handlers.Add(TypeHandlersManager.arrayRefTypeHandlers);
				}
				else
					handlers.Add((TypeHandler)Activator.CreateInstance(t));
			}

			handlers.Sort((a, b) => (a.GetType().GetCustomAttributes(typeof(PriorityAttribute), false) as PriorityAttribute[])[0].priority -
									(b.GetType().GetCustomAttributes(typeof(PriorityAttribute), false) as PriorityAttribute[])[0].priority);

			TypeHandlersManager.typeHandlers = handlers.ToArray();

			InternalNGDebug.Assert(TypeHandlersManager.refTypeHandlers != null, "Ref-type handler does not exist.");
			InternalNGDebug.Assert(TypeHandlersManager.structTypeHandlers != null, "Struct-type handler does not exist.");
			InternalNGDebug.Assert(TypeHandlersManager.arrayRefTypeHandlers != null, "Array-type handler does not exist.");
		}

		public static ClassHandler	GetRefTypeHandler()
		{
			return TypeHandlersManager.refTypeHandlers;
		}

		public static StructHandler	GetStructTypeHandler()
		{
			return TypeHandlersManager.structTypeHandlers;
		}

		public static ArrayHandler	GetArrayRefTypeHandler()
		{
			return TypeHandlersManager.arrayRefTypeHandlers;
		}

		public static TypeHandler	GetTypeHandler<T>()
		{
			for (int i = 0; i < TypeHandlersManager.typeHandlers.Length; i++)
			{
				if (TypeHandlersManager.typeHandlers[i].CanHandle(typeof(T)))
					return TypeHandlersManager.typeHandlers[i];
			}

			throw new MissingTypeHandlerException(typeof(T));
		}

		public static TypeHandler	GetTypeHandler(Type targetType)
		{
			if (targetType == null)
				return null;

			for (int i = 0; i < TypeHandlersManager.typeHandlers.Length; i++)
			{
				if (TypeHandlersManager.typeHandlers[i].CanHandle(targetType))
					return TypeHandlersManager.typeHandlers[i];
			}

			return null;
		}

		public static FieldInnerValueType	GetFieldInnerValueType(Type type)
		{
			if (type == null)
				return FieldInnerValueType.Null;

			if (typeof(Object).IsAssignableFrom(type) == true)
				return FieldInnerValueType.UnityObject;

			if (type.IsArray == true)
			{
				Type	subType = type.GetElementType();

				if (typeof(Object).IsAssignableFrom(subType) == true)
					return FieldInnerValueType.ArrayUnityObject;
				if (subType.IsValueType == true && subType.IsPrimitive == true)
					return FieldInnerValueType.ArrayValueType;
				return FieldInnerValueType.ArrayRefType;
			}

			if (type.GetInterface(typeof(IList<>).Name) != null) // IList<> with Serializable elements.
			{
				Type	subType = type.GetInterface(typeof(IList<>).Name).GetGenericArguments()[0];

				if (typeof(Object).IsAssignableFrom(subType) == true)
					return FieldInnerValueType.ArrayUnityObject;
				if (subType.IsValueType == true && subType.IsPrimitive == true) // Only primives, excluding Decimal (Nobody cares of him) and struct.
					return FieldInnerValueType.ArrayValueType;
				return FieldInnerValueType.ArrayRefType;
			}

			if (type.IsStruct() == true)
				return FieldInnerValueType.StructType;

			if (type.IsValueType == true)
				return FieldInnerValueType.ValueType;

			return FieldInnerValueType.RefType;
		}

		public static Type	GetNetworkType(Type type)
		{
			if (type == typeof(string))
				return typeof(string);

			FieldInnerValueType	innerValueType = TypeHandlersManager.GetFieldInnerValueType(type);
			
			if (innerValueType == FieldInnerValueType.UnityObject)
				return typeof(UnityObject);
			if (innerValueType == FieldInnerValueType.ArrayUnityObject)
				return typeof(UnityObject[]);

			if (innerValueType == FieldInnerValueType.RefType ||
				innerValueType == FieldInnerValueType.StructType)
			{
				return typeof(GenericClass);
			}

			if (innerValueType == FieldInnerValueType.ArrayRefType)
				return typeof(GenericClass[]);

			return type;
		}
	}
}