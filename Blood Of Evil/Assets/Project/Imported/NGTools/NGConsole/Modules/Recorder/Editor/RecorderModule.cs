using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGConsole
{
	[Serializable]
	//[VisibleModule(100)]
	internal sealed class RecorderModule : Module, IStreams
	{
		[Serializable]
		private sealed class Vars
		{
			public int	workingStream;
		}

		public List<StreamLog>	Streams { get { return new List<StreamLog>(this.streams.ToArray()); } }
		public int				WorkingStream { get { return this.perWindowVars.Get(Utility.drawingWindow).workingStream; } }

		[Exportable(ExportableAttribute.ArrayOptions.Immutable)]
		private List<SampleStream>	streams;

		[SerializeField]
		private PerWindowVars<Vars>	perWindowVars;

		[NonSerialized]
		private Vars	currentVars;

		private GUIStyle	requirePlayModeStyle;

		public	RecorderModule()
		{
			this.name = "Recorder";
			this.streams = new List<SampleStream>();
			this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnEnable(NGConsoleWindow console, int id)
		{
			base.OnEnable(console, id);

			foreach (var stream in this.streams)
				stream.Init(this.console, this);

			this.console.ConsoleCleared += this.Clear;
			this.console.wantsMouseMove = true;

			// Populate with default commands if missing.
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchNextStreamCommand, KeyCode.Tab, true);
			Preferences.Settings.inputsManager.AddCommand("Navigation", Constants.SwitchPreviousStreamCommand, KeyCode.Tab, true, true);

			if (this.perWindowVars == null)
				this.perWindowVars = new PerWindowVars<Vars>();
		}

		public override void	OnDisable()
		{
			this.console.ConsoleCleared -= this.Clear;
			this.console.wantsMouseMove = false;

			foreach (var stream in this.streams)
				stream.Uninit();
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
			this.console.RemoveNotification();
		}

		public override void	OnGUI(Rect r)
		{
			this.currentVars = this.perWindowVars.Get(Utility.drawingWindow);

			float	yOrigin = r.y;
			float	maxHeight = r.height;

			if (EditorApplication.isPlaying == false && this.streams.Count > 0)
			{
				if (this.requirePlayModeStyle == null)
				{
					this.requirePlayModeStyle = new GUIStyle(GUI.skin.label);
					this.requirePlayModeStyle.alignment = TextAnchor.MiddleRight;
					this.requirePlayModeStyle.normal.textColor = Color.green;
				}

				r.height = EditorGUIUtility.singleLineHeight;
				GUI.Label(r, LC.G("SampleStream_RequirePlayMode"), this.requirePlayModeStyle);
			}

			r.x = 0F;
			r.width = this.console.position.width;
			r = this.DrawSampleTabs(r);

			r.x = 0F;
			r.width = this.console.position.width;
			r.height = maxHeight - (r.y - yOrigin);

			this.currentVars.workingStream = Mathf.Clamp(this.currentVars.workingStream, 0, this.streams.Count - 1);
			if (0 <= this.currentVars.workingStream && this.currentVars.workingStream < this.streams.Count)
				this.streams[this.currentVars.workingStream].OnGUI(r);
			else
				GUI.Label(r, LC.G("RecorderModule_NoSampleCreated"), GeneralStyles.CenterText);
		}

		public void	Clear()
		{
			foreach (var stream in this.streams)
				stream.Clear();
		}

		public void	FocusStream(int i)
		{
			if (i < 0)
				this.currentVars.workingStream = 0;
			else if (i >= this.streams.Count)
				this.currentVars.workingStream = this.streams.Count - 1;
			else
				this.currentVars.workingStream = i;
		}

		public void	DeleteStream(int i)
		{
			this.streams[i].Uninit();
			this.streams.RemoveAt(i);

			foreach (Vars var in this.perWindowVars.Each())
				var.workingStream = Mathf.Clamp(var.workingStream, 0, this.streams.Count - 1);
		}

		private Rect	DrawSampleTabs(Rect r)
		{
			r.height = EditorGUIUtility.singleLineHeight;

			// Switch stream
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchNextStreamCommand) == true)
			{
				this.currentVars.workingStream += 1;
				if (this.currentVars.workingStream >= this.streams.Count)
					this.currentVars.workingStream = 0;

				Event.current.Use();
			}
			if (Preferences.Settings.inputsManager.Check("Navigation", Constants.SwitchPreviousStreamCommand) == true)
			{
				this.currentVars.workingStream -= 1;
				if (this.currentVars.workingStream < 0)
					this.currentVars.workingStream = this.streams.Count - 1;

				Event.current.Use();
			}

			GUILayout.BeginArea(r);
			{
				GUILayout.BeginHorizontal();
				{
					for (int i = 0; i < this.streams.Count; i++)
						this.streams[i].OnTabGUI(i);

					if (GUILayout.Button("+", Preferences.Settings.general.menuButtonStyle) == true)
					{
						SampleStream	stream = new SampleStream();

						stream.Init(this.console, this);
						stream.RefreshFilteredRows();
						this.streams.Add(stream);

						if (this.streams.Count == 1)
							this.currentVars.workingStream = 0;
					}

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			r.y += r.height + 2F;

			return r;
		}

		private void	RenameStream(object data, string newName)
		{
			if (string.IsNullOrEmpty(newName) == false)
				this.streams[(int)data].name = newName;
		}

		private void	GUIExport()
		{
			Vars	vars = this.perWindowVars.Get(Utility.drawingWindow);

			if (vars != null && // Happens during the first frame, when OnGUI has not been called yet.
				vars.workingStream >= 0 &&
				vars.workingStream < this.streams.Count &&
				this.streams[vars.workingStream].rowsDrawer.Count > 0 &&
				GUILayout.Button(LC.G("RecorderModule_ExportSamples"), Preferences.Settings.general.menuButtonStyle) == true)
			{
				List<Row>	rows = new List<Row>();

				for (int i = 0; i < this.streams[vars.workingStream].rowsDrawer.Count; i++)
					rows.Add(this.console.rows[this.streams[vars.workingStream].rowsDrawer[i]]);

				ExportRowsEditorWindow.Export(rows);
			}
		}
	}
}