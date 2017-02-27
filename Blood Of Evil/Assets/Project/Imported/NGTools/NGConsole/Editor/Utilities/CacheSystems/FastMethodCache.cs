using NGTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	internal sealed class FastMethodCache
	{
		[Serializable]
		private sealed class HashMethod
		{
			public readonly int	hash;
			public MethodInfo	methodInfo;

			internal	HashMethod(int hash)
			{
				this.hash = hash;
			}
		}

		private Type			classType;
		private HashMethod[]	methods;
		private int				indexLeft;

		public	FastMethodCache(Type classType)
		{
			this.classType = classType;
			this.methods = new HashMethod[16];
			this.indexLeft = 0;
		}

		/// <summary>
		/// Finds the cached version or tries to fetch the method based on the name and its parameters.
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public MethodInfo	GetMethod(string methodName, string parameters)
		{
			// Ugly? Why not? @_@
			int	hash = methodName.GetHashCode() + parameters.GetHashCode();

			// Check if cached.
			for (int i = 0; i < this.indexLeft; i++)
			{
				if (this.methods[i].hash == hash)
					return this.methods[i].methodInfo;
			}

			try
			{
				// Populate new file.
				HashMethod	hashMethod = new HashMethod(hash);

				hashMethod.methodInfo = this.GetMethodInfo(methodName, parameters);

				// Check array overflow.
				if (this.indexLeft == this.methods.Length)
					Array.Resize(ref this.methods, this.methods.Length << 1);

				this.methods[this.indexLeft] = hashMethod;
				++this.indexLeft;

				//Debug.Log("Cached method:" + methodName + "(" + parameters + ")");
				return hashMethod.methodInfo;
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
			}

			return null;
		}

		private MethodInfo	GetMethodInfo(string methodName, string allParameters)
		{
			//Debug.Log("ClassType=" + classType);
			string[]	parameters = allParameters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			// Many parameters includes ambiguity.
			if (parameters.Length > 0)
				return this.GetMethodFromParameters(methodName, parameters);
			else
			{
				try
				{
					return this.classType.GetMethod(methodName, ~BindingFlags.Default, null, new Type[] {}, null);
				}
				catch (AmbiguousMatchException)
				{
					Debug.Log("Ambiguous in " + this.classType + " " + methodName);
					foreach (var mi in this.classType.GetMethods(~BindingFlags.Default))
					{
						if (mi.Name == methodName)
							Debug.Log(mi.Name);
					}
					return this.GetMethodFromParameters(methodName, parameters);
				}
			}
		}

		private MethodInfo	GetMethodFromParameters(string methodName, string[] parameters)
		{
			List<MethodInfo>	matchingParamMethods = new List<MethodInfo>();

			// Get mathods matching the exact number of parameters.
			foreach (var m in this.classType.GetMethods(~BindingFlags.Default))
			{
				//Debug.Log(m.Name +"=="+ methodName);
				if (m.Name == methodName)
				{
					if (m.GetParameters().Length == parameters.Length)
						matchingParamMethods.Add(m);
				}
			}

			if (matchingParamMethods.Count == 1)
				return matchingParamMethods[0];
			else
			{
				// In case of many matching methods, due to lack of data (e.g. System.Object and UnityEngine.Object both displaying as Object...).
				// Take the the first method that match the parameters.
				foreach (var m in matchingParamMethods)
				{
					if (this.MatchParameters(m.GetParameters(), parameters) == true)
						return m;
				}
			}
			return null;
		}

		private bool	MatchParameters(ParameterInfo[] parameters, string[] models)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				string		paramTrim = models[i].Trim();
				string[]	paramData = paramTrim.Split(' ');

				// Check type.
				// FullName might be null in this case:
				// public static T Object.Instantiate<T>(T original) where T : Object;
				if ((parameters[i].ParameterType.FullName != null &&
					 parameters[i].ParameterType.FullName.Contains(paramData[0]) == false) ||
					(parameters[i].ParameterType.Name != null &&
					 parameters[i].ParameterType.Name.Contains(paramData[0]) == false))
				{
					//Debug.Log("NoMatch");
					return false;
				}

				// Check name.
				if (paramData.Length == 2)
				{
					//Debug.Log(parameters[i].Name + "(" + paramData[1] + ")");
					if (parameters[i].Name.Contains(paramData[1]) == false)
					{
						//Debug.Log("NoMatch");
						return false;
					}
				}
			}
			return true;
		}
	}
}