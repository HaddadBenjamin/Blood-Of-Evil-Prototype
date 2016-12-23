using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class HooksTracker 
#if UNITY_EDITOR
	: EditorWindow
#endif
{
#if UNITY_EDITOR
		
	[MenuItem("REPLICA/Hooks tracker")]
	public static void ShowWindow()
	{
		HooksTracker window = EditorWindow.GetWindow<HooksTracker>();
		window.Show();
	}

	public static void StartTracking(string c, string m, System.Reflection.MethodInfo fct)
	{
		FeedHooksToHooked(c, m, fct);
		FeedHookedToHooks(c, m, fct);
	}

	public static void StopTracking(string c, string m, System.Reflection.MethodInfo fct)
	{
		DigestHooksToHooked(c, m, fct);
		DigestHookedToHooks(c, m, fct);
	}

	bool hooksToHooked;

	void OnGUI()
	{
		if (TrackedHooks != null)
		{
			if (hooksToHooked)
				if (GUILayout.Button("Hooks to hooked")) hooksToHooked = false;
			if (!hooksToHooked)
				if (GUILayout.Button("Hooked to hooks")) hooksToHooked = true;

			if (hooksToHooked)
			{
				this.HooksToHooked();
			}
			else
			{
				this.HookedToHooks();
			}
		}
	}

	#region Hooks to hooked

	class HookableEvent
	{
		public List<System.Reflection.MethodInfo> hooks;
		public bool folded;

		public HookableEvent()
		{
			hooks = new List<System.Reflection.MethodInfo>();
		}
	}

	class HookableClass
	{
		public Dictionary<string, HookableEvent> events;
		public bool folded;

		public HookableClass()
		{
			events = new Dictionary<string, HookableEvent>();
		}
	}

	private static Dictionary<string, HookableClass> TrackedHooks;

	private static void FeedHooksToHooked(string c, string m, MethodInfo fct)
	{
		if (TrackedHooks == null)
			TrackedHooks = new Dictionary<string, HookableClass>();

		if (!TrackedHooks.ContainsKey(c))
			TrackedHooks.Add(c, new HookableClass());

		if (!TrackedHooks[c].events.ContainsKey(m))
			TrackedHooks[c].events.Add(m, new HookableEvent());

		TrackedHooks[c].events[m].hooks.Add(fct);
	}

	private static void DigestHooksToHooked(string c, string m, MethodInfo fct)
	{
		if (TrackedHooks != null)
		{
			if (TrackedHooks.ContainsKey(c))
			{
				if (TrackedHooks[c].events.ContainsKey(m))
				{
					TrackedHooks[c].events[m].hooks.Remove(fct);

					if (TrackedHooks[c].events[m].hooks.Count == 0)
						TrackedHooks[c].events.Remove(m);
				}

				if (TrackedHooks[c].events.Count == 0)
					TrackedHooks.Remove(c);
			}

			if (TrackedHooks.Count == 0)
				TrackedHooks = null;
		}
	}

	private void HooksToHooked()
	{
		// List of hooks
		foreach (KeyValuePair<string, HookableClass> hClass in TrackedHooks)
		{
			hClass.Value.folded = EditorGUILayout.Foldout(hClass.Value.folded, hClass.Key);
			if (hClass.Value.folded)
			{
				EditorGUI.indentLevel++;

				// Hooks events
				foreach (KeyValuePair<string, HookableEvent> hEvent in hClass.Value.events)
				{
					hEvent.Value.folded = EditorGUILayout.Foldout(hEvent.Value.folded, hEvent.Key);
					if (hEvent.Value.folded)
					{
						EditorGUI.indentLevel++;

						// Hooked list
						foreach (System.Reflection.MethodInfo hooked in hEvent.Value.hooks)
						{
							EditorGUILayout.LabelField(hooked.DeclaringType.Name, hooked.Name);
						}

						EditorGUI.indentLevel--;
					}
				}

				EditorGUI.indentLevel--;
			}
		}
	}

	#endregion

	#region Hooked to hooks

	class HookedClass
	{
		public bool folded;

		public Dictionary<string, HookedHookedClass> classes;

		public HookedClass()
		{
			folded = false;
			classes = new Dictionary<string, HookedHookedClass>();
		}
	}

	struct Hook
	{
		public string eventName;
		public string functionName;

		public Hook(string m, string fctName)
		{
			eventName = m;
			functionName = fctName;
		}
	}

	class HookedHookedClass
	{
		public List<Hook> hooks;

		public HookedHookedClass()
		{
			hooks = new List<Hook>();
		}
	}
		
	private static Dictionary<string, HookedClass> TrackedHooked;

	private static void FeedHookedToHooks(string c, string m, MethodInfo fct)
	{
		string tName = fct.DeclaringType.Name;

		if (TrackedHooked == null)
			TrackedHooked = new Dictionary<string, HookedClass>();
				
		if (!TrackedHooked.ContainsKey(tName))
			TrackedHooked.Add(tName, new HookedClass());

		if (!TrackedHooked[tName].classes.ContainsKey(c))
			TrackedHooked[tName].classes.Add(c, new HookedHookedClass());

		TrackedHooked[tName].classes[c].hooks.Add(new Hook(m, fct.Name));
	}

	private static void DigestHookedToHooks(string c, string m, MethodInfo fct)
	{
		string tName = fct.DeclaringType.Name;

		if (TrackedHooked != null)
		{
			if (TrackedHooked.ContainsKey(tName))
			{
				if (TrackedHooked[tName].classes.ContainsKey(c))
				{
					TrackedHooked[tName].classes[c].hooks.Remove(new Hook(m, fct.Name));

					if (TrackedHooked[tName].classes[c].hooks.Count == 0)
						TrackedHooked[tName].classes.Remove(c);
				}

				if (TrackedHooked[tName].classes.Count == 0)
					TrackedHooked.Remove(tName);
			}

			if (TrackedHooked.Count == 0)
				TrackedHooked = null;
		}
	}

	private void HookedToHooks()
	{
		// List of hooked classes
		foreach (KeyValuePair<string, HookedClass> hClass in TrackedHooked)
		{
			hClass.Value.folded = EditorGUILayout.Foldout(hClass.Value.folded, hClass.Key);
			if (hClass.Value.folded)
			{
				EditorGUI.indentLevel++;

				// Hooks events
				foreach (KeyValuePair<string, HookedHookedClass> c in hClass.Value.classes)
				{
					EditorGUILayout.LabelField(c.Key);

					EditorGUI.indentLevel++;
					foreach (Hook hook in c.Value.hooks)
					{
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField(hook.eventName);
							EditorGUILayout.LabelField(hook.functionName);
						EditorGUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}
		}
	}

	#endregion

	void Update()
	{
		this.Repaint();
	}

#else
	public static void StartTracking(string c, string m, System.Reflection.MethodInfo fct)
	{

	}

	public static void StopTracking(string c, string m, System.Reflection.MethodInfo fct)
	{

	}
#endif
}
