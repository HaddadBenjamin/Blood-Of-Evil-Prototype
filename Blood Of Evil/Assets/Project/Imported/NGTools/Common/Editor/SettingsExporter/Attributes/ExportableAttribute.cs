using System;

namespace NGToolsEditor
{
	/// <summary>
	/// <para>Allows the field/property to be exported/imported.</para>
	/// <para>Sets the behaviour of a class when applied to a class.</para>
	/// <para>Makes EditorWindow exportable by the wizard.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ExportableAttribute : Attribute
	{
		/// <summary>Sets the default export/import option.</summary>
		[Flags]
		public enum ArrayOptions
		{
			/// <summary>Adds a new element in the array.</summary>
			Add = 1,
			/// <summary>Overwrites an existing element when available, otherwise do nothing.</summary>
			Overwrite = 2,
			/// <summary>Forbids the import option to be altered. Immutable classes must have a default public constructor.</summary>
			Immutable = 4,
		}

		public readonly ArrayOptions	options = ArrayOptions.Add;
		public readonly string[]		fields;

		public	ExportableAttribute()
		{
		}

		public	ExportableAttribute(ArrayOptions options)
		{
			this.options = options;
		}

		/// <summary>
		/// Forces export of fields on a native class.
		/// </summary>
		/// <param name="fields"></param>
		public	ExportableAttribute(params string[] fields)
		{
			this.fields = fields;
		}
	}
}