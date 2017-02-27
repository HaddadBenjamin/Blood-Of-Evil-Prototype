using NGTools.Network;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorField : MonitorData
	{
		private FieldInfo		fieldInfo;
		private TypeHandler		typeHandler;
		private bool			isStruct;

		public	MonitorField(string path, Func<object> getInstance, FieldInfo fieldInfo) : base(path, getInstance)
		{
			this.fieldInfo = fieldInfo;
			this.typeHandler = TypeHandlersManager.GetTypeHandler(this.fieldInfo.FieldType);
			this.isStruct = this.fieldInfo.FieldType.IsStruct();

			this.value = this.fieldInfo.GetValue(this.getInstance());

			this.MonitorSubData(this.fieldInfo.FieldType, () => this.fieldInfo.GetValue(this.getInstance()));
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			//Debug.Log("F"+this.fieldInfo.Name + "=" + this.value + " <> "+ this.fieldInfo.GetValue(this.instance));
			if (this.isStruct == false) // Dont check struct.
			{
				if (object.Equals(this.value, this.fieldInfo.GetValue(this.getInstance())) == false)
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
			this.value = this.fieldInfo.GetValue(this.getInstance());

			if (this.value == null &&
				this.children != null)
			{
				this.children.Clear();
			}
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerUpdateFieldValuePacket(this.path, this.typeHandler.Serialize(this.fieldInfo.FieldType, this.value)) };
		}
	}
}