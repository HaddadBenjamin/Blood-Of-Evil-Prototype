using System;
using System.Collections.Generic;
using System.Reflection;

namespace NGToolsEditor.NGAssetsFinder
{
	internal sealed class ContainerType
	{
		public bool	HasType { get { return this.containObject == true || this.fields.Count > 0 || this.properties.Count > 0; } }

		public Type					type;
		public List<FieldInfo>		fields;
		public List<PropertyInfo>	properties;
		public bool					isInstance;
		public bool					containObject;

		public	ContainerType(Type type, bool isInstance = false, bool containObject = false)
		{
			this.type = type;
			this.fields = new List<FieldInfo>();
			this.properties = new List<PropertyInfo>();
			this.isInstance = isInstance;
			this.containObject = containObject;
		}
	}
}