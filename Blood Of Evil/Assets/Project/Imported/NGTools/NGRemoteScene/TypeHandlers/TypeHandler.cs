using System;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// <para>Defines a behaviour for a type that MUST be known in both server and client sides.</para>
	/// <para>When implementing a TypeHandler, you MUST create its drawer by implementing a class inheriting from TypeHandlerDrawer.</para>
	/// <para>All primary types from .NET and most common types from UnityEngine are implemented.</para>
	/// <remarks>
	/// <para>If you define a TypeHandler for your custom class Foo, it will only work if this type is present in the project.</para>
	/// <para>NG Remote has been thought to work from an empty project, therefore implementation of custom classes should be avoided.</para>
	/// </remarks>
	/// </summary>
	/// <seealso cref="NGToolsEditor.TypeHandlerDrawer"/>
	public abstract class TypeHandler
	{
		protected static ByteBuffer	buffer = new ByteBuffer(64);

		protected Type	type;

		protected	TypeHandler()
		{
		}

		protected	TypeHandler(Type type)
		{
			this.type = type;
		}

		public abstract bool	CanHandle(Type type);

		/// <summary>
		/// <para>Serializes the <paramref name="instance"/> in a network-readable format into <paramref name="buffer"/>.</para>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="fieldType"></param>
		/// <param name="instance"></param>
		public abstract void	Serialize(ByteBuffer buffer, Type fieldType, object instance);

		public byte[]			Serialize(Type fieldType, object instance)
		{
			this.Serialize(TypeHandler.buffer, fieldType, instance);
			return TypeHandler.buffer.Flush();
		}

		public byte[]			Serialize(object instance)
		{
			this.Serialize(TypeHandler.buffer, this.type, instance);
			return TypeHandler.buffer.Flush();
		}

		/// <summary>
		/// <para>Deserializes the network-readable value from <paramref name="buffer"/>.</para>
		/// <para>If an error occurs and the buffer is not fully consumed, NetField will backup. Therefore there is no need to handle all issues.</para>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="fieldType"></param>
		/// <returns></returns>
		public abstract object	Deserialize(ByteBuffer buffer, Type fieldType);

		/// <summary>
		/// <para>Deserializes the network-readable value from <paramref name="buffer"/> and gets the real value from it.</para>
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="fieldType"></param>
		/// <returns></returns>
		public virtual object	DeserializeRealValue(NGServerScene manager, ByteBuffer buffer, Type fieldType)
		{
			return this.Deserialize(buffer, fieldType);
		}
	}
}