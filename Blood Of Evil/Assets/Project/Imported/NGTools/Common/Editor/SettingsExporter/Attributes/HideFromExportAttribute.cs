using System;

namespace NGToolsEditor
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class HideFromExportAttribute : Attribute
	{
	}
}