using System;

namespace NGTools.NGRemoteScene
{
	/// <summary>
	/// Thrown when trying to access a non-existing Component.
	/// </summary>
	internal sealed class MissingComponentException : Exception
	{
		public readonly int	instanceID;

		public override string Message
		{
			get
			{
				return "Component \"" + this.instanceID + "\" is missing.";
			}
		}

		/// <summary>
		/// Thrown when trying to fetch a non-implemented TypeHandler.
		/// </summary>
		public	MissingComponentException(int instanceID)
		{
			this.instanceID = instanceID;
		}
	}
}