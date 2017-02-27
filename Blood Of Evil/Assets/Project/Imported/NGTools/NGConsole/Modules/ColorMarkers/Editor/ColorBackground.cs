using System;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class ColorBackground : ISerializationCallbackReceiver
	{
		public string	name;
		public Color	color;

		public void	OnBeforeSerialize()
		{
		}

		public void	OnAfterDeserialize()
		{
			// Force a default value. People might easily forget the alpha of the color.
			if (this.name == string.Empty)
			{
				name = "New Marker";
				color = Color.black;
			}
		}
	}
}