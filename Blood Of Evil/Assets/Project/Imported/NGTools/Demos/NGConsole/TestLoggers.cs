#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace NGTools.Tests
{
	using UnityEngine;

	public class TestLoggers : MonoBehaviour
	{
		private enum TestMode
		{
			Loggers,
		}

		private const float	Spacing = 10F;
		private const float	XOffset = 20F;
		private const float	YOffset = 20F;
		private const float	SampleInterval = 1F;

		public float	currentTime;
		public float	randomThing;

		private Assembly	editorAssembly;

		private Type		ContactFormWizardType;
		private Type		NGConsoleType;
		private FieldInfo	VisibleModulesFieldInfo;
		private FieldInfo	SettingsFieldInfo;
		private MethodInfo	SetModuleMethodInfo;
		private MethodInfo	RepaintMethodInfo;

		private Type		ModuleType;
		private FieldInfo	IdFieldInfo;

		private Type		MainModuleType;

		private Type		RecorderModuleType;
		private FieldInfo	StreamsFieldInfo;

		private TestMode	mode;
		//private string[]	modeLabels;
		private Object		NGConsoleInstance;
		private object		workingModule;
		private bool		switchModule = false;

		private GUIStyle	centeredTextField;
		private GUIStyle	centeredLabel;
		private Rect		r = new Rect(0F, 0F, 0, 30F);
		private Rect		fullScreenRect = new Rect();
		private Vector2		loggersScrollPosition;

		private Coroutine	writingRecorderTest;

		protected virtual void	Awake()
		{
			Debug.Log("[Core] Awake");

			Assembly[]	assemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in assemblies)
			{
				if (assembly.FullName.StartsWith("Assembly-CSharp-Editor") == true ||
					assembly.FullName.StartsWith("Assembly-CSharp-Editor-firstpass") == true)
				{
					try
					{
						this.editorAssembly = assembly;
						this.ContactFormWizardType = this.editorAssembly.GetType("NGToolsEditor.ContactFormWizard");
						this.NGConsoleType = this.editorAssembly.GetType("NGToolsEditor.NGConsole.NGConsoleWindow");
						this.VisibleModulesFieldInfo = this.NGConsoleType.GetField("visibleModules", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
						this.SettingsFieldInfo = this.NGConsoleType.GetField("settings", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
						this.SetModuleMethodInfo = this.NGConsoleType.GetMethod("SetModule", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
						this.RepaintMethodInfo = this.NGConsoleType.GetMethod("Repaint", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

						this.ModuleType = this.editorAssembly.GetType("NGToolsEditor.NGConsole.Module");
						this.IdFieldInfo = this.ModuleType.GetField("id", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

						this.MainModuleType = this.editorAssembly.GetType("NGToolsEditor.NGConsole.MainModule");

						this.RecorderModuleType = this.editorAssembly.GetType("NGToolsEditor.NGConsole.RecorderModule");
						this.StreamsFieldInfo = this.RecorderModuleType.GetField("streams", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
						break;
					}
					catch
					{
					}
				}
			}

			this.mode = TestMode.Loggers;
			//this.modeLabels = Enum.GetNames(typeof(TestMode));
		}

		protected virtual void OnEnable()
		{
			Debug.Log("[Core] OnEnable");
			this.InvokeRepeating("Memory", 1F, 3F);
			this.InvokeRepeating("OnlyWarning", .5F, 4F);
		}

		protected virtual void OnDisable()
		{
			Debug.Log("[Core] OnDisable");
			this.CancelInvoke();
		}

		protected virtual void Memory()
		{
			Debug.Log("[Memory] " + GC.GetTotalMemory(false));
		}

		protected virtual void OnlyWarning()
		{
			Debug.LogWarning("Warning");
		}

		protected virtual void	Update()
		{
			this.currentTime = Time.realtimeSinceStartup;
			this.randomThing = Random.Range(0F, 1F);
		}

		protected virtual void	OnGUI()
		{
			if (this.centeredTextField == null)
			{
				this.centeredTextField = GUI.skin.textField;
				this.centeredTextField.alignment = TextAnchor.MiddleCenter;
				this.centeredLabel = new GUIStyle(GUI.skin.label);
				this.centeredLabel.alignment = TextAnchor.MiddleCenter;
			}

			if (this.NGConsoleType == null)
			{
				this.fullScreenRect.x = 0F;
				this.fullScreenRect.y = 0F;
				this.fullScreenRect.width = Screen.width;
				this.fullScreenRect.height = Screen.height;
				if (GUI.Button(this.fullScreenRect, "The NG Console tutorial is encountering issues. Contact the author and tell him that it is broken.", this.centeredLabel) == true)
				{
					// Force the wizard to be unique.
					ScriptableWizard.GetWindow(this.ContactFormWizardType).Close();
					ScriptableWizard.DisplayWizard(string.Empty, this.ContactFormWizardType);
				}
			}

			if (TestUtility.RequireUnmaximized() == true)
				return;

			this.r.x = TestLoggers.XOffset;
			this.r.y = TestLoggers.YOffset;
			this.r.width = Screen.width - this.r.x - this.r.x;

			Object[]	instances = Resources.FindObjectsOfTypeAll(NGConsoleType);

			if (instances.Length == 0)
			{
				if (GUI.Button(r, "Open NG Console before starting the tutorial") == true)
					EditorApplication.ExecuteMenuItem("Window/" + Constants.PackageTitle + "/ƝƓ Ҁonsole");
				return;
			}

			this.NGConsoleInstance = instances[0];

			if (this.SettingsFieldInfo != null)
			{
				if (this.SettingsFieldInfo.GetValue(this.NGConsoleInstance) == null)
				{
					this.fullScreenRect.x = 0F;
					this.fullScreenRect.y = 0F;
					this.fullScreenRect.width = Screen.width;
					this.fullScreenRect.height = Screen.height;
					GUI.Label(this.fullScreenRect, "NG Console requires an instance of NGSetting to run. Go to the console and link it to one.", this.centeredLabel);
					return;
				}
			}

			//GUI.Label(r, "Tests");
			//r.x += 100F;

			//r.width -= 100F;
			//EditorGUI.BeginChangeCheck();
			//this.mode = (TestMode)GUI.SelectionGrid(r, (int)this.mode, this.modeLabels, 4);
			//if (EditorGUI.EndChangeCheck() == true)
			//	this.switchModule = false;
			//r.y += r.height + r.height;
			//r.x -= 100F;
			//r.width += 100F;

			if (this.mode == TestMode.Loggers)
				this.DrawTestLoggers();
			//else if (this.mode == TestMode.Recorder)
			//	this.DrawTestRecorder();
		}

		private void	DrawTestRecorder()
		{
			this.SelectModuleOnNGConsole(this.RecorderModuleType);

			if (this.workingModule == null)
			{
				this.DisplayBrokenTutorialHelper("Recorder");
				return;
			}

			IList	streams = (IList)this.StreamsFieldInfo.GetValue(this.workingModule);

			if (streams.Count == 0)
			{
				GUI.Label(r, "First of all, create a sample.");
				return;
			}

			GUI.Label(r, "There is 2 conditions. The start and the end.");
			r.y += r.height;

			GUI.Label(r, "When the start condition is filled, the sample starts recordings logs.");
			r.y += r.height;

			GUI.Label(r, "When the end condition is filled, the sample stops.");
			r.y += r.height + r.height;

			GUI.Label(r, "Set conditions and try to capture the following pattern.");
			r.y += r.height;

			GUI.enabled = this.writingRecorderTest == null;
			if (GUI.Button(r, "Start the pattern below") == true)
			{
				this.writingRecorderTest = this.StartCoroutine(this.OutputSample());
			}
			GUI.enabled = true;

			r.y += r.height;
			r.y += r.height;

			GUI.Label(r, "Sample START");
			r.y += r.height;
			GUI.Label(r, "1 A Lambda message.");
			r.y += r.height;
			GUI.Label(r, "2 Warning, you might only record warnings in your sample.");
			r.y += r.height;
			GUI.Label(r, "3 Or errors, why not both?");
			r.y += r.height;
			GUI.Label(r, "4 Use filters to specify what you are willing to record.");
			r.y += r.height;
			GUI.Label(r, "5 You can create many samples.");
			r.y += r.height;
			GUI.Label(r, "6 Saves logs in a defined range of time, logs or space.");
			r.y += r.height;
			GUI.Label(r, "7 Want more? Look at the documentation and create your own condition!");
			r.y += r.height;
			GUI.Label(r, "Sample END");
		}

		private IEnumerator	OutputSample()
		{
			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.Log("Sample START");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.Log("1 A Lambda message.");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.LogWarning("2 Warning, you might only record warnings in your sample.");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.LogError("3 Or errors, why not both?");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.Log("4 Use filters to specify what you are willing to record.");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.LogWarning("5 You can create many samples.");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.LogError("6 Saves logs in a defined range of time, logs or space.");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.LogWarning("7 Want more? Look at the documentation and create your own condition!");

			yield return new WaitForSeconds(TestLoggers.SampleInterval);

			Debug.Log("Sample END");

			this.writingRecorderTest = null;
		}

		private void	DrawTestLoggers()
		{
			this.SelectModuleOnNGConsole(this.MainModuleType);

			if (this.workingModule == null)
			{
				this.DisplayBrokenTutorialHelper("Loggers");
				return;
			}

			GUI.Label(r, "Press buttons below to test the loggers provided by the class NGDebug.");
			r.y += r.height;
			GUI.Label(r, "Copy code to test it yourself.");
			r.y += r.height;

			Rect	pos = new Rect(0F, r.y, Screen.width, Screen.height - r.y);
			Rect	viewRect = new Rect(0F, 0F, 0F, r.height * 26 + TestLoggers.Spacing * 7);
			this.loggersScrollPosition = GUI.BeginScrollView(pos, this.loggersScrollPosition, viewRect);
			{
				r.y = 0F;

				if (GUI.Button(r, "NGTools.NGDebug.Log(GetComponents(typeof(Component)))") == true)
				{
					NGDebug.Log(this.GetComponents(typeof(Component)));
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(GetComponents(typeof(Component)));", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.Log(Physics.OverlapSphere(Vector3.zero, 1))") == true)
				{
					NGDebug.Log(Physics.OverlapSphere(Vector3.zero, 1F));
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(Physics.OverlapSphere(Vector3.zero, 1));", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.Log(Physics.RaycastAll(Vector3.zero, Vector3.up, 3))") == true)
				{
					NGDebug.Log(Physics.RaycastAll(Vector3.zero, Vector3.up, 3F));
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(Physics.RaycastAll(Vector3.zero, Vector3.up, 3));", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.Log(this, gameObject, transform)") == true)
				{
					NGDebug.Log(this, this.gameObject, this.transform);
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(this, gameObject, transform);", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.Log(NULL)") == true)
				{
					Object	o = null;

					NGDebug.Log(o);
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(NULL);", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.Log(NULL, this, NULL)") == true)
				{
					NGDebug.Log(null, this, null);
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Log(NULL, this, NULL);", this.centeredTextField);
				r.y += r.height + TestLoggers.Spacing;

				if (GUI.Button(r, "NGDebug.LogCollection(new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>()))") == true)
				{
					NGDebug.LogCollection(new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>()));
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.LogCollection(new List<GameObject>(Resources.FindObjectsOfTypeAll<GameObject>()));", this.centeredTextField);
				r.y += r.height;

				GUI.Label(r, "Use LogCollection with List, Stack or any array or collection that inherit from IEnumerable.");
				r.y += r.height;

				if (GUI.Button(r, "NGDebug.LogHierarchy(this)") == true)
				{
					NGDebug.LogHierarchy(this);
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.LogHierarchy(this);", this.centeredTextField);
				r.y += r.height;

				GUI.Label(r, "Use LogHierarchy to display the upward hierarchy of any Game Object, Component or RaycastHit.");
				r.y += r.height;

				if (GUI.Button(r, "NGDebug.Snapshot(this)") == true)
				{
					NGDebug.Snapshot(this);
				}
				r.y += r.height;

				GUI.TextField(r, "NGDebug.Snapshot(this);", this.centeredTextField);
				r.y += r.height;

				GUI.Label(r, "Use Snapshot to output public fields of any object at the time you call it.");
				r.y += r.height;

				GUI.Label(r, "Tips:");
				r.y += r.height;

				GUI.Label(r, "Pass your mouse over the images in NG Console to display the related Game Object's name.");
				r.y += r.height;

				GUI.Label(r, "Click on an image to ping the Object.");
				r.y += r.height;

				GUI.Label(r, "Double click on an image to select the Object.");
				r.y += r.height;

				GUI.Label(r, "Press Shift and click on an image to select the Object.");
				r.y += r.height;
			}
			GUI.EndScrollView();
		}

		private void	DisplayBrokenTutorialHelper(string title)
		{
			if (GUI.Button(r, "Tutorial \"" + title + "\" is broken, due to missing module. Please contact the author.") == true)
			{
				// Force the wizard to be unique.
				ScriptableWizard.GetWindow(this.ContactFormWizardType).Close();
				ScriptableWizard.DisplayWizard(string.Empty, this.ContactFormWizardType);
			}
		}

		private void	SelectModuleOnNGConsole(Type targetModuleType)
		{
			if (this.switchModule == false || this.workingModule == null)
			{
				this.switchModule = true;

				if (targetModuleType != null)
				{
					Array	visibleModules = (Array)this.VisibleModulesFieldInfo.GetValue(this.NGConsoleInstance);

					this.workingModule = null;

					if (visibleModules != null)
					{
						foreach (var module in visibleModules)
						{
							if (module.GetType() == targetModuleType)
							{
								object	id = this.IdFieldInfo.GetValue(module);

								this.SetModuleMethodInfo.Invoke(this.NGConsoleInstance, new object[] { id });
								this.workingModule = module;
								break;
							}
						}
					}

					this.RepaintMethodInfo.Invoke(this.NGConsoleInstance, null);
				}
			}
		}
	}
}
#endif
//using UnityEngine;

//public class TestLoggers : MonoBehaviour
//{
//	public int		anInteger;
//	public float	aFloat;
//	public string	aString;
//	public Object	anObject;

//	protected virtual void Awake()
//	{
//		Debug.Log("Awake");
//		this.InvokeRepeating("LogWarning", 1F, 1F);
//	}

//	private void	LogWarning()
//	{
//		Debug.LogWarning("Warning");
//	}

//	protected virtual void	OnGUI()
//	{
//		if (GUILayout.Button("NGDebug.Snapshot(this)",		GUILayout.Width(200F), GUILayout.Height(150F)) == true)
//		{
//			NGDebug.Snapshot(this);
//		}
//		if (GUILayout.Button("NGDebug.Log(GetComponents)", GUILayout.Width(200F), GUILayout.Height(150F)) == true)
//		{
//			NGDebug.Log(this.GetComponents<Component>());
//		}
//		if (GUILayout.Button("NGDebug.LogHierarchy(this)", GUILayout.Width(200F), GUILayout.Height(150F)) == true)
//		{
//			NGDebug.LogHierarchy(this);
//		}
//	}
//}