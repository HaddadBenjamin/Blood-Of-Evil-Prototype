using System;
using System.Collections.Generic;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class KeyboardModule : CameraServerDataModule
	{
		public const int	ModuleID = 2;
		public const int	Priority = 10;
		public const string	Name = "Keyboard";

		private List<KeyCode>	lastKeysDown = new List<KeyCode>();

		public	KeyboardModule() : base(KeyboardModule.ModuleID, KeyboardModule.Priority, KeyboardModule.Name)
		{
		}

		public override void	OnGUI(ICameraData data)
		{
			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode != KeyCode.None &&
					this.lastKeysDown.Contains(Event.current.keyCode) == false)
				{
					//Debug.Log("Send down " + Event.current.keyCode);
					Utility.sharedBBuffer.Append((UInt16)Event.current.keyCode);
					Utility.sharedBBuffer.Append(false);

					data.TCPListener.BroadcastPacket(new CameraDataPacket(this.moduleID, Time.time, Utility.sharedBBuffer.Flush()));

					this.lastKeysDown.Add(Event.current.keyCode);
				}
			}
			else if (Event.current.type == EventType.KeyUp)
			{
				//Debug.Log("Send up " + Event.current.keyCode);

				Utility.sharedBBuffer.Append((UInt16)Event.current.keyCode);
				Utility.sharedBBuffer.Append(true);

				data.TCPListener.BroadcastPacket(new CameraDataPacket(this.moduleID, Time.time, Utility.sharedBBuffer.Flush()));

				this.lastKeysDown.Remove(Event.current.keyCode);
			}
		}
	}
}