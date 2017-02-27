using System;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	public sealed class InputCommand
	{
		/// <summary>
		/// String appended to the name to fetch the locale.
		/// </summary>
		public const string	DescriptionLocalizationSuffix = "_Description";

		public string	name;
		public KeyCode	keyCode;
		public bool		control;
		public bool		shift;
		public bool		alt;

		/// <summary></summary>
		/// <param name="name">Name of the command. Used to call this command. Is also used as a Localization Key.</param>
		/// <param name="keyCode">KeyCode required by this command.</param>
		/// <param name="control">Whether modifier control is required.</param>
		/// <param name="shift">Whether modifier shift is required.</param>
		/// <param name="alt">Whether modifier alt is required.</param>
		public	InputCommand(string name, KeyCode keyCode, bool control, bool shift, bool alt)
		{
			this.name = name;
			this.keyCode = keyCode;
			this.control = control;
			this.shift = shift;
			this.alt = alt;
		}

		public bool	Check()
		{
			EventType	targetKey = EventType.KeyDown;

			// Exception! Because this event is not passed as KeyDown but only at KeyUp.
			if (this.keyCode == KeyCode.Tab && this.control == true)
				targetKey = EventType.KeyUp;

			return Event.current.type == targetKey &&
				   this.keyCode != KeyCode.None &&
				   Event.current.keyCode == this.keyCode &&
				   Event.current.control == this.control &&
				   Event.current.alt == this.alt &&
				   Event.current.shift == this.shift;
		}
	}
}