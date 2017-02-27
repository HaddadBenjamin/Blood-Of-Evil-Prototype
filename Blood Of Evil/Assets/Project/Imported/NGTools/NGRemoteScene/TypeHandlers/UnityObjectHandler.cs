using System;

namespace NGTools.NGRemoteScene
{
	using UnityEngine;

	public class UnityObject
	{
		public Type		type { get; private set; }
		public int		gameObjectInstanceID { get; private set; }
		public int		instanceID { get; private set; }
		public string	name { get; private set; }

		public	UnityObject()
		{
			// Default type if unknown. Should be temporary.
			this.type = typeof(Object);
			this.name = string.Empty;
		}

		public	UnityObject(ByteBuffer buffer)
		{
			this.type = Type.GetType(buffer.ReadUnicodeString());

			// In case of corrupted data.
			if (this.type == null)
				this.type = typeof(Object);

			this.gameObjectInstanceID = buffer.ReadInt32();

			if (this.gameObjectInstanceID != -1)
			{
				this.instanceID = buffer.ReadInt32();
				if (this.instanceID != 0)
					this.name = buffer.ReadUnicodeString();
			}
		}

		public	UnityObject(Type type, int instanceID)
		{
			this.type = type;
			this.gameObjectInstanceID = instanceID;
			this.instanceID = instanceID;
			this.name = string.Empty;
		}

		public void	Assign(Type type, int gameObjectInstanceID, int instanceID, string name)
		{
			this.type = type;
			this.gameObjectInstanceID = gameObjectInstanceID;
			this.instanceID = instanceID;
			this.name = name;
		}
	}

	[Priority(10)]
	internal sealed class UnityObjectHandler : TypeHandler
	{
		public override bool	CanHandle(Type type)
		{
			return type == typeof(UnityObject) || typeof(Object).IsAssignableFrom(type) == true;
		}

		public override void	Serialize(ByteBuffer buffer, Type fieldType, object instance)
		{
			buffer.AppendUnicodeString(fieldType.GetShortAssemblyType());

			UnityObject	unityObject = instance as UnityObject;

			if (unityObject != null)
			{
				buffer.Append(0);
				buffer.Append(unityObject.instanceID);
				buffer.Append(0);
				return;
			}

			Object	refUnityObject = instance as Object;

			if (refUnityObject != null)
			{
				GameObject	gameObject = refUnityObject as GameObject;

				if (gameObject != null)
					buffer.Append(gameObject.GetInstanceID());
				else
				{
					Component	component = refUnityObject as Component;

					if (component != null)
						buffer.Append(component.gameObject.GetInstanceID());
					else
						buffer.Append(0);
				}

				buffer.Append(refUnityObject.GetInstanceID());
				buffer.AppendUnicodeString(refUnityObject.name);
			}
			else
				buffer.Append(-1);
		}

		public override object		Deserialize(ByteBuffer buffer, Type fieldType)
		{
			return new UnityObject(buffer);
		}

		public override object	DeserializeRealValue(NGServerScene manager, ByteBuffer buffer, Type fieldType)
		{
			UnityObject	unityObject = this.Deserialize(buffer, fieldType) as UnityObject;

			if (unityObject.instanceID == 0)
				return null;
			return manager.GetResource(unityObject.type, unityObject.instanceID);
		}
	}
}