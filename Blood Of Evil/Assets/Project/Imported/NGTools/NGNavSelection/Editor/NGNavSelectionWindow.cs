#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define UNITY_4
#endif

using NGToolsEditor.NGFav;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;

namespace NGToolsEditor.NGNavSelection
{
	using UnityEngine;

	[InitializeOnLoad]
	public class NGNavSelectionWindow : EditorWindow, IHasCustomMenu
	{
		public enum WindowsVirtualKey
		{
			NONE = 0x00,

			[Description("Left mouse button")]
			VK_LBUTTON = 0x01,

			[Description("Right mouse button")]
			VK_RBUTTON = 0x02,

			[Description("Control-break processing")]
			VK_CANCEL = 0x03,

			[Description("Middle mouse button (three-button mouse)")]
			VK_MBUTTON = 0x04,

			[Description("X1 mouse button")]
			VK_XBUTTON1 = 0x05,

			[Description("X2 mouse button")]
			VK_XBUTTON2 = 0x06,

			[Description("BACKSPACE key")]
			VK_BACK = 0x08,

			[Description("TAB key")]
			VK_TAB = 0x09,

			[Description("CLEAR key")]
			VK_CLEAR = 0x0C,

			[Description("ENTER key")]
			VK_RETURN = 0x0D,

			[Description("SHIFT key")]
			VK_SHIFT = 0x10,

			[Description("CTRL key")]
			VK_CONTROL = 0x11,

			[Description("ALT key")]
			VK_MENU = 0x12,

			[Description("PAUSE key")]
			VK_PAUSE = 0x13,

			[Description("CAPS LOCK key")]
			VK_CAPITAL = 0x14,

			[Description("IME Kana mode")]
			VK_KANA = 0x15,

			[Description("IME Hanguel mode (maintained for compatibility; use VK_HANGUL)")]
			VK_HANGUEL = 0x15,

			[Description("IME Hangul mode")]
			VK_HANGUL = 0x15,

			[Description("IME Junja mode")]
			VK_JUNJA = 0x17,

			[Description("IME final mode")]
			VK_FINAL = 0x18,

			[Description("IME Hanja mode")]
			VK_HANJA = 0x19,

			[Description("IME Kanji mode")]
			VK_KANJI = 0x19,

			[Description("ESC key")]
			VK_ESCAPE = 0x1B,

			[Description("IME convert")]
			VK_CONVERT = 0x1C,

			[Description("IME nonconvert")]
			VK_NONCONVERT = 0x1D,

			[Description("IME accept")]
			VK_ACCEPT = 0x1E,

			[Description("IME mode change request")]
			VK_MODECHANGE = 0x1F,

			[Description("SPACEBAR")]
			VK_SPACE = 0x20,

			[Description("PAGE UP key")]
			VK_PRIOR = 0x21,

			[Description("PAGE DOWN key")]
			VK_NEXT = 0x22,

			[Description("END key")]
			VK_END = 0x23,

			[Description("HOME key")]
			VK_HOME = 0x24,

			[Description("LEFT ARROW key")]
			VK_LEFT = 0x25,

			[Description("UP ARROW key")]
			VK_UP = 0x26,

			[Description("RIGHT ARROW key")]
			VK_RIGHT = 0x27,

			[Description("DOWN ARROW key")]
			VK_DOWN = 0x28,

			[Description("SELECT key")]
			VK_SELECT = 0x29,

			[Description("PRINT key")]
			VK_PRINT = 0x2A,

			[Description("EXECUTE key")]
			VK_EXECUTE = 0x2B,

			[Description("PRINT SCREEN key")]
			VK_SNAPSHOT = 0x2C,

			[Description("INS key")]
			VK_INSERT = 0x2D,

			[Description("DEL key")]
			VK_DELETE = 0x2E,

			[Description("HELP key")]
			VK_HELP = 0x2F,

			[Description("0 key")]
			K_0 = 0x30,

			[Description("1 key")]
			K_1 = 0x31,

			[Description("2 key")]
			K_2 = 0x32,

			[Description("3 key")]
			K_3 = 0x33,

			[Description("4 key")]
			K_4 = 0x34,

			[Description("5 key")]
			K_5 = 0x35,

			[Description("6 key")]
			K_6 = 0x36,

			[Description("7 key")]
			K_7 = 0x37,

			[Description("8 key")]
			K_8 = 0x38,

			[Description("9 key")]
			K_9 = 0x39,

			[Description("A key")]
			K_A = 0x41,

			[Description("B key")]
			K_B = 0x42,

			[Description("C key")]
			K_C = 0x43,

			[Description("D key")]
			K_D = 0x44,

			[Description("E key")]
			K_E = 0x45,

			[Description("F key")]
			K_F = 0x46,

			[Description("G key")]
			K_G = 0x47,

			[Description("H key")]
			K_H = 0x48,

			[Description("I key")]
			K_I = 0x49,

			[Description("J key")]
			K_J = 0x4A,

			[Description("K key")]
			K_K = 0x4B,

			[Description("L key")]
			K_L = 0x4C,

			[Description("M key")]
			K_M = 0x4D,

			[Description("N key")]
			K_N = 0x4E,

			[Description("O key")]
			K_O = 0x4F,

			[Description("P key")]
			K_P = 0x50,

			[Description("Q key")]
			K_Q = 0x51,

			[Description("R key")]
			K_R = 0x52,

			[Description("S key")]
			K_S = 0x53,

			[Description("T key")]
			K_T = 0x54,

			[Description("U key")]
			K_U = 0x55,

			[Description("V key")]
			K_V = 0x56,

			[Description("W key")]
			K_W = 0x57,

			[Description("X key")]
			K_X = 0x58,

			[Description("Y key")]
			K_Y = 0x59,

			[Description("Z key")]
			K_Z = 0x5A,

			[Description("Left Windows key (Natural keyboard)")]
			VK_LWIN = 0x5B,

			[Description("Right Windows key (Natural keyboard)")]
			VK_RWIN = 0x5C,

			[Description("Applications key (Natural keyboard)")]
			VK_APPS = 0x5D,

			[Description("Computer Sleep key")]
			VK_SLEEP = 0x5F,

			[Description("Numeric keypad 0 key")]
			VK_NUMPAD0 = 0x60,

			[Description("Numeric keypad 1 key")]
			VK_NUMPAD1 = 0x61,

			[Description("Numeric keypad 2 key")]
			VK_NUMPAD2 = 0x62,

			[Description("Numeric keypad 3 key")]
			VK_NUMPAD3 = 0x63,

			[Description("Numeric keypad 4 key")]
			VK_NUMPAD4 = 0x64,

			[Description("Numeric keypad 5 key")]
			VK_NUMPAD5 = 0x65,

			[Description("Numeric keypad 6 key")]
			VK_NUMPAD6 = 0x66,

			[Description("Numeric keypad 7 key")]
			VK_NUMPAD7 = 0x67,

			[Description("Numeric keypad 8 key")]
			VK_NUMPAD8 = 0x68,

			[Description("Numeric keypad 9 key")]
			VK_NUMPAD9 = 0x69,

			[Description("Multiply key")]
			VK_MULTIPLY = 0x6A,

			[Description("Add key")]
			VK_ADD = 0x6B,

			[Description("Separator key")]
			VK_SEPARATOR = 0x6C,

			[Description("Subtract key")]
			VK_SUBTRACT = 0x6D,

			[Description("Decimal key")]
			VK_DECIMAL = 0x6E,

			[Description("Divide key")]
			VK_DIVIDE = 0x6F,

			[Description("F1 key")]
			VK_F1 = 0x70,

			[Description("F2 key")]
			VK_F2 = 0x71,

			[Description("F3 key")]
			VK_F3 = 0x72,

			[Description("F4 key")]
			VK_F4 = 0x73,

			[Description("F5 key")]
			VK_F5 = 0x74,

			[Description("F6 key")]
			VK_F6 = 0x75,

			[Description("F7 key")]
			VK_F7 = 0x76,

			[Description("F8 key")]
			VK_F8 = 0x77,

			[Description("F9 key")]
			VK_F9 = 0x78,

			[Description("F10 key")]
			VK_F10 = 0x79,

			[Description("F11 key")]
			VK_F11 = 0x7A,

			[Description("F12 key")]
			VK_F12 = 0x7B,

			[Description("F13 key")]
			VK_F13 = 0x7C,

			[Description("F14 key")]
			VK_F14 = 0x7D,

			[Description("F15 key")]
			VK_F15 = 0x7E,

			[Description("F16 key")]
			VK_F16 = 0x7F,

			[Description("F17 key")]
			VK_F17 = 0x80,

			[Description("F18 key")]
			VK_F18 = 0x81,

			[Description("F19 key")]
			VK_F19 = 0x82,

			[Description("F20 key")]
			VK_F20 = 0x83,

			[Description("F21 key")]
			VK_F21 = 0x84,

			[Description("F22 key")]
			VK_F22 = 0x85,

			[Description("F23 key")]
			VK_F23 = 0x86,

			[Description("F24 key")]
			VK_F24 = 0x87,

			[Description("NUM LOCK key")]
			VK_NUMLOCK = 0x90,

			[Description("SCROLL LOCK key")]
			VK_SCROLL = 0x91,

			[Description("Left SHIFT key")]
			VK_LSHIFT = 0xA0,

			[Description("Right SHIFT key")]
			VK_RSHIFT = 0xA1,

			[Description("Left CONTROL key")]
			VK_LCONTROL = 0xA2,

			[Description("Right CONTROL key")]
			VK_RCONTROL = 0xA3,

			[Description("Left MENU key")]
			VK_LMENU = 0xA4,

			[Description("Right MENU key")]
			VK_RMENU = 0xA5,

			[Description("Browser Back key")]
			VK_BROWSER_BACK = 0xA6,

			[Description("Browser Forward key")]
			VK_BROWSER_FORWARD = 0xA7,

			[Description("Browser Refresh key")]
			VK_BROWSER_REFRESH = 0xA8,

			[Description("Browser Stop key")]
			VK_BROWSER_STOP = 0xA9,

			[Description("Browser Search key")]
			VK_BROWSER_SEARCH = 0xAA,

			[Description("Browser Favorites key")]
			VK_BROWSER_FAVORITES = 0xAB,

			[Description("Browser Start and Home key")]
			VK_BROWSER_HOME = 0xAC,

			[Description("Volume Mute key")]
			VK_VOLUME_MUTE = 0xAD,

			[Description("Volume Down key")]
			VK_VOLUME_DOWN = 0xAE,

			[Description("Volume Up key")]
			VK_VOLUME_UP = 0xAF,

			[Description("Next Track key")]
			VK_MEDIA_NEXT_TRACK = 0xB0,

			[Description("Previous Track key")]
			VK_MEDIA_PREV_TRACK = 0xB1,

			[Description("Stop Media key")]
			VK_MEDIA_STOP = 0xB2,

			[Description("Play/Pause Media key")]
			VK_MEDIA_PLAY_PAUSE = 0xB3,

			[Description("Start Mail key")]
			VK_LAUNCH_MAIL = 0xB4,

			[Description("Select Media key")]
			VK_LAUNCH_MEDIA_SELECT = 0xB5,

			[Description("Start Application 1 key")]
			VK_LAUNCH_APP1 = 0xB6,

			[Description("Start Application 2 key")]
			VK_LAUNCH_APP2 = 0xB7,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key")]
			VK_OEM_1 = 0xBA,

			[Description("For any country/region, the '+' key")]
			VK_OEM_PLUS = 0xBB,

			[Description("For any country/region, the ',' key")]
			VK_OEM_COMMA = 0xBC,

			[Description("For any country/region, the '-' key")]
			VK_OEM_MINUS = 0xBD,

			[Description("For any country/region, the '.' key")]
			VK_OEM_PERIOD = 0xBE,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key")]
			VK_OEM_2 = 0xBF,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key")]
			VK_OEM_3 = 0xC0,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key")]
			VK_OEM_4 = 0xDB,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key")]
			VK_OEM_5 = 0xDC,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key")]
			VK_OEM_6 = 0xDD,

			[Description("Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key")]
			VK_OEM_7 = 0xDE,

			[Description("Used for miscellaneous characters; it can vary by keyboard.")]
			VK_OEM_8 = 0xDF,


			[Description("Either the angle bracket key or the backslash key on the RT 102-key keyboard")]
			VK_OEM_102 = 0xE2,

			[Description("IME PROCESS key")]
			VK_PROCESSKEY = 0xE5,


			[Description("Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP")]
			VK_PACKET = 0xE7,

			[Description("Attn key")]
			VK_ATTN = 0xF6,

			[Description("CrSel key")]
			VK_CRSEL = 0xF7,

			[Description("ExSel key")]
			VK_EXSEL = 0xF8,

			[Description("Erase EOF key")]
			VK_EREOF = 0xF9,

			[Description("Play key")]
			VK_PLAY = 0xFA,

			[Description("Zoom key")]
			VK_ZOOM = 0xFB,

			[Description("PA1 key")]
			VK_PA1 = 0xFD,

			[Description("Clear key")]
			VK_OEM_CLEAR = 0xFE,
		}

		public const string	Title = "ƝƓ Ɲav Ȿelection";
		public const string	LastHashPrefKey = "NGNavSelection_lastHash";
		public const string	AutoSavePrefKey = "NGNavSelection_historic";
		public const int	MaxHistoric = 1000;
		public const float	HighlightCursorWidth = 5F;
		public static Color	HighlightCursorBackgroundColor = new Color(.3F, .3F, .3F);
		public static Color	HighlightFocusedHistoricBackgroundColor = new Color(.7F, .7F, .1F);

		[DllImport("user32.dll")]
		internal static extern Int16	GetAsyncKeyState(Int32 virtualKeyCode);
		[DllImport("user32.dll")]
		internal static extern IntPtr	GetActiveWindow();

		// Does not handle null entries in historic.
		public static bool	CanSelectNext { get { return NGNavSelectionWindow.historicCursor != -1 && NGNavSelectionWindow.historic.Count > 0; } }
		public static bool	CanSelectPrevious { get { return (NGNavSelectionWindow.historicCursor == -1 || NGNavSelectionWindow.historicCursor > 0) && NGNavSelectionWindow.historic.Count > 0; } }

		public static event Action	SelectionChanged;

		private static List<AssetsSelection>	historic = new List<AssetsSelection>();
		private static int						historicCursor = -1;
		private static int						lastHash = 0;
		private static int						lastFocusedHistoric = -1;
		private static bool						editorState = false;
		private static bool						savedOnCompile = false;
		private static bool						buttonWasDown = false;

		private GUIListDrawer<AssetsSelection>	listDrawer;
		private Vector2		dragOriginPosition;
		private double		lastClick;

		private bool	isLocked;
		private Vector2	initialMin;
		private Vector2	initialMax;

		private ErrorPopup	errorPopup = new ErrorPopup("An error occured, try to reopen " + NGNavSelectionWindow.Title + ".");

		static	NGNavSelectionWindow()
		{
			Utility.AddMenuItemPicker(Constants.MenuItemPath + NGNavSelectionWindow.Title);

			try
			{
				if (Application.platform == RuntimePlatform.WindowsEditor)
					EditorApplication.update += NGNavSelectionWindow.HandleMouseInputs;
				NGEditorApplication.EditorExit += NGNavSelectionWindow.SaveHistoric;
				Preferences.SettingsChanged += NGNavSelectionWindow.Preferences_SettingsChanged;

				// It must me delayed! Most Object are correctly fetched, except folders and maybe others.
				EditorApplication.delayCall += () => {
					string	autoSave = NGEditorPrefs.GetString(NGNavSelectionWindow.AutoSavePrefKey, string.Empty, true);

					if (autoSave != string.Empty)
					{
						string[]	selections = autoSave.Split(',');
						int			lastHash = 0;

						for (int i = 0; i < selections.Length; i++)
						{
							string[]	IDs = selections[i].Split(';');
							int[]		array = new int[IDs.Length];

							for (int j = 0; j < IDs.Length; j++)
								array[j] = int.Parse(IDs[j]);

							AssetsSelection	selection = new AssetsSelection(array);

							if (selection.refs.Count > 0)
							{
								int	hash = selection.GetSelectionHash();

								if (lastHash != hash)
								{
									lastHash = hash;
									NGNavSelectionWindow.historic.Add(selection);
								}
							}
						}
					}

					NGNavSelectionWindow.editorState = EditorApplication.isPlaying;

					NGNavSelectionWindow.lastHash = NGEditorPrefs.GetInt(NGNavSelectionWindow.LastHashPrefKey, NGNavSelectionWindow.lastHash);
				};

				Marshal.PrelinkAll(typeof(NGNavSelectionWindow));
			}
			catch
			{
			}
		}

		[MenuItem(Constants.MenuItemPath + NGNavSelectionWindow.Title, priority = Constants.MenuItemPriority + 315)]
		public static void	Open()
		{
			EditorWindow.GetWindow<NGNavSelectionWindow>(NGNavSelectionWindow.Title);
		}

		private static void	Preferences_SettingsChanged()
		{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1
			EditorApplication.update -= NGNavSelectionWindow.UpdateSelection;
#else
			Selection.selectionChanged -= NGNavSelectionWindow.UpdateSelection;
#endif
			EditorApplication.playmodeStateChanged -= NGNavSelectionWindow.PlayStateChanged;

			if (Preferences.Settings != null)
			{
				NGSettingsWindow.AddSection(NGNavSelectionWindow.Title, NGNavSelectionWindow.OnGUISettings);
				if (Preferences.Settings.nav.enable == true)
				{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1
					EditorApplication.update += NGNavSelectionWindow.UpdateSelection;
#else
					Selection.selectionChanged += NGNavSelectionWindow.UpdateSelection;
#endif
					EditorApplication.playmodeStateChanged += NGNavSelectionWindow.PlayStateChanged;
				}
			}
			else
				NGSettingsWindow.RemoveSection(NGNavSelectionWindow.Title);
		}

		private static void	OnGUISettings()
		{
			if (Application.platform != RuntimePlatform.WindowsEditor)
				EditorGUILayout.LabelField(LC.G("NGNavSelection_OnlyAvailableOnWindows"), GeneralStyles.WrapLabel);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGNavSelection_EnableDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.nav.enable = EditorGUILayout.Toggle(LC.G("Enable"), Preferences.Settings.nav.enable);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Preferences.Settings.nav.enable == false)
				{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1
					EditorApplication.update -= NGNavSelectionWindow.UpdateSelection;
#else
					Selection.selectionChanged -= NGNavSelectionWindow.UpdateSelection;
#endif
					EditorApplication.playmodeStateChanged -= NGNavSelectionWindow.PlayStateChanged;
				}
				else
				{
#if UNITY_4 || UNITY_5_0 || UNITY_5_1
					EditorApplication.update += NGNavSelectionWindow.UpdateSelection;
#else
					Selection.selectionChanged += NGNavSelectionWindow.UpdateSelection;
#endif
					EditorApplication.playmodeStateChanged += NGNavSelectionWindow.PlayStateChanged;
				}
				Preferences.InvalidateSettings();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGNavSelection_MaxHistoricDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.nav.maxHistoric = EditorGUILayout.IntField(LC.G("NGNavSelection_MaxHistoric"), Preferences.Settings.nav.maxHistoric);
			if (EditorGUI.EndChangeCheck() == true)
			{
				Preferences.Settings.nav.maxHistoric = Mathf.Clamp(Preferences.Settings.nav.maxHistoric, 1, NGNavSelectionWindow.MaxHistoric);
				Preferences.InvalidateSettings();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(LC.G("NGNavSelection_MaxDisplayHierarchyDescription"), GeneralStyles.WrapLabel);
			Preferences.Settings.nav.maxDisplayHierarchy = EditorGUILayout.IntField(LC.G("NGNavSelection_MaxDisplayHierarchy"), Preferences.Settings.nav.maxDisplayHierarchy);
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (Preferences.Settings.nav.maxDisplayHierarchy < -1)
					Preferences.Settings.nav.maxDisplayHierarchy = -1;
				Preferences.InvalidateSettings();
			}
		}

		private static void	PlayStateChanged()
		{
			if (NGNavSelectionWindow.editorState == false && EditorApplication.isPlayingOrWillChangePlaymode == true)
			{
				NGNavSelectionWindow.editorState = true;
				NGNavSelectionWindow.SaveHistoric();
			}
			else if (NGNavSelectionWindow.editorState == true && EditorApplication.isPlayingOrWillChangePlaymode == false)
			{
				NGNavSelectionWindow.editorState = false;
				NGNavSelectionWindow.SaveHistoric();
			}
			else if (EditorApplication.isCompiling == true && NGNavSelectionWindow.savedOnCompile == false)
			{
				NGNavSelectionWindow.savedOnCompile = true;
				NGNavSelectionWindow.SaveHistoric();
			}
		}

		private static void	UpdateSelection()
		{
			int	hash = NGNavSelectionWindow.GetCurrentSelectionHash();

			if (NGNavSelectionWindow.lastHash == hash || hash == 0)
				return;

			if (Selection.objects.Length > 0)
			{
				// Prevent adding a new selection if the user just selected it through NG Nav Selection.
				if (0 <= NGNavSelectionWindow.lastFocusedHistoric && NGNavSelectionWindow.lastFocusedHistoric < NGNavSelectionWindow.historic.Count && NGNavSelectionWindow.historic[NGNavSelectionWindow.lastFocusedHistoric].GetSelectionHash() == hash)
					return;

				// Add a new selection or update the last one.
				if (NGNavSelectionWindow.historicCursor != -1)
				{
					NGNavSelectionWindow.historic.RemoveRange(NGNavSelectionWindow.historicCursor + 1, NGNavSelectionWindow.historic.Count - NGNavSelectionWindow.historicCursor - 1);
					NGNavSelectionWindow.historicCursor = -1;
				}

				// Detect a change in the selection only if user selects ONE Object.
				if (Selection.objects.Length == 1)
					NGNavSelectionWindow.historic.Add(new AssetsSelection(Selection.objects));
				else if (NGNavSelectionWindow.historic.Count >= 1)
					NGNavSelectionWindow.historic[NGNavSelectionWindow.historic.Count - 1] = new AssetsSelection(Selection.objects);

				if (Preferences.Settings.nav.maxHistoric > 0 && NGNavSelectionWindow.historic.Count > Preferences.Settings.nav.maxHistoric)
					NGNavSelectionWindow.historic.RemoveRange(0, NGNavSelectionWindow.historic.Count - Preferences.Settings.nav.maxHistoric);
			}
			else
				NGNavSelectionWindow.historicCursor = -1;

			NGNavSelectionWindow.lastFocusedHistoric = -1;

			NGNavSelectionWindow.lastHash = hash;
			if (NGNavSelectionWindow.SelectionChanged != null)
				NGNavSelectionWindow.SelectionChanged();
		}

		private static void	HandleMouseInputs()
		{
			if (EditorApplication.isCompiling == true && NGNavSelectionWindow.savedOnCompile == false)
			{
				NGNavSelectionWindow.savedOnCompile = true;
				NGNavSelectionWindow.SaveHistoric();
				return;
			}

			IntPtr	activeWindow = NGNavSelectionWindow.GetActiveWindow();

			if (activeWindow != IntPtr.Zero)
			{
				// Go to previous selection.
				if (NGNavSelectionWindow.GetAsyncKeyState((Int32)WindowsVirtualKey.VK_XBUTTON1) != 0)
				{
					if (NGNavSelectionWindow.buttonWasDown == false)
					{
						NGNavSelectionWindow.buttonWasDown = true;
						NGNavSelectionWindow.SelectPreviousSelection();
					}
				}
				// Go to next selection.
				else if (NGNavSelectionWindow.GetAsyncKeyState((Int32)WindowsVirtualKey.VK_XBUTTON2) != 0)
				{
					if (NGNavSelectionWindow.buttonWasDown == false)
					{
						NGNavSelectionWindow.buttonWasDown = true;
						NGNavSelectionWindow.SelectNextSelection();
					}
				}
				else
					NGNavSelectionWindow.buttonWasDown = false;
			}
		}

		public static void	SelectPreviousSelection()
		{
			if (NGNavSelectionWindow.historicCursor == 0)
				return;

			do
			{
				if (NGNavSelectionWindow.historicCursor == -1 && NGNavSelectionWindow.historic.Count >= 2)
				{
					if (Selection.activeInstanceID == 0)
						NGNavSelectionWindow.historicCursor = NGNavSelectionWindow.historic.Count - 1;
					else
						NGNavSelectionWindow.historicCursor = NGNavSelectionWindow.historic.Count - 2;
					NGNavSelectionWindow.lastHash = NGNavSelectionWindow.historic[NGNavSelectionWindow.historicCursor].GetSelectionHash();
				}
				else if (NGNavSelectionWindow.historicCursor > 0)
				{
					--NGNavSelectionWindow.historicCursor;
					NGNavSelectionWindow.lastHash = NGNavSelectionWindow.historic[NGNavSelectionWindow.historicCursor].GetSelectionHash();

					if (NGNavSelectionWindow.historicCursor == 0 && NGNavSelectionWindow.lastHash == 0)
						break;
				}
				else
					break;
			}
			while (NGNavSelectionWindow.lastHash == 0);

			if (0 <= NGNavSelectionWindow.historicCursor &&
				NGNavSelectionWindow.historicCursor < NGNavSelectionWindow.historic.Count)
			{
				NGNavSelectionWindow.historic[NGNavSelectionWindow.historicCursor].Select();
				NGNavSelectionWindow.lastFocusedHistoric = -1;
				if (NGNavSelectionWindow.SelectionChanged != null)
					NGNavSelectionWindow.SelectionChanged();
			}
		}

		public static void	SelectNextSelection()
		{
			int	lastNotNullSelection = NGNavSelectionWindow.historicCursor;

			do
			{
				if (NGNavSelectionWindow.historicCursor >= 0 && NGNavSelectionWindow.historicCursor < NGNavSelectionWindow.historic.Count - 1)
				{
					++NGNavSelectionWindow.historicCursor;
					NGNavSelectionWindow.lastHash = NGNavSelectionWindow.historic[NGNavSelectionWindow.historicCursor].GetSelectionHash();
					if (NGNavSelectionWindow.lastHash != 0)
						lastNotNullSelection = NGNavSelectionWindow.historicCursor;

					if (NGNavSelectionWindow.historicCursor >= NGNavSelectionWindow.historic.Count - 1)
						NGNavSelectionWindow.historicCursor = -1;
				}
				else
					break;
			}
			while (NGNavSelectionWindow.lastHash == 0);

			if (0 <= lastNotNullSelection &&
				lastNotNullSelection < NGNavSelectionWindow.historic.Count)
			{
				NGNavSelectionWindow.historic[lastNotNullSelection].Select();
				NGNavSelectionWindow.lastFocusedHistoric = -1;
				if (NGNavSelectionWindow.SelectionChanged != null)
					NGNavSelectionWindow.SelectionChanged();
			}
			else if (lastNotNullSelection != -1)
			{
				NGNavSelectionWindow.historic[NGNavSelectionWindow.historic.Count - 1].Select();
				NGNavSelectionWindow.lastFocusedHistoric = -1;
				if (NGNavSelectionWindow.SelectionChanged != null)
					NGNavSelectionWindow.SelectionChanged();
			}
		}

		private static int	GetCurrentSelectionHash()
		{
			for (int i = 0; i < Selection.instanceIDs.Length; i++)
			{
				if (EditorUtility.InstanceIDToObject(Selection.instanceIDs[i]) != null)
				// Yeah, what? Is there a problem with my complex anti-colisionning hash function?
					return Selection.instanceIDs.Sum();
			}

			return 0;
		}

		private static void	SaveHistoric()
		{
			StringBuilder	buffer = Utility.GetBuffer();

			for (int i = 0; i < NGNavSelectionWindow.historic.Count; ++i)
			{
				for (int j = 0; j < NGNavSelectionWindow.historic[i].refs.Count; j++)
				{
					Object	o = NGNavSelectionWindow.historic[i][j];

					if (o != null)
					{
						buffer.Append(o.GetInstanceID());
						buffer.Append(';');
					}
				}

				// Should never happens, except if the save is corrupted.
				if (buffer.Length > 0)
				{
					buffer.Length -= 1;
					buffer.Append(',');
				}
			}

			if (buffer.Length > 0)
				buffer.Length -= 1;

			NGEditorPrefs.SetString(NGNavSelectionWindow.AutoSavePrefKey, Utility.ReturnBuffer(buffer), true);
			NGEditorPrefs.SetInt(NGNavSelectionWindow.LastHashPrefKey, NGNavSelectionWindow.lastHash);
		}

		protected virtual void	OnEnable()
		{
			this.minSize = new Vector2(140F, EditorGUIUtility.singleLineHeight);

			this.listDrawer = new GUIListDrawer<AssetsSelection>();
			this.listDrawer.list = NGNavSelectionWindow.historic;
			this.listDrawer.ElementGUI = this.DrawSelection;
			this.listDrawer.reverseList = true;

			this.wantsMouseMove = true;

			Preferences.SettingsChanged += this.Repaint;
			NGNavSelectionWindow.SelectionChanged += this.Repaint;
		}

		protected virtual void	OnDestroy()
		{
			Preferences.SettingsChanged -= this.Repaint;
			NGNavSelectionWindow.SelectionChanged -= this.Repaint;
		}

		protected virtual void	OnGUI()
		{
			if (Preferences.Settings == null)
			{
				GUILayout.Label(string.Format(LC.G("RequiringConfigurationFile"), NGNavSelectionWindow.Title));
				if (GUILayout.Button(LC.G("ShoWPreferencesWindow")) == true)
					Utility.ShowPreferencesWindowAt(Constants.PreferenceTitle);
				return;
			}

			Rect	r = this.position;

			r.x = 0F;
			r.y = 0F;

			if (this.errorPopup.exception != null)
			{
				r.height = this.errorPopup.boxHeight;
				this.errorPopup.OnGUIRect(r);
				r.y += r.height;

				r.height = this.position.height - r.height;
			}

			try
			{
				this.listDrawer.OnGUI(r);
			}
			catch (Exception ex)
			{
				this.errorPopup.exception = ex;
			}
		}

		public static int	GetHistoricCursor()
		{
			return NGNavSelectionWindow.historicCursor;
		}

		public static void	SetHistoricCursor(int i)
		{
			NGNavSelectionWindow.historicCursor = i;

			if (i >= 0)
			{
				NGNavSelectionWindow.historic[NGNavSelectionWindow.historicCursor].Select();
				NGNavSelectionWindow.lastHash = NGNavSelectionWindow.GetCurrentSelectionHash();
			}
		}

		private void	DrawSelection(Rect r, AssetsSelection selection, int i)
		{
			if (selection.refs[0].@object == null)
			{
				GUI.enabled = false;
				GUI.Button(r, string.Join("/", selection.refs[0].hierarchy.ToArray()), GeneralStyles.ToolbarButtonLeft);
				GUI.enabled = true;
			}
			else
			{
				if (Event.current.type == EventType.Repaint)
				{
					if (i == NGNavSelectionWindow.historicCursor ||
						(NGNavSelectionWindow.historicCursor == -1 && i == NGNavSelectionWindow.historic.Count - 1 && Selection.activeObject == NGNavSelectionWindow.historic[NGNavSelectionWindow.historic.Count - 1][0]))
					{
						float	w = r.width;
						r.width = NGNavSelectionWindow.HighlightCursorWidth;
						EditorGUI.DrawRect(r, NGNavSelectionWindow.HighlightCursorBackgroundColor);
						r.x += r.width;
						r.width = w - r.width;
					}

					if (i == NGNavSelectionWindow.lastFocusedHistoric)
					{
						float	w = r.width;
						r.width = NGNavSelectionWindow.HighlightCursorWidth;
						EditorGUI.DrawRect(r, NGNavSelectionWindow.HighlightFocusedHistoricBackgroundColor);
						r.x += r.width;
						r.width = w - r.width;
					}
				}

				if (Event.current.type == EventType.MouseDrag &&
					(this.dragOriginPosition - Event.current.mousePosition).sqrMagnitude >= Constants.MinStartDragDistance &&
					i.Equals(DragAndDrop.GetGenericData(Utility.DragObjectDataName)) == true)
				{
					DragAndDrop.StartDrag("Drag Object");
					Event.current.Use();
				}
				else if (Event.current.type == EventType.MouseDown &&
						 r.Contains(Event.current.mousePosition) == true)
				{
					this.dragOriginPosition = Event.current.mousePosition;

					if (Event.current.button == 0)
					{
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.objectReferences = new UnityEngine.Object[] { selection.refs[0].@object };
						DragAndDrop.SetGenericData(Utility.DragObjectDataName, i);
					}
				}

				if (selection.refs.Count == 1)
					Utility.content.text = this.GetHierarchy(selection.refs[0].@object);
				else
					Utility.content.text = "(" + selection.refs.Count + ") " + this.GetHierarchy(selection.refs[0].@object);
				Utility.content.image = Utility.GetIcon(selection.refs[0].@object.GetInstanceID());

				if (GUI.Button(r, Utility.content, GeneralStyles.ToolbarButtonLeft))
				{
					if (Event.current.button == 1 || this.lastClick + Constants.DoubleClickTime > EditorApplication.timeSinceStartup)
					{
						selection.Select();
						NGNavSelectionWindow.lastFocusedHistoric = i;
					}
					else if (Event.current.button == 0)
						EditorGUIUtility.PingObject(selection.refs[0].@object);

					this.lastClick = EditorApplication.timeSinceStartup;
				}
				Utility.content.image = null;
			}
		}

		private string	GetHierarchy(Object obj)
		{
			if (Preferences.Settings.nav.maxDisplayHierarchy == 0)
				return obj.name;

			GameObject	go = obj as GameObject;

			if (go != null)
			{
				StringBuilder	buffer = Utility.GetBuffer();
				Transform		transform = go.transform;

				buffer.Insert(0, transform.gameObject.name);
				buffer.Insert(0, '/');
				transform = transform.parent;

				for (int i = 0; (Preferences.Settings.nav.maxDisplayHierarchy == -1 || i < Preferences.Settings.nav.maxDisplayHierarchy) && transform != null; i++)
				{
					buffer.Insert(0, transform.gameObject.name);
					buffer.Insert(0, '/');
					transform = transform.parent;
				}

				buffer.Remove(0, 1);

				return Utility.ReturnBuffer(buffer);
			}
			else
			{
				string	path = AssetDatabase.GetAssetPath(obj);

				if (string.IsNullOrEmpty(path) == false)
				{
					if (Preferences.Settings.nav.maxDisplayHierarchy == -1)
						return path;

					for (int i = path.Length - 1, j = 0; i >= 0; --i)
					{
						if (path[i] == '/')
						{
							++j;

							if (j - 1 == Preferences.Settings.nav.maxDisplayHierarchy)
								return path.Substring(i + 1);
						}
					}

					return path;
				}
			}

			return obj.name;
		}

		protected virtual void	ShowButton(Rect r)
		{
			Utility.content.text = string.Empty;
			Utility.content.tooltip = "Lock the size of this window when moving neighbor windows. It does not prevent from changing the size if you resize it.";
			EditorGUI.BeginChangeCheck();
			this.isLocked = GUI.Toggle(r, this.isLocked, Utility.content, GeneralStyles.LockButton);
			Utility.content.tooltip = string.Empty;
			if (EditorGUI.EndChangeCheck() == true)
			{
				if (this.isLocked == true)
				{
					this.initialMin = this.minSize;
					this.initialMax = this.maxSize;
					this.minSize = new Vector2(this.position.width, this.position.height);
					this.maxSize = this.minSize;
				}
				else
				{
					this.minSize = this.initialMin;
					this.maxSize = this.initialMax;
				}
			}
		}

		void	IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
		{
			Utility.AddNGMenuItems(menu, this, NGNavSelectionWindow.Title, Constants.WikiBaseURL + "#markdown-header-19-ng-nav-selection");
		}
	}
}