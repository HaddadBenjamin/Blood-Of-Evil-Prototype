using NGTools.Network;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorProperty : MonitorData
	{
		private PropertyInfo	propertyInfo;
		private TypeHandler		typeHandler;
		private bool			isStruct;

		public	MonitorProperty(string path, Func<object> getInstance, PropertyInfo propertyInfo) : base(path, getInstance)
		{
			this.propertyInfo = propertyInfo;
			this.typeHandler = TypeHandlersManager.GetTypeHandler(this.propertyInfo.PropertyType);
			this.isStruct = this.propertyInfo.PropertyType.IsStruct();

			this.value = this.propertyInfo.GetValue(this.getInstance(), null);

			this.MonitorSubData(this.propertyInfo.PropertyType, () => this.propertyInfo.GetValue(this.getInstance(), null));
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			//Debug.Log("P"+this.propertyInfo.Name + "=" + this.value + " <> "+ this.propertyInfo.GetValue(this.instance, null));
			if (this.isStruct == false) // Dont check struct.
			{
				if (object.Equals(this.value, this.propertyInfo.GetValue(this.getInstance(), null)) == false)
					updates.Add(this);
			}

			if (this.children != null)
			{
				for (int i = 0; i < this.children.Count; i++)
					this.children[i].CollectUpdates(updates);
			}
		}

		public override void	Update()
		{
			this.value = this.propertyInfo.GetValue(this.getInstance(), null);

			if (this.value == null &&
				this.children != null)
			{
				this.children.Clear();
			}
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerUpdateFieldValuePacket(this.path, this.typeHandler.Serialize(this.propertyInfo.PropertyType, this.value)) };
		}
	}
}