using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	[Serializable]
	[Exportable(ExportableAttribute.ArrayOptions.Add)]
	public class StreamLog : ISettingExportable
	{
		/// <summary>Called whenever a filter's setting is altered, enabled or disabled.</summary>
		[NonSerialized]
		public Action	FilterAltered;
		/// <summary>Called whenever a type is toggled (Normal, warning, error, exception).</summary>
		[NonSerialized]
		public Action	OptionAltered;

		[Exportable]
		public string	name;
		[Exportable]
		public bool		onlyCategory;
		[Exportable]
		public bool		displayLog;
		[Exportable]
		public bool		displayWarning;
		[Exportable]
		public bool		displayError;
		[Exportable]
		public bool		displayException;
		public bool		addConsumedLog;
		[Exportable]
		public bool		consumeLog;

		[NonSerialized]
		public int	totalCount;
		[NonSerialized]
		public int	logCount;
		[NonSerialized]
		public int	warningCount;
		[NonSerialized]
		public int	errorCount;
		[NonSerialized]
		public int	exceptionCount;

		public RowsDrawer	rowsDrawer;
		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public GroupFilters	groupFilters;

		[NonSerialized]
		protected NGConsoleWindow	console;
		[NonSerialized]
		protected IStreams	container;

		[NonSerialized]
		protected GUIContent	logContent;
		[NonSerialized]
		protected GUIContent	warningContent;
		[NonSerialized]
		protected GUIContent	errorContent;
		[NonSerialized]
		protected GUIContent	exceptionContent;
		[NonSerialized]
		protected GUIContent	consumeLogContent;

		[NonSerialized]
		private int	lastIndexConsummed = -1;

		public	StreamLog()
		{
			this.name = "New stream";
			this.displayLog = true;
			this.displayWarning = true;
			this.displayError = true;
			this.displayException = true;
			this.addConsumedLog = false;
			this.consumeLog = false;

			this.rowsDrawer = new RowsDrawer();
			this.groupFilters = new GroupFilters();
		}

		public virtual void	Init(NGConsoleWindow console, IStreams container)
		{
			this.console = console;
			this.container = container;

			this.console.CheckNewLogConsume += this.ConsumeLog;
			this.console.PropagateNewLog += this.AddLog;

			this.rowsDrawer.Init(this.console, this.console);
			this.rowsDrawer.RowDeleted += this.DecrementCounts;
			this.rowsDrawer.LogContextMenu += this.FillContextMenu;
			this.rowsDrawer.Clear();

			this.groupFilters.FilterAltered += this.RefreshFilteredRows;

			this.totalCount = 0;
			this.logCount = 0;
			this.warningCount = 0;
			this.errorCount = 0;
			this.exceptionCount = 0;

			this.logContent = new GUIContent(Utility.GetConsoleIcon("console.infoicon.sml"));
			this.warningContent = new GUIContent(Utility.GetConsoleIcon("console.warnicon.sml"));
			this.errorContent = new GUIContent(Utility.GetConsoleIcon("console.erroricon.sml"));
			this.exceptionContent = new GUIContent(this.errorContent.image);
			this.consumeLogContent = new GUIContent("", AssetDatabase.LoadAssetAtPath(Path.Combine(Preferences.RootPath, "Common/Misc/pacman.png"), typeof(Texture2D)) as Texture2D);
		}

		public virtual void	Uninit()
		{
			this.groupFilters.FilterAltered -= this.RefreshFilteredRows;
			this.rowsDrawer.LogContextMenu -= this.FillContextMenu;
			this.rowsDrawer.RowDeleted -= this.DecrementCounts;
			this.rowsDrawer.Uninit();

			if (this.console != null)
			{
				this.console.CheckNewLogConsume -= this.ConsumeLog;
				this.console.PropagateNewLog -= this.AddLog;
			}
		}

		public virtual void	OnTabGUI(int i)
		{
			EditorGUI.BeginChangeCheck();
			GUILayout.Toggle(i == this.container.WorkingStream, (this.onlyCategory == true ? "[" : string.Empty) + this.name + (this.onlyCategory == true ? "]" : string.Empty) + " (" + this.totalCount + ")", Preferences.Settings.general.menuButtonStyle);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Event.current.button == 0)
				{
					this.container.FocusStream(i);
				}
				else
				{
					// Show context menu on right click.
					if (Event.current.button == 1)
					{
						GenericMenu	menu = new GenericMenu();
						menu.AddItem(new GUIContent(LC.G("MainModule_ChangeName")), false, this.ChangeStreamName);
						menu.AddItem(new GUIContent(LC.G("MainModule_ToggleCategory")), this.onlyCategory, this.ToggleCategory);
						menu.AddItem(new GUIContent(LC.G("Delete")), false, this.DeleteStream, i);
						menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
					}
					else if (Event.current.button == 2)
					{
						this.DeleteStream(i);
					}
				}
			}
		}

		public virtual Rect	OnGUI(Rect r)
		{
			float	yOrigin = r.y;
			float	maxWidth = r.width;
			float	maxHeight = r.height;

			// Draw options.
			r = this.DrawOptions(r);

			// Draw filters on the right.
			r.width = maxWidth - r.x;
			r = this.DrawFilters(r);

			// Draw rows in a new line.
			r.x = 0F;
			r.y += r.height;
			r.width = maxWidth;
			r.height = maxHeight - (r.y - yOrigin);
			r = this.rowsDrawer.DrawRows(r, null);
			return r;
		}

		/// <summary>Checks whether Row is displayed regarding its log type.</summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public virtual bool	CanDisplay(Row row)
		{
			if ((row.log.mode & Mode.ScriptingException) != 0)
			{
				if (this.displayException == false)
					return false;
			}
			else if ((row.log.mode & Mode.Error) != 0 ||
					 (row.log.mode & Mode.Fatal) != 0 ||
					 (row.log.mode & Mode.ScriptingError) != 0 ||
					 (row.log.mode & Mode.ScriptCompileError) != 0 ||
					 (row.log.mode & Mode.Assert) != 0 ||
					 (row.log.mode & Mode.AssetImportError) != 0)
			{
				if (this.displayError == false)
					return false;
			}
			else if ((row.log.mode & Mode.ScriptingWarning) != 0 ||
					 (row.log.mode & Mode.ScriptCompileWarning) != 0 ||
					 (row.log.mode & Mode.AssetImportWarning) != 0)
			{
				if (this.displayWarning == false)
					return false;
			}
			else if (this.displayLog == false)
				return false;

			return true;
		}

		private List<int>	consumedLogs = new List<int>();

		/// <summary>
		/// If this stream consumes incoming logs, it checks if the given <paramref name="row"/> is filtered and consumes it.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="row"></param>
		public virtual void	ConsumeLog(int i, Row row)
		{
			// StreamLog does not receive output from compiler.
			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
				return;

			if (this.consumeLog == true)
				//(this.addConsumedLog == true ||
				// row.isConsumed == false) &&
			{
				// Category has a priority over all rules.
				string	category;
				int		hash = row.log.fileHash + row.log.line;

				if (MainModule.methodsCategories.TryGetValue(hash, out category) == false)
				{
					ILogContentGetter	log = row as ILogContentGetter;

					if (log != null)
						category = log.Category;
					else
						category = null;

					MainModule.methodsCategories.Add(hash, category);
				}

				if (this.onlyCategory == true)
				{
					if (string.IsNullOrEmpty(category) == false && this.name == category)
					{
						row.isConsumed = true;
						this.consumedLogs.Add(i);
						this.lastIndexConsummed = i;
					}
				}
				else if (this.groupFilters.Filter(row) == true)
				{
					row.isConsumed = true;
					this.consumedLogs.Add(i);
					this.lastIndexConsummed = i;
				}
			}
		}

		public virtual void	AddLog(int i, Row row)
		{
			// StreamLog does not receive output from compiler.
			if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptCompileWarning)) != 0)
				return;

			// Category has a priority over all rules.
			string	category;
			int		hash = row.log.fileHash + row.log.line;

			if (MainModule.methodsCategories.TryGetValue(hash, out category) == false)
			{
				ILogContentGetter	log = row as ILogContentGetter;

				if (log != null)
					category = log.Category;
				else
					category = null;

				MainModule.methodsCategories.Add(hash, category);
			}

			//Utility.AssertFile(true, "I=" + i + " " + this.addConsumedLog + " && " + row.isConsumed + " && " + i + " != " + this.lastIndexConsummed);
			if (this.addConsumedLog == false)
			{
				if (row.isConsumed == true &&
					//this.consumedLogs.Contains(i) == false)
					i != this.lastIndexConsummed)
				{
					return;
				}
			}

			if (string.IsNullOrEmpty(category) == false || this.onlyCategory == true)
			{
				if (this.name == category)
				{
					//Utility.AssertFile(true, "added " + i);

					// Count row, but do not display if it is not options compliant.
					this.CountLog(row);
					if (this.CanDisplay(row) == true)
						this.rowsDrawer.Add(i);
				}
				return;
			}

			// Exclude filtered row.
			if (this.groupFilters.Filter(row) == true)
			{
				//Utility.AssertFile(true, "added " + i);

				// Count row, but do not display if it is not options compliant.
				this.CountLog(row);
				if (this.CanDisplay(row) == true)
					this.rowsDrawer.Add(i);
			}
		}

		public virtual void	RefreshFilteredRows()
		{
			// Save current selected logs.
			List<Row>	rows = new List<Row>();
			List<Row[]>	backup = new List<Row[]>();

			foreach (RowsDrawer.Vars vars in this.rowsDrawer.perWindowVars.Each())
			{
				rows.Clear();

				foreach (var item in vars.Each())
					rows.Add(this.rowsDrawer.rows.GetRow(this.rowsDrawer[item]));

				backup.Add(rows.ToArray());
			}

			this.Clear();

			// Reimport all logs.
			for (int i = 0; i < this.rowsDrawer.rows.CountRows(); i++)
				this.AddLog(i, this.rowsDrawer.rows.GetRow(i));

			int	logIndex = 0;
			int	l = 0;

			// Try to restore selected logs.
			foreach (RowsDrawer.Vars vars in this.rowsDrawer.perWindowVars.Each())
			{
				for (int j = 0; j < backup[l].Length; j++)
				{
					for (int i = logIndex; i < this.rowsDrawer.Count && logIndex < backup[l].Length; i++)
					{
						if (backup[l][j] == this.rowsDrawer.rows.GetRow(this.rowsDrawer[i]))
						{
							vars.AddSelection(i);
							++logIndex;
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Resets counters and clears all Rows in this stream.
		/// </summary>
		public virtual void	Clear()
		{
			this.consumedLogs.Clear();
			this.lastIndexConsummed = -1;
			this.totalCount = 0;
			this.logCount = 0;
			this.warningCount = 0;
			this.errorCount = 0;
			this.exceptionCount = 0;
			this.rowsDrawer.Clear();
		}

		public void	FillContextMenu(GenericMenu menu, Row row)
		{
			menu.AddSeparator("");

			for (int i = 0; i < this.groupFilters.filters.Count; i++)
				this.groupFilters.filters[i].ContextMenu(menu, row, i);
		}

		protected virtual Rect	DrawOptions(Rect r)
		{
			float	maxWidth = r.width;

			EditorGUI.BeginChangeCheck();

			r.height = EditorGUIUtility.singleLineHeight;

			this.logContent.text = this.logCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.logContent).x;
			this.displayLog = GUI.Toggle(r, this.displayLog, this.logContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.warningContent.text = this.warningCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.warningContent).x;
			this.displayWarning = GUI.Toggle(r, this.displayWarning, this.warningContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.errorContent.text = this.errorCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.errorContent).x;
			this.displayError = GUI.Toggle(r, this.displayError, this.errorContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			this.exceptionContent.text = this.exceptionCount.ToString();
			r.width = Preferences.Settings.general.menuButtonStyle.CalcSize(this.exceptionContent).x;
			this.displayException = GUI.Toggle(r, this.displayException, this.exceptionContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			// Update Unity Console only if required.
			if (EditorGUI.EndChangeCheck() == true)
			{
				this.RefreshFilteredRows();

				if (this.OptionAltered != null)
					this.OptionAltered();

				Utility.RepaintConsoleWindow();
			}

			this.consumeLogContent.tooltip = LC.G("StreamLog_ConsumeLogTooltip");
			this.consumeLog = GUI.Toggle(r, this.consumeLog, this.consumeLogContent, Preferences.Settings.general.menuButtonStyle);
			r.x += r.width;

			r.width = maxWidth - r.width * 4F;

			return r;
		}

		protected virtual Rect	DrawFilters(Rect r)
		{
			EditorGUI.BeginChangeCheck();

			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal();
				{
					this.groupFilters.OnGUI();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.x = 0F;
			r.width = this.console.position.width;

			for (int i = 0; i < this.groupFilters.filters.Count; i++)
			{
				if (this.groupFilters.filters[i].Enabled == true)
				{
					r.y += r.height;

					GUILayout.BeginArea(r);
					{
						GUILayout.BeginHorizontal();
						{
							this.groupFilters.filters[i].OnGUI();
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
				}
			}

			if (EditorGUI.EndChangeCheck() == true)
			{
				this.RefreshFilteredRows();

				if (this.FilterAltered != null)
					this.FilterAltered();
			}

			return r;
		}

		private void	DeleteStream(object data)
		{
			int	i = (int)data;

			this.container.DeleteStream(i);
		}

		private void	ChangeStreamName()
		{
			PromptWindow.Start(this.name, this.RenameStream, null);
		}

		private void	ToggleCategory()
		{
			this.onlyCategory = !this.onlyCategory;
		}

		private void	RenameStream(object data, string newName)
		{
			if (string.IsNullOrEmpty(newName) == false)
				this.name = newName;
		}

		protected void	CountLog(Row row)
		{
			if ((row.log.mode & Mode.ScriptingException) != 0)
				++this.exceptionCount;
			else if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptingError | Mode.Fatal | Mode.Error | Mode.Assert | Mode.AssetImportError)) != 0)
			{
				++this.errorCount;
			}
			else if ((row.log.mode & (Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning)) != 0)
			{
				++this.warningCount;
			}
			else
				++this.logCount;
			++this.totalCount;
		}

		private void	DecrementCounts(Row row)
		{
			if ((row.log.mode & Mode.ScriptingException) != 0)
				--this.exceptionCount;
			else if ((row.log.mode & (Mode.ScriptCompileError | Mode.ScriptingError | Mode.Fatal | Mode.Error | Mode.Assert | Mode.AssetImportError)) != 0)
			{
				--this.errorCount;
			}
			else if ((row.log.mode & (Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning)) != 0)
			{
				--this.warningCount;
			}
			else
				--this.logCount;
			--this.totalCount;
		}

		public void	PreExport()
		{
		}

		public void	PreImport()
		{
		}

		public void	PostImport()
		{
			if (this.OptionAltered != null)
				this.OptionAltered();

			Utility.RepaintConsoleWindow();
		}
	}
}