using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;

namespace NGToolsEditor.NGConsole
{
	using UnityEngine;

	namespace Test
	{
		public class LambdaClass
		{
			public static class NestedClass
			{
				public static void	A()
				{
					Debug.Log("Log in NestedClass in nested namespace.");
				}
			}

			public void	B()
			{
				Debug.Log("Log in nested namespace.");
			}
		}
	}

	[Serializable]
	[ExcludeFromExport]
	internal sealed class DebugModule : Module
	{
		public bool	debug;
		public bool	increment;

		public int	outputCountPerIteration = 1;
		public int	outputIterations = 1;

		public override void	OnEnable(NGConsoleWindow editor, int id)
		{
			base.OnEnable(editor, id);

			Conf.DebugModeChanged += this.OnDebugModeChanged;

			if (Conf.DebugMode != Conf.DebugModes.None)
				this.console.PostOnGUIHeader += DrawDebugBar;
		}

		public override void	OnDisable()
		{
			base.OnDisable();

			Conf.DebugModeChanged -= this.OnDebugModeChanged;

			this.console.PostOnGUIHeader -= DrawDebugBar;
		}

		private void	OnDebugModeChanged()
		{
			this.console.PostOnGUIHeader -= DrawDebugBar;
			if (Conf.DebugMode != Conf.DebugModes.None)
				this.console.PostOnGUIHeader += DrawDebugBar;

			this.console.Repaint();
		}

		private Rect	DrawDebugBar(Rect r)
		{
			float	x = r.x;

			r.height = 16F;

			GUI.Box(r, GUIContent.none);
			GUI.Label(r, "Debug");

			r.x += 50F;

			r.width = 20F;
			this.debug = GUI.Toggle(r, this.debug, "");
			r.x += r.width;

			r.width = 50F;
			if (GUI.Button(r, "Sync") == true)
				this.console.syncLogs.Sync();
			r.x += r.width;

			if (GUI.Button(r, "GC") == true)
			{
				long	before = GC.GetTotalMemory(false);
				GC.Collect();
				Debug.Log(before + " > " + GC.GetTotalMemory(true));
			}
			r.x += r.width;

			r.width = 75F;
			if (GUI.Button(r, "ClrCFiles") == true)
				Utility.files.Reset();
			r.x += r.width;
			if (GUI.Button(r, "ClrCClasses") == true)
				Utility.classes = new FastClassCache();
			r.x += r.width;
			if (GUI.Button(r, "DiffLcl") == true)
				this.DiffLocalesMissingKeysFromDefaultLanguage();
			r.x += r.width;
			if (GUI.Button(r, "CheckLcl") == true)
				this.CheckUnusedLocales();
			r.x += r.width;
			if (GUI.Button(r, "MultCtxts") == true)
				NGDebug.Log(Preferences.Settings, Preferences.Settings, Preferences.Settings);
			r.x += r.width;
			if (GUI.Button(r, "Snapshot") == true)
				NGDebug.Snapshot(this);
			r.x += r.width;
			if (GUI.Button(r, "TestParam") == true)
			{
				int	n = 0;
				int? m = 0;
				Type	t = null;
				Vector3?	p = null;
				this.TestParameterTypes<Type>(0, 1, true, 3, 4, 5, 6, ConsoleColor.Black, null, p, new Rect(), new Rect(),
											  out n, ref n, out m, ref m, null, out t);
			}
			r.x += r.width;

			if (GUI.Button(r, "ListReg") == true)
				this.OutputWinReg();

			r.x = x;
			r.y += r.height;

			r.width = 50F;

			// Test cases
			if (GUI.Button(r, "1L") == true)
				Debug.Log("monoline");
			r.x += r.width;

			r.width = 40F;
			this.outputCountPerIteration = EditorGUI.IntField(r, this.outputCountPerIteration);
			r.x += r.width;

			r.width = 10F;
			GUI.Label(r, "x");
			r.x += r.width;

			r.width = 40F;
			this.outputIterations = EditorGUI.IntField(r, this.outputIterations);
			r.x += r.width;

			r.width = 40F;

			if (GUI.Button(r, "Go") == true)
				Utility.StartBackgroundTask(this.TaskWriteLogs());
			r.x += r.width;

			r.width = 20F;
			this.increment = GUI.Toggle(r, this.increment, "");
			r.x += r.width;

			r.width = 40F;
			if (GUI.Button(r, "2L") == true)
				Debug.Log("first\nSECOND");
			r.x += r.width;
			if (GUI.Button(r, "3L") == true)
				Debug.Log("first\nSECOND\nThird");
			r.x += r.width;
			if (GUI.Button(r, "10L") == true)
				Debug.Log("first\nSECOND\nThird\n4\n5\n6\n7\n8\n9\n10");
			r.x += r.width;
			if (GUI.Button(r, "20L") == true)
				Debug.Log("first\nSECOND\nThird\n4\n5\n6\n7\n8\n9\n10\nfirst\nSECOND\nThird\n4\n5\n6\n7\n8\n9\n10");
			r.x += r.width;
			if (GUI.Button(r, "Warn.") == true)
				Debug.LogWarning("Warning");
			r.x += r.width;
			if (GUI.Button(r, "Err.") == true)
				Debug.LogError("Error");
			r.x += r.width;
			if (GUI.Button(r, "Exc.") == true)
				Debug.LogException(new NotImplementedException("NotImp", new Exception("innerException")));
			r.x += r.width;
			if (GUI.Button(r, "CatA") == true)
				this.CatA("TestA");
			r.x += r.width;
			if (GUI.Button(r, "CatB") == true)
				this.CatB("TestB");
#if !UNITY_4_5 && !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0
			r.x += r.width;
			if (GUI.Button(r, "Assert") == true)
				Debug.Assert(false, "Assert test");
#endif
			r.x += r.width;
			if (GUI.Button(r, "NestedA") == true)
				Test.LambdaClass.NestedClass.A();
			r.x += r.width;
			if (GUI.Button(r, "NestedB") == true)
				new Test.LambdaClass().B();
			r.x += r.width;
			if (GUI.Button(r, "DeepLog") == true)
			{
				try
				{
					var	ms = new MemoryStream();
					ms.Read(null, 0, 0);
				}
				catch (Exception ex)
				{
					InternalNGDebug.LogException(ex);
				}
			}
			// Syntax error
			//();
			// Warning unused variable
			//int	unusedVariable = 123;

			r.x = x;
			r.y += r.height;

			return r;
		}

		[NGLogger("A")]
		public void	CatA(object message)
		{
			UnityEngine.Debug.Log("[" + Constants.PackageTitle + "] " + message);
		}

		[NGLogger("B")]
		public void	CatB(object message)
		{
			UnityEngine.Debug.LogWarning("[" + Constants.PackageTitle + "] " + message);
		}

		private void	OutputWinReg()
		{
			Debug.Log("x32");

			string	registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
			using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key))
			{
				foreach (string subkey_name in key.GetSubKeyNames())
				{
					using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkey_name))
					{
						Debug.Log(subkey.GetValue("DisplayName"));
					}
				}
			}

			Debug.Log("x64");

			registry_key = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
			using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registry_key))
			{
				foreach (string subkey_name in key.GetSubKeyNames())
				{
					using (Microsoft.Win32.RegistryKey subkey = key.OpenSubKey(subkey_name))
					{
						Debug.LogError(subkey.GetValue("DisplayName"));
					}
				}
			}
		}

		private void	CheckUnusedLocales()
		{
			try
			{
				Dictionary<string, string>	defaultLocale = new Dictionary<string, string>();

				Localization.LoadLanguage(Constants.DefaultLanguage, defaultLocale, true);

				Dictionary<string, int>		keys = new Dictionary<string, int>(defaultLocale.Count);

				foreach (string key in defaultLocale.Keys)
				{
					keys.Add(key, 0);
				}

				foreach (Type type in Utility.EachAllSubClassesOf(typeof(object)))
				{
					FieldInfo[]	fields = type.GetFields();

					for (int i = 0; i < fields.Length; i++)
					{
						LocaleHeaderAttribute[]	attributes = fields[i].GetCustomAttributes(typeof(LocaleHeaderAttribute), false) as LocaleHeaderAttribute[];

						if (attributes.Length > 0)
						{
							if (keys.ContainsKey(attributes[0].key) == true)
								keys[attributes[0].key]++;
							else
								Debug.Log("LocaleHeader \"" + attributes[0].key + "\" is missing from default locale.");
						}
					}
				}

				//foreach (var item in keys)
				//{
				//	if (item.Value == 0)
				//		Debug.Log("Unused key \"" + item.Key + "\".");
				//}
			}
			catch
			{
			}
		}

		private IEnumerator	TaskWriteLogs()
		{
			int	n = this.outputCountPerIteration;
			int	m = this.outputIterations;

			if (this.increment == false)
			{
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
						Debug.Log("Line");

					yield return null;
				}
			}
			else
			{
				for (int i = 0; i < m; i++)
				{
					for (int j = 1; j <= n; j++)
						Debug.Log("Line" + (j + i * j));

					yield return null;
				}
			}
		}

		private void	TestParameterTypes<T>(float a, int b, bool c, byte d, short f, double g, decimal h,
											  Enum i, ILogFilter j, Vector3? j2, Rect l, Rect? m,
											  out int n, ref int o, out int? p, ref int? q,
											  T r, out T t, ILogFilter v = null, int w = 13, string x = "Jojo") where T : class
		{
			n = 0;
			p = 0;
			t = default(T);
			Debug.Log("Test parameter types.");
		}
		

		/// <summary>
		/// Checks missing keys from all languages against default language, then checks for new keys missing in the default language.
		/// </summary>
		public void	DiffLocalesMissingKeysFromDefaultLanguage()
		{
			try
			{
				string	rootPath = Path.Combine(Preferences.RootPath, Constants.RelativeLocaleFolder);

				string[]						languages = Directory.GetDirectories(rootPath);
				Dictionary<string, string>		defaultLocale = new Dictionary<string, string>();
				Dictionary<string, string>		workingLocale = new Dictionary<string, string>();
				Dictionary<string, TextAsset>	assetFiles = new Dictionary<string, TextAsset>();
				TextAsset						file = null;

				Localization.LoadLanguage(Constants.DefaultLanguage, defaultLocale, true);

				// Remove full path from languages.
				for (int i = 0; i < languages.Length; i++)
					languages[i] = languages[i].Substring(rootPath.Length);

				for (int i = 0; i < languages.Length; i++)
				{
					if (languages[i] == Constants.DefaultLanguage)
						continue;

					Localization.LoadLanguage(languages[i], workingLocale, true);

					foreach (var pair in defaultLocale)
					{
						// Check missing key from current locale against default one.
						if (workingLocale.ContainsKey(pair.Key) == false)
						{
							if (assetFiles.TryGetValue(pair.Value, out file) == false)
							{
								file = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(TextAsset)) as TextAsset;
								assetFiles.Add(pair.Value, file);
							}

							Debug.LogError("Localization[" + languages[i] + "][" + pair.Key + "] is missing, detected at \"" + pair.Value + "\".", file);
						}
					}

					foreach (var pair in workingLocale)
					{
						// Check for new keys.
						if (defaultLocale.ContainsKey(pair.Key) == false)
						{
							if (assetFiles.TryGetValue(pair.Value, out file) == false)
							{
								file = AssetDatabase.LoadAssetAtPath(pair.Value, typeof(TextAsset)) as TextAsset;
								assetFiles.Add(pair.Value, file);
							}

							Debug.LogWarning("Localization[" + languages[i] + "][" + pair.Key + "] is new from \"" + pair.Value + "\"", file);
						}
					}
				}
			}
			catch
			{
			}
		}
	}
}