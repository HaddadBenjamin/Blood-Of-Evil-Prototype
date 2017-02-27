using NGTools;
using NGTools.NGFav;
using NGTools.NGRemoteScene;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor.NGFav
{
	using UnityEngine;

	[Serializable]
	public sealed class SelectionItem
	{
		public const char	DataSeparator = ';';
		public const char	DataSeparatorCharPlaceholder = (char)5;

		private static Dictionary<string, MethodInfo>	cachedMethods = new Dictionary<string, MethodInfo>();

		public string		assetPath = string.Empty;
		public Object		@object;
		public List<string>	hierarchy = new List<string>();
		public string		resolverAssemblyQualifiedName;
		public int			dataResolver;

		public string	resolverFailedError;

		private int	lastHierarchyUpdate = 0;

		/// <summary></summary>
		/// <param name="object"></param>
		/// <exception cref="System.MissingMethodException">Thrown when the a resolver is returned and is not static.</exception>
		public	SelectionItem(Object @object)
		{
			this.@object = @object;

			if (this.@object is GameObject)
			{
				GameObject	gameObject = @object as GameObject;
				Transform	t = gameObject.transform;

				while (t != null)
				{
					this.hierarchy.Add(t.gameObject.name);

					// Look for the closest ICustomFavorite in hierarchy.
					if (t != null && string.IsNullOrEmpty(this.resolverAssemblyQualifiedName) == true)
					{
						PrefabType	type = PrefabUtility.GetPrefabType(@object);

						if (type == PrefabType.None || type == PrefabType.PrefabInstance)
						{
							ICustomFavorite	customFav = t.gameObject.GetIComponent<ICustomFavorite>();

							if (customFav != null)
							{
								Func<int, GameObject>	callbackResolver;

								customFav.GetFavorite(out this.dataResolver, out callbackResolver);

								if (callbackResolver.Target != null)
								{
									InternalNGDebug.Log(Errors.Fav_ResolverNotStatic, "The callback resolver you returned from GetFavorite must be static.");
									throw new MissingMethodException();
								}
								else
								{
									this.resolverAssemblyQualifiedName = callbackResolver.Method.DeclaringType.AssemblyQualifiedName + "." + callbackResolver.Method.Name;
								}

								break;
							}
						}
					}

					t = t.parent;
				}

				this.hierarchy.Reverse();
			}
		}

		public void	TryReconnect()
		{
			InternalNGDebug.AssertFile(this.@object == null, "SelectionItem is trying to reconnect while having an object already.");

			if (this.lastHierarchyUpdate == NGFavWindow.UpdateHierarchyCounter)
				return;

			this.lastHierarchyUpdate = NGFavWindow.UpdateHierarchyCounter;

			if (string.IsNullOrEmpty(this.resolverAssemblyQualifiedName) == false)
			{
				MethodInfo	method = null;

				if (SelectionItem.cachedMethods.TryGetValue(this.resolverAssemblyQualifiedName, out method) == false)
				{
					int	methodSeparator = this.resolverAssemblyQualifiedName.LastIndexOf('.', this.resolverAssemblyQualifiedName.Length - 1);

					// Make sure method is present and has at least one char length.
					if (methodSeparator != -1 && methodSeparator < this.resolverAssemblyQualifiedName.Length - 1)
					{
						string	className = this.resolverAssemblyQualifiedName.Substring(0, methodSeparator);
						Type	classType = Type.GetType(className);

						if (classType != null)
						{
							string	methodName = this.resolverAssemblyQualifiedName.Substring(methodSeparator + 1);

							try
							{
								method = classType.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
							}
							catch (AmbiguousMatchException)
							{
								InternalNGDebug.Log(Errors.Fav_ResolverIsAmbiguous, "The resolver has an ambiguous name \"" + methodName + "\" in type \"" + classType.Name + "\".");
							}
						}
					}

					SelectionItem.cachedMethods.Add(this.resolverAssemblyQualifiedName, method);
				}

				if (method != null)
				{
					try
					{
						this.@object = method.Invoke(null, new object[] { this.dataResolver }) as GameObject;

						// Look for nested favorite from the given object.
						if (this.@object != null && this.hierarchy.Count > 1)
						{
							string[]	array = new string[this.hierarchy.Count - 1];
							this.hierarchy.CopyTo(1, array, 0, array.Length);

							Transform	t = (this.@object as GameObject).transform.FindChild(string.Join("/", array));

							if (t != null)
								this.@object = t.gameObject;
						}

						this.resolverFailedError = string.Empty;
					}
					catch (Exception ex)
					{
						this.resolverFailedError = "The resolver \"" + method + "\" has thrown an exception." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
					}
					return;
				}
			}

			// Is GameObject (Asset or scene).
			if (this.@object == null && this.hierarchy.Count > 0)
			{
				// Can be found if GameObject is enabled.
				this.@object = GameObject.Find(string.Join("/", this.hierarchy.ToArray()));

				if (this.@object == null)
				{
					this.@object = GameObject.Find(this.hierarchy[this.hierarchy.Count - 1]);

					if (this.@object == null)
					{
						// Otherwise have to look up through all hierarchy.
						for (int i = 0; i < NGFavWindow.RootObjects.Count; i++)
						{
							Transform	t = NGFavWindow.RootObjects[i].transform;

							if (t.name.Equals(this.hierarchy[0]) == true)
							{
								int	j = 1;

								for (; j < this.hierarchy.Count; j++)
								{
									t = t.transform.FindChild(this.hierarchy[j]);

									if (t == null)
										break;
								}

								if (j >= this.hierarchy.Count)
								{
									this.@object = t.gameObject;
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}