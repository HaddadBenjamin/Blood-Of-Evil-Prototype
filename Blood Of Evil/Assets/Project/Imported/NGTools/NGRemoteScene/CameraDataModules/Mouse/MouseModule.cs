using System;
using UnityEngine;

namespace NGTools.NGRemoteScene
{
	[Flags]
	public enum MouseButtons
	{
		None = 0,
		Button0,
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,
		Button20,
		Button21,
		Button22,
		Button23,
		Button24,
		Button25,
		Button26,
		Button27,
		Button28,
		Button29,
		Button30,
		Button31
	}

	internal sealed class MouseModule : CameraServerDataModule
	{
		public const int	ModuleID = 3;
		public const int	Priority = 100;
		public const string	Name = "Mouse";

		private float			lastMouseX;
		private float			lastMouseY;
		private MouseButtons	lastMouseButtons;

		public	MouseModule() : base(MouseModule.ModuleID, MouseModule.Priority, MouseModule.Name)
		{
		}

		public override void	Update(ICameraData data)
		{
			bool	mouseHasChange = false;
			MouseButtons	b = 0;

			// Save mouse inputs for the first 3 buttons.
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) == true)
					b |= (MouseButtons)(1 << i);
			}

			if (this.lastMouseButtons != b)
				mouseHasChange = true;
			else if (this.lastMouseX != Input.mousePosition.x ||
					 this.lastMouseY != Input.mousePosition.y)
			{
				mouseHasChange = true;
			}

			if (mouseHasChange == true)
			{
				Utility.sharedBBuffer.Clear();
				Utility.sharedBBuffer.Append(Input.mousePosition.x);
				Utility.sharedBBuffer.Append(Screen.height - Input.mousePosition.y);
				Utility.sharedBBuffer.Append((Int32)b);

				data.TCPListener.BroadcastPacket(new CameraDataPacket(this.moduleID, Time.time, Utility.sharedBBuffer.Flush()));

				this.lastMouseX = Input.mousePosition.x;
				this.lastMouseY = Input.mousePosition.y;
				this.lastMouseButtons = b;
			}
		}
	}
}