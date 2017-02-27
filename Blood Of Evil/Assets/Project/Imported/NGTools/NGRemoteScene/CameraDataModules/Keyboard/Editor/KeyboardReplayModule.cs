using NGTools;
using NGTools.NGRemoteScene;
using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor.NGRemoteScene
{
	internal sealed class KeyboardReplayModule : ReplayDataModule
	{
		public static Color	BackgroundColor = Color.black * .75F;

		private Vector2	keyScrollPosition;

		public	KeyboardReplayModule() : base(KeyboardModule.ModuleID, KeyboardModule.Priority, KeyboardModule.Name)
		{
		}

		public	KeyboardReplayModule(KeyboardModuleEditor module) : base(KeyboardModule.ModuleID, KeyboardModule.Priority, KeyboardModule.Name)
		{
			this.data.AddRange(module.data);
		}

		public override void	OnGUIReplay(Rect r)
		{
			if (this.index == -1)
				return;

			KeyboardModuleEditor.KeyInput	data = this.data[this.index] as KeyboardModuleEditor.KeyInput;

			if (data.keys.Length == 0)
				return;

			r.x = 0F;
			r.width = 100F;
			r.height = data.keys.Length * 16F;
			EditorGUI.DrawRect(r, Color.grey);

			r.height = 16F;

			for (int i = 0; i < data.keys.Length; i++)
			{
				EditorGUI.DrawRect(r, KeyboardReplayModule.BackgroundColor);
				EditorGUI.LabelField(r, data.keys[i].ToString());
				r.y += r.height;
			}
		}

		public override void	Export(ByteBuffer writer)
		{
			writer.Append(this.data.Count);

			foreach (KeyboardModuleEditor.KeyInput input in this.data)
			{
				writer.Append(input.time);
				writer.Append((UInt16)input.keys.Length);
				for (int i = 0; i < input.keys.Length; i++)
					writer.Append((UInt16)input.keys[i]);
			}
		}

		public override void	Import(Replay settings, ByteBuffer reader)
		{
			int	count = reader.ReadInt32();

			this.data.Clear();
			this.data.Capacity = count;

			for (int i = 0; i < count; i++)
			{
				KeyboardModuleEditor.KeyInput	input = new KeyboardModuleEditor.KeyInput();
				input.time = reader.ReadSingle();
				input.keys = new KeyCode[(UInt16)reader.ReadUInt16()];
				for (int j = 0; j < input.keys.Length; j++)
					input.keys[j] = (KeyCode)reader.ReadUInt16();
				this.data.Add(input);
			}
		}

		public override void	OnGUIDBG()
		{
			GUILayout.Label("Key");
			this.keyScrollPosition = GUILayout.BeginScrollView(this.keyScrollPosition);
			{
				foreach (KeyboardModuleEditor.KeyInput data in this.data)
				{
					StringBuilder	b = Utility.GetBuffer();
					for (int j = 0; j < data.keys.Length; j++)
					{
						b.Append(data.keys[j]);
						b.Append(',');
					}
					GUILayout.Label(data.time + " " + Utility.ReturnBuffer(b));
				}
			}
			GUILayout.EndScrollView();
		}
	}
}