using System;
using System.Reflection;

namespace NGToolsEditor.NGConsole
{
	/// <summary>
	/// Class cloned from Unity's editor internal class "UnityEditorInternal.LogEntry". It contains the collapseCount.
	/// </summary>
	public sealed class UnityLogEntry
	{
		public object	instance;

		public string	condition { get { return (string)this.conditionField.GetValue(instance); } }
		public int		errorNum { get { return (int)this.errorNumField.GetValue(instance); } }
		public string	file { get { return (string)this.fileField.GetValue(instance); } }
		public int		line { get { return (int)this.lineField.GetValue(instance); } }
		public int		mode { get { return (int)this.modeField.GetValue(instance); } }
		public int		instanceID { get { return (int)this.instanceIDField.GetValue(instance); } }
		public int		identifier { get { return (int)this.identifierField.GetValue(instance); } }
		public int		isWorldPlaying { get { return (int)this.isWorldPlayingField.GetValue(instance); } }

		public int		collapseCount;

		private FieldInfo	conditionField;
		private FieldInfo	errorNumField;
		private FieldInfo	fileField;
		private FieldInfo	lineField;
		private FieldInfo	modeField;
		private FieldInfo	instanceIDField;
		private FieldInfo	identifierField;
		private FieldInfo	isWorldPlayingField;

		public	UnityLogEntry(Type logEntryType)
		{
			this.conditionField = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
			this.errorNumField = logEntryType.GetField("errorNum", BindingFlags.Instance | BindingFlags.Public);
			this.fileField = logEntryType.GetField("file", BindingFlags.Instance | BindingFlags.Public);
			this.lineField = logEntryType.GetField("line", BindingFlags.Instance | BindingFlags.Public);
			this.modeField = logEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public);
			this.instanceIDField = logEntryType.GetField("instanceID", BindingFlags.Instance | BindingFlags.Public);
			this.identifierField = logEntryType.GetField("identifier", BindingFlags.Instance | BindingFlags.Public);
			this.isWorldPlayingField = logEntryType.GetField("isWorldPlaying", BindingFlags.Instance | BindingFlags.Public);
		}

		public override string	ToString()
		{
			return
				"Condition=" + this.condition + Environment.NewLine +
				"ErrorNum=" + this.errorNum + Environment.NewLine +
				"File=" + this.file + Environment.NewLine +
				"Line=" + this.line + Environment.NewLine +
				"Mode=" + this.mode + Environment.NewLine +
				"InstanceID=" + this.instanceID + Environment.NewLine +
				"Identifier=" + this.identifier + Environment.NewLine +
				"IsWorldPlaying=" + this.isWorldPlaying + Environment.NewLine;
		}
	}
}