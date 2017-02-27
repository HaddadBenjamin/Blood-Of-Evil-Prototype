using System;
using System.Text;
using UnityEngine;

namespace NGTools.NGGameConsole
{
	public class TimeData : DataConsole
	{
		public string	timeFormat = "HH:mm:ss.fff";
		public bool		displayCaptureFramerate = true;
		public bool		displayDeltaTime = true;
		public bool		displayFixedDeltaTime = true;
		public bool		displayFixedTime = true;
		public bool		displayFrameCount = true;
		public bool		displayRealtimeSinceStartup = true;
		public bool		displayRenderedFrameCount = true;
		public bool		displaySmoothDeltaTime = true;
		public bool		displayTime = true;
		public bool		displayTimeScale = true;
		public bool		displayTimeSinceLevelLoad = true;

		public override void	ShortGUI()
		{
			this.label.text = DateTime.Now.ToString(this.timeFormat);
			this.DrawSimpleShortGUI();
		}

		public override void	FullGUI()
		{
			StringBuilder	buffer = Utility.sharedBuffer;

			buffer.Length = 0;
			buffer.Append("Full Time: ");
			buffer.Append(DateTime.Now.ToString(this.timeFormat));

			if (this.displayCaptureFramerate == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Capture Framerate: ");
				buffer.Append(Time.captureFramerate);
			}

			if (this.displayDeltaTime == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Delta Time: ");
				buffer.Append(Time.deltaTime);
			}

			if (this.displayFixedDeltaTime == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Fixed Delta Time: ");
				buffer.Append(Time.fixedDeltaTime);
			}

			if (this.displayFixedTime == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Fixed Time: ");
				buffer.Append(Time.fixedTime);
			}

			if (this.displayFrameCount == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Frame Count: ");
				buffer.Append(Time.frameCount);
			}

			if (this.displayRealtimeSinceStartup == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Realtime Since Startup: ");
				buffer.Append(Time.realtimeSinceStartup);
			}

			if (this.displayRenderedFrameCount == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Rendered Frame Count: ");
				buffer.Append(Time.renderedFrameCount);
			}

			if (this.displaySmoothDeltaTime == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Smooth Delta Time: ");
				buffer.Append(Time.smoothDeltaTime);
			}

			if (this.displayTime == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Time: ");
				buffer.Append(Time.time);
			}

			if (this.displayTimeScale == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Time Scale: ");
				buffer.Append(Time.timeScale);
			}

			if (this.displayTimeSinceLevelLoad == true)
			{
				buffer.Append(Environment.NewLine);
				buffer.Append("Time Since Level Load: ");
				buffer.Append(Time.timeSinceLevelLoad);
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