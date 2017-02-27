using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	internal sealed class VerbosePreset : Preset
	{
		public override void	SetSettings(NGSettings instance)
		{
			//instance.general.openMode = ConsoleSettings.ModeOpen.AssetDatabaseOpenAsset;
			//instance.general.horizontalScrollbar = true;
			instance.general.filterUselessStackFrame = false;

			//instance.log.giveFocusToEditor = true;
			instance.log.forceFocusOnModifier = EventModifiers.Alt;
			instance.log.displayTime = true;
			//instance.log.timeFormat = "HH:mm:ss.fff";

			instance.stackTrace.displayFilepath = NGSettings.PathDisplay.Visible;
			instance.stackTrace.displayRelativeToAssets = false;

			instance.stackTrace.displayReturnValue = true;
			instance.stackTrace.displayReflectedType = NGSettings.StackTraceSettings.DisplayReflectedType.NamespaceAndClass;
			instance.stackTrace.displayArgumentType = true;
			instance.stackTrace.displayArgumentName = true;

			instance.stackTrace.previewLinesBeforeStackFrame = 4;
			instance.stackTrace.previewLinesAfterStackFrame = 4;
			instance.stackTrace.displayTabAsSpaces = 4;
		}
	}
}