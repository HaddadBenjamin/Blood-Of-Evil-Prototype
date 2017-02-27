using NGTools;
using NGTools.NGRemoteScene;
using System;

namespace NGToolsEditor.NGRemoteScene
{
	public abstract class ArgumentDrawer
	{
		public readonly Type	type;
		public readonly string	name;

		protected object	value;

		public	ArgumentDrawer(string name, Type type)
		{
			this.name = name;
			this.type = type;

			try
			{
				this.value = Activator.CreateInstance(type);
			}
			catch
			{
			}
		}

		public abstract void	OnGUI();
		public abstract void	Save(string path);
		public abstract void	Load(string path);

		public void	Append(ByteBuffer buffer)
		{
			TypeHandler	typeHandler = TypeHandlersManager.GetTypeHandler(this.type);

			if (typeHandler == null)
				throw new NotImplementedException("TypeHandler for \"" + this.type + "\" was not found.");
			else
				typeHandler.Serialize(buffer, this.type, this.value);
		}
	}
}