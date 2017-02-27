using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NGTools
{
	using UnityEngine;

	public static partial class Utility
	{
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
					if (fis[i].IsDefined(typeof(HideInInspector), false) == true ||
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
					if (pis[i].IsDefined(typeof(HideInInspector), false) == true ||
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
	}
}