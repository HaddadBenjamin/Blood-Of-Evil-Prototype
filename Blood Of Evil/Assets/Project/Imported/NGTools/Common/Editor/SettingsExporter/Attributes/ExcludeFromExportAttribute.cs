using System;

namespace NGToolsEditor
{
	/// <summary>
	/// Explicitly excludes this class from array export.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
	public sealed class ExcludeFromExportAttribute : Attribute
	{
	}
}