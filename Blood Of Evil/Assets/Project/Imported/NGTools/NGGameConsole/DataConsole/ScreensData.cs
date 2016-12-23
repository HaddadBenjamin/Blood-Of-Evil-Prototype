using System.Text;
using UnityEngine;

namespace NGTools
{
	public class ScreensData : DataConsole
	{
		public override void	FullGUI()
		{
			StringBuilder	buffer = Utility.sharedBuffer;

			buffer.Length = 0;
			buffer.Append("Autorotate To Landscape Left: ");
			buffer.AppendLine(Screen.autorotateToLandscapeLeft.ToString());
			buffer.Append("Autorotate To Landscape Right: ");
			buffer.AppendLine(Screen.autorotateToLandscapeRight.ToString());
			buffer.Append("Autorotate To Portrait: ");
			buffer.AppendLine(Screen.autorotateToPortrait.ToString());
			buffer.Append("Autorotate To Portrait Upside Down: ");
			buffer.AppendLine(Screen.autorotateToPortraitUpsideDown.ToString());
			buffer.Append("Current Resolution: ");
			buffer.AppendLine(Screen.currentResolution.ToString());
			buffer.Append("DPI: ");
			buffer.AppendLine(Screen.dpi.ToString());
			buffer.Append("FullScreen: ");
			buffer.AppendLine(Screen.fullScreen.ToString());
			buffer.Append("Width: ");
			buffer.AppendLine(Screen.width.ToString());
			buffer.Append("Height: ");
			buffer.AppendLine(Screen.height.ToString());
			buffer.Append("Orientation: ");
			buffer.AppendLine(Screen.orientation.ToString());
			buffer.Append("Sleep Timeout: ");
			buffer.AppendLine(Screen.sleepTimeout.ToString());
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			buffer.Append("Show Cursor: ");
			buffer.AppendLine(Screen.showCursor.ToString());
			buffer.Append("Lock Cursor: ");
			buffer.AppendLine(Screen.lockCursor.ToString());
#else
			buffer.Append("Cursor Lock State: ");
			buffer.AppendLine(Cursor.lockState.ToString());
			buffer.Append("Cursor Visible: ");
			buffer.AppendLine(Cursor.visible.ToString());
#endif
			buffer.AppendLine();

			buffer.AppendLine("Resolution supported: ");
			for (int i = 0; i < Screen.resolutions.Length; i++)
				buffer.AppendLine(Screen.resolutions[i].ToString());


			for (int i = 0; i < Display.displays.Length; i++)
			{
				buffer.AppendLine();
				buffer.Append("Display ");
				buffer.Append(i);

				if (Display.displays[i] == Display.main)
					buffer.Append(" (main)");

				buffer.AppendLine(":");
				buffer.Append(" Rendering: ");
				buffer.Append(Display.displays[i].renderingWidth);
				buffer.Append(" x ");
				buffer.AppendLine(Display.displays[i].renderingHeight.ToString());
				buffer.Append(" System: ");
				buffer.Append(Display.displays[i].systemWidth);
				buffer.Append(" x ");
				buffer.Append(Display.displays[i].systemHeight);
			}

			this.label.text = buffer.ToString();
			GUILayout.TextArea(this.label.text, this.fullStyle);
		}

		public override string	Copy()
		{
			return this.label.text;
		}
	}
}