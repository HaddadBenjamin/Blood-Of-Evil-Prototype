using UnityEngine;

namespace NGTools.NGRemoteScene
{
	internal sealed class TouchModule : CameraServerDataModule
	{
		public const int	ModuleID = 4;
		public const int	Priority = 500;
		public const string	Name = "Touch";

		public	TouchModule() : base(TouchModule.ModuleID, TouchModule.Priority, TouchModule.Name)
		{
		}

		public override void	Update(ICameraData data)
		{
			if (Input.touchCount == 0)
				return;

			Utility.sharedBBuffer.Clear();
			Utility.sharedBBuffer.Append((byte)Input.touchCount);

			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch	touch = Input.GetTouch(i);

				Utility.sharedBBuffer.Append(touch.position.x);
				Utility.sharedBBuffer.Append(Screen.height - touch.position.y);
				Utility.sharedBBuffer.Append(touch.fingerId);
			}

			data.TCPListener.BroadcastPacket(new CameraDataPacket(this.moduleID, Time.time, Utility.sharedBBuffer.Flush()));
		}
	}
}