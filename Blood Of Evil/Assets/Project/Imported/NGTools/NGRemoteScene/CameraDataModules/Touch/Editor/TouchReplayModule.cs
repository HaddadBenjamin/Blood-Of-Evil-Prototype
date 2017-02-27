using NGTools;
using NGTools.NGRemoteScene;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class TouchReplayModule : ReplayDataModule
	{
		private Vector2	mouseScrollPosition;

		public	TouchReplayModule() : base(TouchModule.ModuleID, TouchModule.Priority, TouchModule.Name)
		{
		}

		public	TouchReplayModule(TouchModuleEditor module) : base(TouchModule.ModuleID, TouchModule.Priority, TouchModule.Name)
		{
			this.data.AddRange(module.data);
		}

		public override void	OnGUIReplay(Rect r)
		{
			float							w = r.width;
			TouchModuleEditor.TouchesState	state = this.data[this.index] as TouchModuleEditor.TouchesState;

			for (int i = 0; i < state.touches.Length; i++)
			{
				r.x += state.touches[i].x;
				r.width = 2F;
				r.y += state.touches[i].y - 5F;
				r.height = 11F;
				EditorGUI.DrawRect(r, Color.black);

				r.x -= 5F;
				r.width = 11F;
				r.y += 5F;
				r.height = 2F;
				EditorGUI.DrawRect(r, Color.black);

				r.x += 5F;
				r.y += 5F;
				r.width = w;
				r.height = 16F;
				EditorGUI.LabelField(r, state.touches[i].fingerID.ToString());

				r.x -= state.touches[i].x;
				r.y -= state.touches[i].y - 5F;
			}
		}

		public override void	Export(ByteBuffer writer)
		{
			writer.Append(this.data.Count);
			foreach (TouchModuleEditor.TouchesState state in this.data)
			{
				writer.Append(state.time);
				writer.Append(state.touches.Length);
				for (int i = 0; i < state.touches.Length; i++)
				{
					writer.Append(state.touches[i].x);
					writer.Append(state.touches[i].y);
					writer.Append(state.touches[i].fingerID);
				}
			}
		}

		public override void	Import(Replay replay, ByteBuffer reader)
		{
			int	count = reader.ReadInt32();

			this.data.Clear();
			this.data.Capacity = count;

			for (int i = 0; i < count; i++)
			{
				TouchModuleEditor.TouchesState	state = new TouchModuleEditor.TouchesState();
				state.time = reader.ReadSingle();

				int	touchesCount = reader.ReadInt32();
				state.touches = new TouchModuleEditor.TouchesState.Touch[touchesCount];

				for (int j = 0; j < touchesCount; j++)
				{
					state.touches[j].x = reader.ReadSingle();
					state.touches[j].y = reader.ReadSingle();
					state.touches[j].fingerID = reader.ReadInt32();
				}

				this.data.Add(state);
			}
		}

		public override void	OnGUIDBG()
		{
			GUILayout.Label("Touch");
			this.mouseScrollPosition = GUILayout.BeginScrollView(this.mouseScrollPosition);
			{
				foreach (TouchModuleEditor.TouchesState state in this.data)
				{
					GUILayout.Label(state.time.ToString());
					for (int i = 0; i < state.touches.Length; i++)
						GUILayout.Label(" " + state.touches[i].x + " " + state.touches[i].y + " " + state.touches[i].fingerID);
				}
			}
			GUILayout.EndScrollView();
		}
	}
}