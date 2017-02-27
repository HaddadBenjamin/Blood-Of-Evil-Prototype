﻿namespace NGToolsEditor.NGConsole
{
	internal sealed class FastestPreset : Preset
	{
		public override void	SetSettings(NGSettings instance)
		{
			instance.general.openMode = NGSettings.ModeOpen.AssetDatabaseOpenAsset;
			//instance.general.horizontalScrollbar = false;
			instance.general.filterUselessStackFrame = false;
			//instance.general.giveFocusToEditor = true;
			//instance.general.forceFocusOnModifier = EventModifiers.Alt;

			instance.log.displayTime = false;
			//instance.log.timeFormat = "HH:mm:ss.fff";

			instance.stackTrace.displayFilepath = NGSettings.PathDisplay.Hidden;
			instance.stackTrace.displayRelativeToAssets = false;

			instance.stackTrace.displayReturnValue = false;
			instance.stackTrace.displayReflectedType = NGSettings.StackTraceSettings.DisplayReflectedType.None;
			instance.stackTrace.displayArgumentType = false;
			instance.stackTrace.displayArgumentName = false;

			//instance.stack.previewLinesBeforeStackFrame = 3;
			//instance.stack.previewLinesAfterStackFrame = 3;
			instance.stackTrace.displayTabAsSpaces = 0;
		}
	}
}