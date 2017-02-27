using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NGToolsEditor
{
	/// <summary>
	/// <para>Stores elements from an Array or List in a List of byte[] instead of a unique byte[].</para>
	/// <para>Reduce single point of failure bottleneck in case of one element crashes during the deserialization.</para>
	/// </summary>
	[Serializable]
	public sealed class MultiDataStorage
	{
		[Serializable]
		private sealed class Data
		{
			public byte[]	data;
		}

		[SerializeField]
		private List<Data>	dataContainer;

		public int	Count { get { return this.dataContainer.Count; } }

		public	MultiDataStorage()
		{
			this.dataContainer = new List<Data>();
		}

		public	MultiDataStorage(int capacity)
		{
			this.dataContainer = new List<Data>(capacity);
		}

		/// <summary>
		/// Adds an element to the list. Will catch any exception in case of failure during the serialization.
		/// </summary>
		/// <param name="value"></param>
		public void	Add(object value)
		{
			try
			{
				byte[]	bytes = Utility.SerializeField(value);
				this.dataContainer.Add(new Data() { data = bytes });
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogFileException(ex);
			}
		}

		public object	Get(int i)
		{
			try
			{
				return Utility.DeserializeField(this.dataContainer[i].data);
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogFileException(ex);
			}

			return null;
		}

		/// <summary>
		/// Serializes data from the given <paramref name="list"/> discarding failed serialization.
		/// </summary>
		/// <param name="list"></param>
		public void	Serialize(IList list)
		{
			this.dataContainer.Clear();

			for (int i = 0; i < list.Count; i++)
				this.Add(list[i]);
		}

		/// <summary>
		/// Deserializes data into the given <paramref name="list"/> discarding failed deserialization.
		/// </summary>
		/// <param name="list"></param>
		public void	Deserialize(IList list)
		{
			list.Clear();

			for (int i = 0; i < this.dataContainer.Count; i++)
			{
				object	element = this.Get(i);
				if (element != null)
					list.Add(element);
			}
		}

		/// <summary>
		/// Deserializes data into the given <paramref name="list"/> discarding failed deserialization.
		/// </summary>
		/// <param name="list"></param>
		public T[]	Deserialize<T>()
		{
			List<T>	list = new List<T>();

			for (int i = 0; i < this.dataContainer.Count; i++)
			{
				T	element = (T)this.Get(i);

				if (element != null)
					list.Add(element);
			}

			return list.ToArray();
		}

		public void Clear()
		{
			this.dataContainer.Clear();
		}
	}
}