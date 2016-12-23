﻿using Microsoft.Win32;
using System.Collections.Generic;

namespace NGToolsEditor
{
	public abstract class PrefsManager
	{
		public List<string>	registryNames = new List<string>();
		public List<string>	keys = new List<string>();
		public List<object>	values = new List<object>();

		public virtual void		DeleteKey(int i)
		{
			this.DeleteKey(this.keys[i]);

			this.registryNames.RemoveAt(i);
			this.keys.RemoveAt(i);
			this.values.RemoveAt(i);
		}

		public abstract void	DeleteKey(string key);

		public abstract void	DeleteAll();

		public abstract bool	HasKey(string key);

		public abstract float	GetFloat(string key, float defaultValue = 0F);
		public abstract int		GetInt(string key, int defaultValue = 0);
		public abstract string	GetString(string key, string defaultValue = null);

		public abstract void	SetFloat(string key, float value);
		public abstract void	SetInt(string key, int value);
		public abstract void	SetString(string key, string value);

		public abstract void	LoadPreferences();

		private int	InsertSort(string n, int start, int length)
		{
			int	half = length >> 1;
			if (half <= 0)
			{
				if (this.keys.Count == 0 || this.keys[start].CompareTo(n) >= 0)
					return start;
				return start + 1;
			}

			int	p = this.keys[start + half].CompareTo(n);

			if (p == 0)
				return start + half;
			if (p > 0)
				return this.InsertSort(n, start, half);
			return this.InsertSort(n, start + half, length - half);
		}

		private string		registryKey;
		private RegistryKey	key;

		public void	BeginLoadFromRegistrar()
		{
			this.key = Registry.CurrentUser.OpenSubKey(registryKey);
		}

		public void	EndLoadFromRegistrar()
		{
			this.key.Close();
		}

		public void	LoadValueFromRegistrar(int i)
		{
			if (this.key.GetValue(this.registryNames[i]).GetType() == typeof(byte[]))
				this.values[i] = this.GetString(this.keys[i]);
			else
				this.values[i] = this.Convert(this.keys[i]);
		}

		protected void	LoadFromRegistrar(string registryKey)
		{
			this.registryNames.Clear();
			this.keys.Clear();
			this.values.Clear();

#if UNITY_EDITOR_WIN
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryKey))
			{
				if (key == null)
					return;

				this.registryKey = registryKey;

				string[]	names = key.GetValueNames();

				this.registryNames.Capacity = names.Length;
				this.keys.Capacity = names.Length;
				this.values.Capacity = names.Length;

				foreach (string v in names)
				{
					string	name = v.Substring(0, v.LastIndexOf('_', v.Length - 1));

					int	i = this.InsertSort(name, 0, this.keys.Count);

					this.registryNames.Insert(i, v);
					this.keys.Insert(i, name);
					this.values.Add(null);
				}
			}
#elif UNITY_EDITOR_OSX
			Dictionary<string, object>	dict = (Dictionary<string, object>)Plist.readPlist(registryKey);

			this.keys.Capacity = dict.Count;
			this.values.Capacity = dict.Count;

			foreach (var pair in dict)
			{
				string	name = pair.Key;

				int	i = this.InsertSort(name, 0, this.keys.Count);

				this.keys.Insert(i, name);

				if (pair.Value is double)
					this.values.Insert(i, this.GetFloat(name));
				else
					this.values.Insert(i, pair.Value);
			}
#endif
		}

		private object	Convert(string prefKey)
		{
			if (this.GetInt(prefKey, int.MinValue) != int.MinValue)
			{
				return this.GetInt(prefKey);
			}
			if (this.GetFloat(prefKey, float.MinValue) != float.MinValue)
			{
				return this.GetFloat(prefKey);
			}
			return this.GetString(prefKey);
		}
	}
}