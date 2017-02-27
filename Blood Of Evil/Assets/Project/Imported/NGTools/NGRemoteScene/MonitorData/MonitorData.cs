using NGTools.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NGTools.NGRemoteScene
{
	using UnityEngine;

	internal abstract class MonitorData
	{
		protected static ByteBuffer	buffer = new ByteBuffer(64);

		public bool		ToDelete { get; protected set; }
		public object	Instance { get { return this.getInstance(); } }

		protected readonly string	path;
		protected object			value;
		protected Func<object>		getInstance;
		protected List<MonitorData>	children;

		public	MonitorData(string path, Func<object> getInstance)
		{
			//Debug.Log("Watch " + path + "	# " + this.GetType());
			this.path = path;
			this.getInstance = getInstance;
		}

		/// <summary>Checks if the current value is different from the reference. Adds the monitor to <paramref name="updates"/> when different.</summary>
		/// <param name="updates"></param>
		public abstract void	CollectUpdates(List<MonitorData> updates);

		/// <summary>Updates the current value with the new value. Called after being collected.</summary>
		public abstract void	Update();

		/// <summary>Creates a Packet to send over the network to update value on clients. Called after being updated.</summary>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException">This method must be override.</exception>
		public virtual Packet[]	CreateUpdatePackets()
		{
			throw new NotImplementedException();
		}

		protected void	MonitorSubData(Type type, Func<object> getInstance, bool firstLevelRefObject = false)
		{
			object	instance = getInstance();
			if (instance == null)
				return;

			if (typeof(Object).IsAssignableFrom(type) == true)
			{
				if (firstLevelRefObject == true)
				{
					ComponentExposer[]	exposers = ComponentExposersManager.GetComponentExposers(type);
					PropertyInfo[]		properties = Utility.GetExposedProperties(type, exposers);
					FieldInfo[]			fields = Utility.GetExposedFields(type, exposers);

					this.children = new List<MonitorData>(properties.Length + fields.Length);

					for (int i = 0; i < properties.Length; ++i)
					{
						// Exclude properties only if they come from MonoBehaviour, Behaviour, Component, Object, to handle hidden properties ("new" or "override" keywords in sub classes).
						if (ServerComponent.ExcludedProperties.Contains(properties[i].Name) == false ||
							(properties[i].DeclaringType != typeof(MonoBehaviour) &&
							 properties[i].DeclaringType != typeof(Behaviour) &&
							 properties[i].DeclaringType != typeof(Component) &&
							 properties[i].DeclaringType != typeof(Object)))
						{
							CustomMonitorData	customMonitor = MonitorDataManager.CreateMonitorData(properties[i].PropertyType, this.path + "." + properties[i].Name, getInstance, new PropertyModifier(properties[i]));

							if (customMonitor != null)
								this.children.Add(customMonitor);
							else
							{
								if (properties[i].PropertyType.IsUnityArray() == true)
									this.children.Add(new MonitorArray(this.path + "." + properties[i].Name, new PropertyModifier(properties[i]), getInstance));
								else
									this.children.Add(new MonitorProperty(this.path + "." + properties[i].Name, getInstance, properties[i]));
							}
						}
					}

					for (int i = 0; i < fields.Length; ++i)
					{
						CustomMonitorData	customMonitor = MonitorDataManager.CreateMonitorData(fields[i].FieldType, this.path + "." + fields[i].Name, getInstance, new FieldModifier(fields[i]));

						if (customMonitor != null)
							this.children.Add(customMonitor);
						else
						{
							if (fields[i].FieldType.IsUnityArray() == true)
								this.children.Add(new MonitorArray(this.path + "." + fields[i].Name, new FieldModifier(fields[i]), getInstance));
							else
								this.children.Add(new MonitorField(this.path + "." + fields[i].Name, getInstance, fields[i]));
						}
					}
				}
			}
			else if (type.IsUnityArray() == true)
			{
				this.children = new List<MonitorData>();

				IEnumerable	array = instance as IEnumerable;
				IEnumerator	it = array.GetEnumerator();
				int			i = 0;

				while (it.MoveNext())
				{
					this.children.Add(new MonitorArrayItem(this.path + "." + i.ToString(), getInstance, i));
					++i;
				}
			}
			else if (type.IsClass == true ||
					 type.IsStruct() == true)
			{
				ComponentExposer[]	exposers = ComponentExposersManager.GetComponentExposers(type);
				PropertyInfo[]		properties = Utility.GetExposedProperties(type, exposers);
				FieldInfo[]			fields = Utility.GetExposedFields(type, exposers);

				this.children = new List<MonitorData>(properties.Length + fields.Length);

				for (int i = 0; i < properties.Length; ++i)
				{
					CustomMonitorData	customMonitor = MonitorDataManager.CreateMonitorData(properties[i].PropertyType, this.path + "." + properties[i].Name, getInstance, new PropertyModifier(properties[i]));

					if (customMonitor != null)
						this.children.Add(customMonitor);
					else
					{
						if (properties[i].PropertyType.IsUnityArray() == true)
							this.children.Add(new MonitorArray(this.path + "." + properties[i].Name, new PropertyModifier(properties[i]), getInstance));
						else
							this.children.Add(new MonitorProperty(this.path + "." + properties[i].Name, getInstance, properties[i]));
					}
				}

				for (int i = 0; i < fields.Length; ++i)
				{
					CustomMonitorData	customMonitor = MonitorDataManager.CreateMonitorData(fields[i].FieldType, this.path + "." + fields[i].Name, getInstance, new FieldModifier(fields[i]));

					if (customMonitor != null)
						this.children.Add(customMonitor);
					else
					{
						if (fields[i].FieldType.IsUnityArray() == true)
							this.children.Add(new MonitorArray(this.path + "." + fields[i].Name, new FieldModifier(fields[i]), getInstance));
						else
							this.children.Add(new MonitorField(this.path + "." + fields[i].Name, getInstance, fields[i]));
					}
				}
			}
		}
	}
}