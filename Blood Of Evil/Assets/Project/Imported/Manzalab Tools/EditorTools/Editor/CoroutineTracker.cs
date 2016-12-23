using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class CoroutineTracker 
#if UNITY_EDITOR
	: EditorWindow
#endif
{
#if UNITY_EDITOR
	class CoroutineData
	{
		public MonoBehaviour runner;
		public Coroutine trackingCoroutine;
		public Coroutine unityCoroutine;
		public string coroutineName;
	}

	private static List<CoroutineData> CoroutinesList = new List<CoroutineData>();

	[MenuItem("REPLICA/Coroutine tracker")]
	public static void ShowWindow()
	{
		CoroutineTracker window = EditorWindow.GetWindow<CoroutineTracker>();
		window.Show();
	}

	public static void StopAllCoroutines(MonoBehaviour runner)
	{
#if LOG_ROUTINES && DEBUG
		Debug.Log("ROUTINE : <<STOP ALL>>");
#endif
		runner.StopAllCoroutines();

		List<CoroutineData> updated = new List<CoroutineData>();
		foreach (CoroutineData data in CoroutinesList)
		{
			if (data.runner && data.runner != runner)
			{
				updated.Add(data);
			}
		}

		CoroutinesList = updated;
	}

	public static Coroutine StartCoroutine(string name, MonoBehaviour runner, IEnumerator routine)
	{
#if LOG_ROUTINES && DEBUG
		Debug.Log("ROUTINE : " + name + " --> starts");
#endif

		CoroutineData data = new CoroutineData();
		data.coroutineName = name;
		data.runner = runner;
		data.unityCoroutine = runner.StartCoroutine(routine);
		return data.trackingCoroutine = runner.StartCoroutine(TrackRoutineEnum(data));
	}

	private static IEnumerator TrackRoutineEnum(CoroutineData data)
	{
#if LOG_ROUTINES_STATE && DEBUG
		string previous = "Tracked routines state\n";
		foreach (CoroutineData prevData in CoroutinesList)
		{
			previous += prevData.coroutineName + '\n';
		}
		Debug.Log(previous);
#endif

		if (data.unityCoroutine != null) // yield break without returns.
		{
			CoroutinesList.Add(data);
			yield return data.unityCoroutine;

#if LOG_ROUTINES && DEBUG
			Debug.Log("ROUTINE : " + data.coroutineName + " --> finished");
#endif

			CoroutinesList.Remove(data);
		}
		else
		{
#if LOG_ROUTINES && DEBUG
			Debug.Log("ROUTINE : " + data.coroutineName + " --> finished (one shot)");
#endif
		}
	}

	void OnGUI()
	{
		if (CoroutinesList != null)
		{
			List<CoroutineData> updated = new List<CoroutineData>();
			foreach (CoroutineData data in CoroutinesList)
			{
				if (data.runner)
				{
					EditorGUILayout.LabelField(data.coroutineName);
					updated.Add(data);
				}
			}

			CoroutinesList = updated;
		}
	}

	void Update()
	{
		this.Repaint();
	}

#else
	public static Coroutine StartCoroutine(string name, MonoBehaviour runner, IEnumerator routine)
	{
		return runner.StartCoroutine(routine);
	}

	public static void StopAllCoroutines(MonoBehaviour runner)
	{
		runner.StopAllCoroutines();
	}
#endif
}
