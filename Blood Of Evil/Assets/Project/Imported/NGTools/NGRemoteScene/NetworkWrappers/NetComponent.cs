using System;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace NGTools
{
	public sealed class NetComponent
	{
		public readonly int			instanceID;
		public readonly bool		togglable;
		public readonly bool		deletable;
		/// <summary>Only used to detect Renderers and display materials.</summary>
		public readonly Type		type;
		public readonly string		name;
		public readonly NetField[]	fields;
		public readonly NetMethod[]	methods;

		public static void	Serialize(ServerComponent component, ByteBuffer buffer)
		{
			Type	componentType = component.component.GetType();

			buffer.Append(component.instanceID);

			if (component.component is Behaviour)
			{
				if (componentType.GetMethod("Start", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("Awake", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("Update", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("FixedUpdate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null ||
					componentType.GetMethod("OnGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null)
				{
					buffer.Append(true); // Togglable
				}
				else
					buffer.Append(false); // Togglable
			}
			else
			{
				if (componentType.GetProperty("enabled", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) != null)
					buffer.Append(true); // Togglable
				else
					buffer.Append(false); // Togglable
			}

			if ((component.component is Transform) == true)
				buffer.Append(false); // Deletable
			else
				buffer.Append(true); // Deletable

			buffer.AppendUnicodeString(componentType.GetShortAssemblyType());
			buffer.AppendUnicodeString(componentType.Name);
			buffer.Append(component.fields.Length);

			for (int i = 0; i < component.fields.Length; i++)
				NetField.Serialize(component.component, component.fields[i], buffer);

			buffer.Append(component.methods.Length);

			for (int i = 0; i < component.methods.Length; i++)
				NetMethod.Serialize(component.methods[i], buffer);
		}

		public static NetComponent	Deserialize(ByteBuffer buffer)
		{
			return new NetComponent(buffer);
		}

		private	NetComponent(ByteBuffer buffer)
		{
			this.instanceID = buffer.ReadInt32();
			this.togglable = buffer.ReadBoolean();
			this.deletable = buffer.ReadBoolean();
			this.type = Type.GetType(buffer.ReadUnicodeString());
			this.name = buffer.ReadUnicodeString();

			int	length = buffer.ReadInt32();

			this.fields = new NetField[length];

			for (int i = 0; i < length; i++)
				this.fields[i] = NetField.Deserialize(buffer);

			length = buffer.ReadInt32();

			this.methods = new NetMethod[length];

			for (int i = 0; i < length; i++)
				this.methods[i] = NetMethod.Deserialize(buffer);
		}
	}
}