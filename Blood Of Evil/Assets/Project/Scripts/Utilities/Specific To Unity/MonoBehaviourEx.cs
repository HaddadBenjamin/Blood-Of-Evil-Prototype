//using UnityEngine;
//using System.Collections;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
/// 
//public abstract class MonoBehaviourEx : MonoBehaviour
//{
//	/// <summary>
//	/// Module color in logs
//	/// </summary>
//	protected virtual string debugModuleColorCode { get { return "#0044C4"; } }

//	/// <summary>
//	/// Module error color in logs
//	/// </summary>
//	protected virtual string debugModuleErrorColorCode { get { return "#FF0000"; } }

//	/// <summary>
//	/// Module warning color in logs
//	/// </summary>
//	protected virtual string debugModuleWarningColorCode { get { return "##F7D358"; } }

//	/// <summary>
//	/// If true, prints time since startup in logs
//	/// </summary>
//	protected bool debugShowTime = false;

//	/// <summary>
//	/// Add debug data in a log message
//	/// </summary>
//	/// <param name="debugString">log to treat</param>
//	/// <returns>treated log</returns>
//	protected string Log(string debugString)
//	{
//		return _Log(debugString, debugModuleColorCode);
//	}

//	/// <summary>
//	/// Add debug data in a log warning
//	/// </summary>
//	/// <param name="debugString">log to treat</param>
//	/// <returns>treated log</returns>
//	protected string LogWarning(string debugString)
//	{
//		return _Log(debugString, debugModuleWarningColorCode);
//	}

//	/// <summary>
//	/// Add debug data in a log error
//	/// </summary>
//	/// <param name="debugString">log to treat</param>
//	/// <returns>treated log</returns>
//	protected string LogError(string debugString)
//	{
//		return _Log(debugString, debugModuleErrorColorCode);
//	}

//	/// <summary>
//	/// Add debug data in a log message
//	/// </summary>
//	/// <param name="debugString">log to treat</param>
//	/// <param name="colorCode">data color</param>
//	/// <returns>treated log</returns>
//	private string _Log(string debugString, string colorCode = "#000000")
//	{
//#if UNITY_EDITOR

//		var stackTrace = new System.Diagnostics.StackTrace(true);
//		var frame = stackTrace.GetFrame(2);
//		var filename = frame.GetFileName();

//		// Cas de la coroutine :)
//		if (filename == null)
//		{
//			frame = stackTrace.GetFrame(3);
//			filename = frame.GetFileName();
//		}

//		filename = System.IO.Path.GetFileName(filename);
//		filename = filename.Replace(".cs", "");

//		var className = this.GetType().Name;
//		if (filename != className)
//			filename += " (" + className + ")";

//		return "<color=" + colorCode + ">[" + (debugShowTime ? Time.realtimeSinceStartup.ToString("0000.000 ") : "") +
//				filename
//				+ "]</color>: " + debugString;
//#else
//		return debugString;
//#endif
//	}

//	public void StopAllTrackedCoroutines()
//	{
//		CoroutineTracker.StopAllCoroutines(this);
//	}

//	public Coroutine StartTrackedCoroutine(string c, string name, IEnumerator routine)
//	{
//		return CoroutineTracker.StartCoroutine(c + '.' + name, this, routine);
//	}

//	public Coroutine StartTrackedCoroutine(string name, IEnumerator routine)
//	{
//		return CoroutineTracker.StartCoroutine(this.GetType().Name + '.' + name, this, routine);
//	}

//	public void DelayedAction(System.Action action, float delay, string trackedName = "delayedActionCoroutine")
//	{
//		this.StartTrackedCoroutine(trackedName, delayedActionCoroutine(action, delay));
//	}

//	//public void CancelDelayedAction()
//	//{
//	//	this.StopCoroutine("delayedActionCoroutine");
//	//}

//	public void StartAsyncLoop(System.Action<float> action, float duration, System.Action endDlg = null)
//	{
//		this.StartTrackedCoroutine("asyncLoopCoroutine", asyncLoopCoroutine(action, duration, endDlg));
//	}

//	private IEnumerator delayedActionCoroutine(System.Action action, float delay)
//	{
//		if (action != null)
//		{
//			yield return new WaitForSeconds(delay);
//			action();
//		}
//	}

//	private IEnumerator asyncLoopCoroutine(System.Action<float> action, float duration, System.Action endDlg = null)
//	{
//		float currentTime = 0;

//		while (currentTime < duration)
//		{
//			action(currentTime / duration);
//			yield return null;
//			currentTime += Time.deltaTime;
//		}
//		action(1f);
//		if (endDlg != null)
//			endDlg();
//	}

//}
