using NGTools;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace NGToolsEditor.NGRemoteScene
{
	[InitializeOnLoad]
	internal static class TypeHandlerDrawersManager
	{
		private static readonly Dictionary<Type, Type>	typeHandlerDrawers;
		private static readonly Dictionary<Type, Type>	typeDrawers;
		private static readonly TypeHandlerDrawer		defaultDrawer;

		static	TypeHandlerDrawersManager()
		{
			TypeHandlerDrawersManager.typeHandlerDrawers = new Dictionary<Type, Type>();
			TypeHandlerDrawersManager.typeDrawers = new Dictionary<Type, Type>();

			foreach (Type t in Utility.EachSubClassesOf(typeof(TypeHandlerDrawer)))
			{
				TypeHandlerDrawerForAttribute[]	typeHandlerAttributes = t.GetCustomAttributes(typeof(TypeHandlerDrawerForAttribute), false) as TypeHandlerDrawerForAttribute[];

				if (typeHandlerAttributes.Length >= 1)
					TypeHandlerDrawersManager.typeHandlerDrawers.Add(typeHandlerAttributes[0].type, t);
				else
				{
					TypeDrawerForAttribute[]	typeAttributes = t.GetCustomAttributes(typeof(TypeDrawerForAttribute), false) as TypeDrawerForAttribute[];

					if (typeAttributes.Length >= 1)
						TypeHandlerDrawersManager.typeDrawers.Add(typeAttributes[0].type, t);
				}
			}

			TypeHandlerDrawersManager.defaultDrawer = Activator.CreateInstance<UnsupportedTypeDrawer>();
		}

		/// <summary>
		/// <para>Create a drawer from the given <paramref name="targetType"/>.</para>
		/// <para>Returns the default drawer if <paramref name="targetType"/> is null.</para>
		/// <para>Returns the array drawer if <paramref name="targetType"/> is an array.</para>
		/// <para>Returns the class drawer if <paramref name="targetType"/> is a class not inheriting from <see cref="UnityEngine.Object"/>.</para>
		/// </summary>
		/// <param name="targetType"></param>
		/// <param name="type">Only use for ArrayDrawer purpose.</param>
		/// <param name="value">Only use for ClassDrawer purpose.</param>
		/// <returns>Always return an instance of TypeHandlerDrawer.</returns>
		public static TypeHandlerDrawer	CreateTypeHandlerDrawer(TypeHandler targetType, Type type, object value)
		{
			Type	typeHandlerType;

			if (targetType != null)
			{
				FieldInnerValueType	fieldInnerValueType = TypeHandlersManager.GetFieldInnerValueType(type);

				try
				{
					if (TypeHandlerDrawersManager.typeHandlerDrawers.TryGetValue(targetType.GetType(), out typeHandlerType) == true)
						return Activator.CreateInstance(typeHandlerType, targetType) as TypeHandlerDrawer;

					if (TypeHandlerDrawersManager.typeDrawers.TryGetValue(type, out typeHandlerType) == true)
						return Activator.CreateInstance(typeHandlerType, targetType) as TypeHandlerDrawer;

					if (fieldInnerValueType == FieldInnerValueType.UnityObject)
						return Activator.CreateInstance(typeof(UnityObjectDrawer), targetType) as TypeHandlerDrawer;

					if (fieldInnerValueType == FieldInnerValueType.ArrayValueType ||
						fieldInnerValueType == FieldInnerValueType.ArrayRefType ||
						fieldInnerValueType == FieldInnerValueType.ArrayUnityObject)
					{
						return Activator.CreateInstance(typeof(ArrayDrawer), targetType, type, value) as TypeHandlerDrawer;
					}

					if (fieldInnerValueType == FieldInnerValueType.StructType ||
						fieldInnerValueType == FieldInnerValueType.RefType)
					{
						return Activator.CreateInstance(typeof(ClassDrawer), targetType, value) as TypeHandlerDrawer;
					}
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException("TargetType=" + targetType + Environment.NewLine + "Type=" + type + Environment.NewLine + "TypeHandler=" + typeHandlerDrawers + Environment.NewLine + "Value=" + value, ex);
				}
			}

			return TypeHandlerDrawersManager.defaultDrawer;
		}
	}
}