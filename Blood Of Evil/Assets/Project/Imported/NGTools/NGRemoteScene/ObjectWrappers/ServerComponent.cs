using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NGTools
{
	public sealed class ServerComponent
	{
		/// <summary>
		/// Hidden properties from MonoBehaviour, Behaviour, Component and Object.
		/// </summary>
		public readonly static string[]	ExcludedProperties = { "name", "tag", "useGUILayout", "hideFlags" };

		private readonly static List<IFieldModifier>	tempFields = new List<IFieldModifier>(16);

		public readonly Component			component;
		public readonly int					instanceID;
		public readonly IFieldModifier[]	fields;
		public readonly ServerMethodInfo[]	methods;

		public	ServerComponent(Component component)
		{
			this.component = component;
			this.instanceID = this.component.GetInstanceID();

			Type				type = this.component.GetType();
			ComponentExposer[]	exposers = ComponentExposersManager.GetComponentExposers(type);
			PropertyInfo[]		pis = Utility.GetExposedProperties(type, exposers);
			FieldInfo[]			fis = Utility.GetExposedFields(type, exposers);
			int					i = 0;

			ServerComponent.tempFields.Clear();

			for (; i < pis.Length; ++i)
			{
				if (ServerComponent.ExcludedProperties.Contains(pis[i].Name) == false)
					ServerComponent.tempFields.Add(new PropertyModifier(pis[i]));
			}

			for (int j = 0; j < fis.Length; ++i, ++j)
				ServerComponent.tempFields.Add(new FieldModifier(fis[j]));

			this.fields = ServerComponent.tempFields.ToArray();

			MethodInfo[]	methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);

			this.methods = new ServerMethodInfo[methods.Length];

			for (i = 0; i < methods.Length; i++)
				this.methods[i] = new ServerMethodInfo(methods[i]);
		}

		public IFieldModifier	GetField(string name)
		{
			for (int i = 0; i < this.fields.Length; i++)
			{
				if (this.fields[i].Name == name)
					return this.fields[i];
			}

			return null;
		}

		public ServerMethodInfo	GetMethod(string name)
		{
			for (int i = 0; i < this.methods.Length; i++)
			{
				if (this.methods[i].methodInfo.Name == name)
					return this.methods[i];
			}

			return null;
		}
	}
}