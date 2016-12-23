using System;
using System.Reflection;

namespace NGToolsEditor
{
	public abstract class EditorPrefType
	{
		public abstract bool	CanHandle(Type type);
		public abstract void	DirectSave(object instance, Type type, string path);

		public virtual void		Save(object instance, Type type, string path)
		{
			this.DirectSave(instance, type, path);
		}

		public virtual void		Load(object instance, Type type, string path)
		{
			throw new NotImplementedException(this.GetType().FullName);
		}

		public abstract object	Fetch(object instance, Type type, string path);

		protected object		GetDefaultValue(FieldInfo field)
		{
			foreach (DefaultValueEditorPrefAttribute attribute in field.GetCustomAttributes(typeof(DefaultValueEditorPrefAttribute), true))
				return attribute.defaultValue;
			return null;
		}
	}
}