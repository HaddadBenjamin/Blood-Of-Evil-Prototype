using NGTools.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NGTools.NGRemoteScene
{
	internal sealed class MonitorArray : MonitorData
	{
		/// <summary>
		/// Use lastSize instead of value to avoid boxing.
		/// </summary>
		private int				lastSize;
		private TypeHandler		typeHandler;
		private IFieldModifier	fieldInfo;

		public	MonitorArray(string path, IFieldModifier fieldInfo, Func<object> getInstance) : base(path, getInstance)
		{
			this.typeHandler = TypeHandlersManager.GetTypeHandler(typeof(int));
			this.fieldInfo = fieldInfo;
			this.lastSize = this.GetSize();

			this.MonitorSubData(this.getInstance.GetType(), () => this.getInstance());
		}

		private int	GetSize()
		{
			object	arrayInstance = this.fieldInfo.GetValue(this.getInstance());
			Array	array = arrayInstance as Array;

			if (array != null)
				return array.Length;
			else
			{
				IList	list = arrayInstance as IList;

				if (list != null)
					return list.Count;
				else
					return 0;
			}
		}

		public override void	CollectUpdates(List<MonitorData> updates)
		{
			int	currentSize = this.GetSize();

			//Debug.Log("CollectArray " + this.lastSize + " != " + currentSize);
			if (this.lastSize != currentSize)
			{
				object	arrayInstance = this.fieldInfo.GetValue(this.getInstance());
				updates.Add(this);
				this.MonitorSubData(arrayInstance.GetType(), () => this.fieldInfo.GetValue(this.getInstance()));
			}
			else
			{
				if (this.children != null)
				{
					//Debug.Log("ArrayCollectArray " + this.children.Count);
					for (int i = 0; i < this.children.Count; i++)
						this.children[i].CollectUpdates(updates);
				}
			}
		}

		public override void	Update()
		{
			this.lastSize = this.GetSize();
		}

		public override Packet[]	CreateUpdatePackets()
		{
			Packet[]	packets = new Packet[this.lastSize + 1];

			packets[0] = new ServerUpdateFieldValuePacket(this.path, this.typeHandler.Serialize(this.lastSize));

			Type		subType = Utility.GetArraySubType(this.fieldInfo.Type);
			TypeHandler	subTypeHandler = TypeHandlersManager.GetTypeHandler(subType);
			IEnumerable	array = this.fieldInfo.GetValue(this.getInstance()) as IEnumerable;
			int			i = 0;

			foreach (var item in array)
			{
				packets[i + 1] = new ServerUpdateFieldValuePacket(this.path + "." + i, subTypeHandler.Serialize(subType, item));
				++i;
			}

			return packets;
		}
	}
}