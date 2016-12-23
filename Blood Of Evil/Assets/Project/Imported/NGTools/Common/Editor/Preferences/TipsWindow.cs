using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NGToolsEditor
{
	public class TipsWindow : EditorWindow
	{
		private static List<string>	tipLCKeys = new List<string>();

		private Vector2	scrollPosition;

		static	TipsWindow()
		{
			// Default tips.
			TipsWindow.tipLCKeys.Add("Console");
			TipsWindow.tipLCKeys.Add("Module");
			TipsWindow.tipLCKeys.Add("Stream");
			TipsWindow.tipLCKeys.Add("Filter");
			TipsWindow.tipLCKeys.Add("Log");
			TipsWindow.tipLCKeys.Add("Translation");
			TipsWindow.tipLCKeys.Add("Tip");
		}

		/// <summary>
		/// <para>Adds new tip on board!</para>
		/// </summary>
		/// <param name="localizationKey">A localization key, make sure you translations Tip_{yourkey}_Title and Tip_{yourkey}_Content are provided.</param>
		public static void	AddTip(string localizationKey)
		{
			TipsWindow.tipLCKeys.Add(localizationKey);
		}

		public static void	RemoveTip(string localizationKey)
		{
			TipsWindow.tipLCKeys.Remove(localizationKey);
		}

		protected virtual void	OnGUI()
		{
			this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
			{
				GUILayout.Label(LC.G("Tips_Title"), GeneralStyles.MainTitle);
				GUILayout.Label(LC.G("Tips_Introduction"), GeneralStyles.InnerBoxText);

				for (int i = 0; i < TipsWindow.tipLCKeys.Count; i++)
				{
					GUILayout.Label((i + 1).ToString() + " - " + LC.G("Tip_" + TipsWindow.tipLCKeys[i] + "_Title"), "HeaderLabel");
					GUILayout.Label(LC.G("Tip_" + TipsWindow.tipLCKeys[i] + "_Content"), GeneralStyles.InnerBoxText);
				}
			}
			EditorGUILayout.EndScrollView();
		}
	}
}