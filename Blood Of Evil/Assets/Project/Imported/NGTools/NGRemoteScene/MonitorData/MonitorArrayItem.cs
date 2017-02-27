using NGTools.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorArrayItem : MonitorData
	{
		private int			index;
		private TypeHandler	typeHandler;
		private Type		subType;
		private bool		isStruct;

		public	MonitorArrayItem(string path, Func<object> getArray, int index) : base(path, getArray)
		{
			this.index = index;
			this.subType = Utility.GetArraySubType(this.getInstance().GetType());
			this.typeHandler = TypeHandlersManager.GetTypeHandler(this.subType);
			this.isStruct = this.subType.IsStruct();

			this.value = this.GetValue();

			this.MonitorSubData(this.subType, this.GetValue);
		}

		private object	GetValue()
		{
			Array	array = this.getInstance() as Array;

			if (array != null)
				return array.GetValue(this.index);
			else
			{
				IList	list = this.getInstance() as IList;

				if (list != null)
					return list[this.index];
				else
					throw new NotImplementedException("MonitorArrayItem does not support type \"" + this.getInstance().GetType() + "\".");
			}
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			if (this.isStruct == false) // Dont check struct.
			{
				object	currentValue = this.GetValue();
				if (object.Equals(this.value, currentValue) == false)
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
			this.value = this.GetValue();

			if (this.value == null &&
				this.children != null)
			{
				this.children.Clear();
			}
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerUpdateFieldValuePacket(this.path, this.typeHandler.Serialize(this.subType, this.value)) };
		}
	}
}