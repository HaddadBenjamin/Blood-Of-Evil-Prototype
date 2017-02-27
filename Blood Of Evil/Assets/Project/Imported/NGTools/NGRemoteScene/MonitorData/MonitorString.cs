using NGTools.Network;
using System;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorString : CustomMonitorData
	{
		private IValueGetter	valueGetter;
		private TypeHandler		typeHandler;

		private static bool	CanMonitorType(Type type)
		{
			return type == typeof(String);
		}

		public	MonitorString(string path, Func<object> getInstance, IValueGetter valueGetter) : base(path, getInstance)
		{
			this.valueGetter = valueGetter;
			this.typeHandler = TypeHandlersManager.GetTypeHandler(valueGetter.Type);

			this.value = valueGetter.GetValue<string>(this.getInstance());
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			//Debug.Log("F"+this.fieldInfo.Name + "=" + this.value + " <> "+ this.fieldInfo.GetValue(this.instance));
			if (string.Compare((string)this.value, valueGetter.GetValue<string>(this.getInstance())) != 0)
				updates.Add(this);
		}

		public override void	Update()
		{
			this.value = valueGetter.GetValue<string>(this.getInstance());
		}

		public override Packet[]	CreateUpdatePackets()
		{
			return new Packet[] { new ServerUpdateFieldValuePacket(this.path, this.typeHandler.Serialize(this.value)) };
		}
	}
}