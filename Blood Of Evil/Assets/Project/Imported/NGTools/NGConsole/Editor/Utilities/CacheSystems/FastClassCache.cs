﻿using NGTools;
using System;

namespace NGToolsEditor
{
	[Serializable]
	public class FastClassCache
	{
		[Serializable]
		public class HashClass
		{
			public readonly int		hash;
			public Type				type;
			public FastMethodCache	methods;

			internal	HashClass(int hash)
			{
				this.hash = hash;
			}
		}

		private HashClass[]		classes;
		private int				indexLeft;

		public	FastClassCache()
		{
			this.classes = new HashClass[128];
			this.indexLeft = 0;
		}

		/// <summary>
		/// Finds the cached version or will try to retrieve the type from assemblies.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public HashClass	GetType(string className)
		{
			int	hash = className.GetHashCode();

			// Check if cached.
			for (int i = 0; i < this.indexLeft; i++)
			{
				if (this.classes[i].hash == hash)
					return this.classes[i];
			}

			try
			{
				// Populate new file.
				HashClass	hashClass = new HashClass(hash);

				hashClass.type = Utility.GetType(className);
				if (hashClass.type != null)
					hashClass.methods = new FastMethodCache(hashClass.type);

				// Check array overflow.
				if (this.indexLeft == this.classes.Length)
					Array.Resize(ref this.classes, this.classes.Length << 1);

				this.classes[this.indexLeft] = hashClass;
				++this.indexLeft;

				//Debug.Log("Cached class:" + className);
				return hashClass;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}

			return null;
		}
	}
}