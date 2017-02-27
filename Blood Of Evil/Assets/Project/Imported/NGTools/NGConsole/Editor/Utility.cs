using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace NGToolsEditor
{
	using NGConsole;

	public static partial class	Utility
	{
		internal static FastFileCache	files = new FastFileCache();
		internal static FastClassCache	classes = new FastClassCache();

		public static void	OpenModuleInWindow(NGConsoleWindow console, Module module, bool focus = false)
		{
			ModuleWindow	moduleWindow = EditorWindow.CreateInstance<ModuleWindow>();
			moduleWindow.Init(console, module);

			Type	ContainerWindow = typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
			Type	View = typeof(EditorWindow).Assembly.GetType("UnityEditor.View");
			Type	DockArea = typeof(EditorWindow).Assembly.GetType("UnityEditor.DockArea");

			if (ContainerWindow != null && View != null && DockArea != null)
			{
				PropertyInfo	windows = ContainerWindow.GetProperty("windows", BindingFlags.Static | BindingFlags.Public);
				PropertyInfo	mainView = ContainerWindow.GetProperty("mainView", BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo	allChildren = View.GetProperty("allChildren", BindingFlags.Instance | BindingFlags.Public);
				FieldInfo		m_Panes = DockArea.GetField("m_Panes", BindingFlags.Instance | BindingFlags.NonPublic);
				MethodInfo		AddTab = DockArea.GetMethod("AddTab", new Type[] { typeof(EditorWindow) });

				if (windows != null && mainView != null && allChildren != null && m_Panes != null && AddTab != null)
				{
					Array	array = windows.GetValue(null, null) as Array;

					foreach (var w in array)
					{
						object	view = mainView.GetValue(w, null);
						Array	children = allChildren.GetValue(view, null) as Array;

						foreach (var c in children)
						{
							try
							{
								var	panes = m_Panes.GetValue(c);

								List<EditorWindow>	panesCasted = panes as List<EditorWindow>;
								if (panesCasted.Any((EditorWindow pane) => pane.GetType() == console.GetType()))
								{
									AddTab.Invoke(c, new object[] { moduleWindow });
									moduleWindow.Show();
									if (focus == false)
										console.Focus();
									return;
								}
							}
							catch
							{
							}
						}
					}
				}
			}

			moduleWindow.Show();
			if (focus == false)
				console.Focus();
		}

		/// <summary>
		/// Colors keywords given by StackSettings.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public static string	ColorLine(string line)
		{
			StringBuilder	buffer = Utility.GetBuffer();

			buffer.Append(' ');
			buffer.AppendStartColor(Preferences.Settings.stackTrace.previewTextColor);
			for (int i = 0; i < line.Length; i++)
			{
				int	previousKeywordColor = -1;

				// Convert tab to spaces.
				if (line[i] == '	' &&
					Preferences.Settings.stackTrace.displayTabAsSpaces > 0)
				{
					buffer.Append(' ', Preferences.Settings.stackTrace.displayTabAsSpaces);
					continue;
				}
				// Color only visible char.
				else if (line[i] != ' ')
				{
					for (int j = 0; j < Preferences.Settings.stackTrace.keywords.Length; ++j)
					{
						for (int k = 0; k < Preferences.Settings.stackTrace.keywords[j].keywords.Length; k++)
						{
							if (Utility.Compare(line, Preferences.Settings.stackTrace.keywords[j].keywords[k], i) == true)
							{
								// Save some color tags.
								if (previousKeywordColor != -1 &&
									Preferences.Settings.stackTrace.keywords[previousKeywordColor].color != Preferences.Settings.stackTrace.keywords[j].color)
								{
									previousKeywordColor = j;
									buffer.AppendEndColor();
									buffer.AppendStartColor(Preferences.Settings.stackTrace.keywords[j].color);
								}
								else if (previousKeywordColor == -1)
								{
									previousKeywordColor = j;
									buffer.AppendStartColor(Preferences.Settings.stackTrace.keywords[j].color);
								}

								buffer.Append(Preferences.Settings.stackTrace.keywords[j].keywords[k]);
								i += Preferences.Settings.stackTrace.keywords[j].keywords[k].Length - 1;
								goto skip;
							}
						}
					}
				}

				buffer.Append(line[i]);
				skip:

				if (previousKeywordColor != -1)
					buffer.AppendEndColor();

				continue;
			}

			buffer.AppendEndColor();

			return Utility.ReturnBuffer(buffer);
		}
	}
}