using UnityEngine;
using UnityEditor;
using System;

public static class EditorUtils
{
	/// <summary>
	/// Searches the editor script among every created scripts
	/// <remarks>Should be called by 'OnEnable'</remarks>
	/// </summary>
	/// <param name="editor">The editor that wants to get its Monoscript</param>
	/// <returns>the Monoscript</returns>
	public static MonoScript GetScript(this Editor editor)
	{
		Type scriptType = editor.target.GetType();

		string[] guids = AssetDatabase.FindAssets("t:Script");
		string path;
		MonoScript script;

		foreach (string guid in guids)
		{
			path = AssetDatabase.GUIDToAssetPath(guid);
			script = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
			
			if (script.GetClass() == scriptType)
			{
				return script;
			}
		}

		return null;
	}
	
}
