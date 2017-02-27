using System;

namespace NGToolsEditor
{
	/// <summary>
	/// <para>Gives this public non-static field a default value when calling Utility.LoadEditorPref.</para>
	/// <para>Only works on integer, float, bool, string and enum.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DefaultValueEditorPrefAttribute : Attribute
	{
		public readonly object	defaultValue;

		public	DefaultValueEditorPrefAttribute(object defaultValue)
		{
			this.defaultValue = defaultValue;
		}
	}
}