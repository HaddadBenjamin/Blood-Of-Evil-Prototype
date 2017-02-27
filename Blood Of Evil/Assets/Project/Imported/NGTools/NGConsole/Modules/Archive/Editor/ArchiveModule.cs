using NGTools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable, VisibleModule(150)]
	internal sealed class ArchiveModule : Module, IRows
	{
		[Serializable]
		private sealed class Vars
		{
			public int	workingFolder;
		}

		[Serializable, ExcludeFromExport]
		private sealed class UnexportableFolder : Folder
		{
		}

		[Serializable]
		public class Folder
		{
			[Exportable]
			public string			name;
			public RowsDrawer		rowsDrawer = new RowsDrawer();
			public List<LogNote>	notes = new List<LogNote>();
			[NonSerialized]
			public Stack<LogNote>	delayAdd;

			public void	Init(NGConsoleWindow editor, IRows rows)
			{
				this.rowsDrawer.Init(editor, rows);

				for (int i = 0; i < this.notes.Count; i++)
					this.notes[i].row.Init(editor, this.notes[i].row.log);

				this.delayAdd = new Stack<LogNote>();
			}

			public void	Uninit()
			{
				this.rowsDrawer.Uninit();

				for (int i = 0; i < this.notes.Count; i++)
					this.notes[i].row.Uninit();
			}
		}

		[Serializable]
		public sealed class LogNote
		{
			[Exportable]
			public Row		row;
			[Exportable]
			public string	note;
		}

		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		public List<Folder>	folders;

		/// <summary>
		/// Delay add/delete log action to prevent changing Rows array while drawing.
		/// </summary>
		[NonSerialized]
		private Stack<LogNote>	delayDelete;

		[NonSerialized]
		private Rect	noteRect;
		[NonSerialized]
		private LogNote	viewingNote;

		[SerializeField]
		private PerWindowVars<Vars>	perWindowVars;

		[NonSerialized]
		private Vars	currentVars;

		public	ArchiveModule()
		{
			this.name = "Archive";
			this.folders = new List<Folder>();
			this.folders.Add(new UnexportableFolder() { name = "Common" });
			this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnEnable(NGConsoleWindow editor, int id)
		{
			base.OnEnable(editor, id);

			this.console.UpdateTick += Update;

			// In case the data is corrupted, restart the instance.
			if (this.folders == null || this.folders.Count == 0)
			{
				this.folders = new List<Folder>();
				this.folders.Add(new UnexportableFolder() { name = "Common" });
			}

			for (int i = 0; i < this.folders.Count; i++)
				this.InitFolder(this.folders[i]);

			RowsDrawer.GlobalLogContextMenu += this.ArchiveFromContextMenu;

			this.delayDelete = new Stack<LogNote>();

			if (this.perWindowVars == null)
				this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnDisable()
		{
			this.console.UpdateTick -= Update;

			for (int i = 0; i < this.folders.Count; i++)
				this.UninitFolder(this.folders[i]);

			RowsDrawer.GlobalLogContextMenu -= this.ArchiveFromContextMenu;
		}

		public override void	OnGUI(Rect r)
		{
			this.currentVars = this.perWindowVars.Get(Utility.drawingWindow);

			r = this.DrawFolders(r);

			this.folders[this.currentVars.workingFolder].rowsDrawer.DrawRows(r, false);

			if (this.viewingNote != null && string.IsNullOrEmpty(this.viewingNote.note) == false)
			{
				if (Event.current.type == EventType.MouseMove)
				{
					if (this.noteRect.Contains(Event.current.mousePosition) == false)
					{
						this.viewingNote = null;
						return;
					}
				}

				this.noteRect.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.DrawRect(this.noteRect, Preferences.Settings.stackTrace.previewSourceCodeBackgroundColor);
				GUI.Label(this.noteRect, this.viewingNote.note, Preferences.Settings.stackTrace.previewSourceCodeStyle);
				this.noteRect.y -= EditorGUIUtility.singleLineHeight;

				Utility.drawingWindow.Repaint();
			}
		}

		private Rect	DrawFolders(Rect r)
		{
			float	height = r.height;

			r.height = Constants.DefaultSingleLineHeight;

			// Switch stream
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchNextStreamCommand) == true)
			{
				this.currentVars.workingFolder += 1;
				if (this.currentVars.workingFolder >= this.folders.Count)
					this.currentVars.workingFolder = 0;

				Event.current.Use();
			}
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchPreviousStreamCommand) == true)
			{
				this.currentVars.workingFolder -= 1;
				if (this.currentVars.workingFolder < 0)
					this.currentVars.workingFolder = this.folders.Count - 1;

				Event.current.Use();
			}

			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal();
				{
					for (int i = 0; i < this.folders.Count; i++)
					{
						EditorGUI.BeginChangeCheck();
						GUILayout.Toggle(i == this.currentVars.workingFolder, this.folders[i].name + " (" + this.folders[i].rowsDrawer.Count + ")", Preferences.Settings.general.menuButtonStyle);
						if (EditorGUI.EndChangeCheck() == true)
						{
							if (Event.current.button == 0)
								this.currentVars.workingFolder = i;
							// Forbid to alter the main folder.
							else if (Conf.DebugMode != Conf.DebugModes.None || i > 0)
							{
								// Show context menu on right click.
								if (Event.current.button == 1)
								{
									GenericMenu	menu = new GenericMenu();
									menu.AddItem(new GUIContent(LC.G("ArchiveModule_ChangeName")), false, this.ChangeStreamName, i);
									if (i > 0)
										menu.AddItem(new GUIContent(LC.G("Delete")), false, this.DeleteFolder, i);
									if (Conf.DebugMode != Conf.DebugModes.None)
										menu.AddItem(new GUIContent("Clear"), false, (d) => { this.folders[(int)d].notes.Clear();this.folders[(int)d].rowsDrawer.Clear(); ; }, i);
									menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
								}
								else if (Event.current.button == 2)
								{
									if (Conf.DebugMode != Conf.DebugModes.None && i > 0)
										this.DeleteFolder(i);
								}
							}
						}

						if (DragAndDrop.GetGenericData("n") != null)
						{
							Rect	toggleRect = GUILayoutUtility.GetLastRect();

							// Check drop Row.
							if (toggleRect.Contains(Event.current.mousePosition))
							{
								if (Event.current.type == EventType.DragUpdated)
								{
									LogNote	note = DragAndDrop.GetGenericData("n") as LogNote;

									if (this.folders[i].notes.Contains(note) == false)
										DragAndDrop.visualMode = DragAndDropVisualMode.Move;
									else
										DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

									Event.current.Use();
								}
								else if (Event.current.type == EventType.DragPerform)
								{
									DragAndDrop.AcceptDrag();

									LogNote	note = DragAndDrop.GetGenericData("n") as LogNote;

									this.delayDelete.Push(note);
									this.folders[i].delayAdd.Push(note);

									Event.current.Use();
								}

								Utility.drawingWindow.Repaint();
							}

							if (Event.current.type == EventType.DragExited ||
								Event.current.type == EventType.MouseUp)
							{
								DragAndDrop.PrepareStartDrag();
							}
						}
					}

					if (GUILayout.Button("+", Preferences.Settings.general.menuButtonStyle) == true)
					{
						Folder	folder = new Folder() { name = "Folder " + this.folders.Count };

						this.InitFolder(folder);

						this.folders.Add(folder);

						if (this.folders.Count == 1)
							this.currentVars.workingFolder = 0;
					}

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.y += r.height + 2F;

			r.height = height - r.height - 2F;

			return r;
		}

		public override void	OnEnter()
		{
			base.OnEnter();

			this.console.BeforeGUIHeaderRightMenu += this.GUIExport;
		}

		public override void	OnLeave()
		{
			base.OnLeave();

			this.console.BeforeGUIHeaderRightMenu -= this.GUIExport;
		}

		private void	Update()
		{
			bool	mustUpdate = false;

			while (this.delayDelete.Count > 0)
			{
				this.DeleteNote(this.delayDelete.Pop());
				mustUpdate = true;
			}

			if (mustUpdate == true)
				this.UpdateName(null);

			if (this.currentVars != null)
			{
				int	restoreFolder = this.currentVars.workingFolder;

				for (int i = 0; i < this.folders.Count; i++)
				{
					while (this.folders[i].delayAdd.Count > 0)
					{
						LogNote	note = this.folders[i].delayAdd.Pop();

						this.currentVars.workingFolder = i;

						this.folders[i].notes.Add(note);
						this.folders[i].rowsDrawer.Add(this.folders[i].notes.Count - 1);
					}
				}

				this.currentVars.workingFolder = restoreFolder;
			}
		}

		private void	DeleteNote(LogNote log)
		{
			for (int i = 0; i < this.folders.Count; i++)
			{
				int	index = this.folders[i].notes.IndexOf(log);

				if (index != -1)
				{
					// When RowsDrawer deletes a row, we just need to update notes.
					if (this.folders[i].rowsDrawer.Count >= this.folders[i].notes.Count)
					{
						int	restoreFolder = this.currentVars.workingFolder;
						this.currentVars.workingFolder = i;
						this.folders[i].rowsDrawer.RemoveAt(this.folders[i].rowsDrawer.Count - 1);

						this.currentVars.workingFolder = restoreFolder;

						foreach (RowsDrawer.Vars vars in this.folders[i].rowsDrawer.perWindowVars.Each())
							vars.ClearSelection();
					}
					else
					{
						for (int j = 0; j < this.folders[i].rowsDrawer.Count; j++)
							this.folders[i].rowsDrawer[j] = j;
					}

					this.folders[i].notes.RemoveAt(index);

					int	extra = this.folders[i].notes.Count - this.folders[i].rowsDrawer.Count;

					// Clean fallback, in case of error somewhere.
					if (extra > 0)
						this.folders[i].notes.RemoveRange(this.folders[i].notes.Count - extra, extra);
				}
			}
		}

		private void	InitFolder(Folder folder)
		{
			folder.Init(this.console, this);

			folder.rowsDrawer.LogContextMenu += this.LogContextMenu;
			folder.rowsDrawer.RowHovered += this.ShowNote;
			folder.rowsDrawer.RowDeleted += this.ManualDeletedLog;
			folder.rowsDrawer.RowClicked += this.StartDrag;
		}

		private void	UninitFolder(Folder folder)
		{
			folder.Uninit();

			folder.rowsDrawer.LogContextMenu -= this.LogContextMenu;
			folder.rowsDrawer.RowHovered -= this.ShowNote;
			folder.rowsDrawer.RowDeleted -= this.ManualDeletedLog;
			folder.rowsDrawer.RowClicked -= this.StartDrag;
		}

		private void	DeleteFolder(object data)
		{
			int	i = (int)data;

			this.UninitFolder(this.folders[i]);

			this.folders.RemoveAt(i);

			foreach (Vars vars in this.perWindowVars.Each())
				vars.workingFolder = Mathf.Clamp(vars.workingFolder, 0, this.folders.Count - 1);

			this.UpdateName(null);
		}

		private void	ChangeStreamName(object data)
		{
			PromptWindow.Start(this.folders[(int)data].name, this.RenameStream, data);
		}

		private void	ManualDeletedLog(Row row)
		{
			LogNote	log = this.GetLogFromRow(row);

			if (log != null)
				this.delayDelete.Push(log);
		}

		private void	UpdateName(Row row)
		{
			int	count = this.folders.Sum((f) => f.rowsDrawer.Count + f.delayAdd.Count) - this.delayDelete.Count;

			if (count == 0)
				this.name = "Archive";
			else
				this.name = "Archive (" + count + ")";
		}

		private void	RenameStream(object data, string newName)
		{
			if (string.IsNullOrEmpty(newName) == false)
				this.folders[(int)data].name = newName;
		}

		private LogNote	GetLogFromRow(Row row)
		{
			for (int i = 0; i < this.folders.Count; i++)
			{
				LogNote	log = this.folders[i].notes.Find(ln => ln.row == row);

				if (log != null)
					return log;
			}

			return null;
		}

		private void	ArchiveFromContextMenu(GenericMenu menu, Row row)
		{
			LogNote	log = this.GetLogFromRow(row);

			menu.AddItem(new GUIContent("Archive log"), log != null, this.ToggleRowFromArchive, row);
		}

		private void	ToggleRowFromArchive(object data)
		{
			Row		row = data as Row;
			LogNote	log = this.GetLogFromRow(row);

			if (log == null)
				this.folders[0].delayAdd.Push(new LogNote() { row = row });
			else
				this.delayDelete.Push(log);

			this.UpdateName(null);
		}

		private void	ShowNote(Rect r, Row row)
		{
			if (Event.current.type == EventType.MouseDrag)
			{
				if (DragAndDrop.GetGenericData("n") != null)
				{
					// Start the actual drag
					DragAndDrop.StartDrag("Dragging Row");

					// Make sure no one uses the event after us
					Event.current.Use();
				}
			}
			else if (Event.current.type == EventType.MouseMove)
			{
				DragAndDrop.PrepareStartDrag();

				this.viewingNote = null;

				for (int i = 0; i < this.folders[this.currentVars.workingFolder].notes.Count; i++)
				{
					if (this.folders[this.currentVars.workingFolder].notes[i].row == row)
					{
						if (string.IsNullOrEmpty(this.folders[this.currentVars.workingFolder].notes[i].note) == false)
						{
							this.noteRect = r;
							this.noteRect.x += this.folders[this.currentVars.workingFolder].rowsDrawer.bodyRect.x;
							this.noteRect.y += this.folders[this.currentVars.workingFolder].rowsDrawer.bodyRect.y;
							this.viewingNote = this.folders[this.currentVars.workingFolder].notes[i];
						}

						break;
					}
				}
			}
		}

		private void	LogContextMenu(GenericMenu menu, Row row)
		{
			menu.AddSeparator("");
			Utility.content.text = LC.G("ArchiveModule_SetNote");
			menu.AddItem(Utility.content, false, this.PrepareSetNote, row);
		}

		private void	PrepareSetNote(object data)
		{
			Vars	closedVars = this.currentVars;

			Action<object, string>	SetNote = delegate(object data2, string content)
			{
				if (data2 is LogNote)
					((LogNote)data2).note = content;
				// Add a new note.
				else if (data2 is Row)
				{
					LogNote	note = new LogNote() {
						row = (Row)data2,
						note = content
					};
					this.folders[closedVars.workingFolder].notes.Add(note);
				}
			};

			for (int i = 0; i < this.folders[this.currentVars.workingFolder].notes.Count; i++)
			{
				if (this.folders[this.currentVars.workingFolder].notes[i].row == data)
				{
					PromptWindow.Start(this.folders[this.currentVars.workingFolder].notes[i].note, SetNote, this.folders[this.currentVars.workingFolder].notes[i]);
					return;
				}
			}

			PromptWindow.Start(string.Empty, SetNote, data);
		}

		private void	GUIExport()
		{
			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			if (vars != null && // Happens during the first frame, when OnGUI has not been called yet.
				this.folders[vars.workingFolder].rowsDrawer.Count > 0 &&
				GUILayout.Button(LC.G("ArchiveModule_ExportArchives"), Preferences.Settings.general.menuButtonStyle) == true)
			{
				List<Row>	rows = new List<Row>();
				Vars		closedVars = vars;

				for (int i = 0; i < this.folders[vars.workingFolder].rowsDrawer.Count; i++)
					rows.Add(this.console.rows[this.folders[vars.workingFolder].rowsDrawer[i]]);

				Action<ILogExporter, Row>	ExportLogNote = delegate(ILogExporter exporter, Row row)
				{
					foreach (var n in this.folders[closedVars.workingFolder].notes)
					{
						if (n.row == row)
						{
							exporter.AddColumn("note", n.note, null);
							break;
						}
					}
				};

				ExportRowsEditorWindow.Export(rows, ExportLogNote);
			}
		}

		private void	StartDrag(Rect r, Row row)
		{
			DragAndDrop.PrepareStartDrag();

			for (int i = 0; i < this.folders[this.currentVars.workingFolder].notes.Count; i++)
			{
				if (this.folders[this.currentVars.workingFolder].notes[i].row == row)
				{
					DragAndDrop.SetGenericData("n", this.folders[this.currentVars.workingFolder].notes[i]);
					break;
				}
			}
		}

		Row	IRows.GetRow(int i)
		{
			return this.folders[this.currentVars.workingFolder].notes[i].row;
		}

		int	IRows.CountRows()
		{
			return this.folders[this.currentVars.workingFolder].notes.Count;
		}
	}
}