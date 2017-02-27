using NGTools;
using NGTools.NGRemoteScene;
using System;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class MouseReplayModule : ReplayDataModule
	{
		public static Color	BackgroundColor = Color.black * .75F;

		private Vector2	mouseScrollPosition;

		public	MouseReplayModule() : base(MouseModule.ModuleID, MouseModule.Priority, MouseModule.Name)
		{
		}

		public	MouseReplayModule(MouseModuleEditor module) : base(MouseModule.ModuleID, MouseModule.Priority, MouseModule.Name)
		{
			this.data.AddRange(module.data);
		}

		public override void	OnGUIReplay(Rect r)
		{
			if (this.index == -1)
				return;

			MouseModuleEditor.MouseInput	input = this.data[this.index] as MouseModuleEditor.MouseInput;

			r.x += input.x;
			r.width = 1F;
			r.y += input.y - 5F;
			r.height = 11F;
			EditorGUI.DrawRect(r, Color.black);

			r.x -= 5F;
			r.width = 11F;
			r.y += 5F;
			r.height = 1F;
			EditorGUI.DrawRect(r, Color.black);

			if (input.buttons != 0)
			{
				r.x += 5F;
				r.y += 5F;
				r.width = 60F;
				r.height = 16F;

				for (int i = 0, n = (int)input.buttons; i < 32; i++)
				{
					if ((n & (1 << i)) != 0)
					{
						EditorGUI.DrawRect(r, MouseReplayModule.BackgroundColor);
						EditorGUI.LabelField(r, "Button " + i);
						r.y += r.height;
					}
				}
			}
		}

		public override void	Export(ByteBuffer writer)
		{
			writer.Append(this.data.Count);
			foreach (MouseModuleEditor.MouseInput input in this.data)
			{
				writer.Append(input.time);
				writer.Append(input.x);
				writer.Append(input.y);
				writer.Append((Int32)input.buttons);
			}
		}

		public override void	Import(Replay replay, ByteBuffer reader)
		{
			int	count = reader.ReadInt32();

			this.data.Clear();
			this.data.Capacity = count;

			for (int i = 0; i < count; i++)
			{
				MouseModuleEditor.MouseInput	input = new MouseModuleEditor.MouseInput();
				input.time = reader.ReadSingle();
				input.x = reader.ReadSingle();
				input.y = reader.ReadSingle();
				input.buttons = (MouseButtons)reader.ReadInt32();
				this.data.Add(input);
			}
		}

		public override void	OnGUIDBG()
		{
			GUILayout.Label("Mouse");
			this.mouseScrollPosition = GUILayout.BeginScrollView(this.mouseScrollPosition);
			{
				foreach (MouseModuleEditor.MouseInput input in this.data)
				{
					GUILayout.Label(input.time + "	" + input.x + " " + input.y + " " + input.buttons);
				}
			}
			GUILayout.EndScrollView();
		}
	}
}