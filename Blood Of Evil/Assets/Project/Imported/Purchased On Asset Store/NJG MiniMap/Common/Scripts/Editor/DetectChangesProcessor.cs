using UnityEditor;
using System.IO;
using NJG;
using UnityEngine;

public class DetectChangesProcessor : UnityEditor.AssetModificationProcessor
{
	public static string[] OnWillSaveAssets(string[] paths)
	{
		// Get the name of the scene to save.
		//string scenePath = string.Empty;
		string sceneName = string.Empty;

		foreach (string path in paths)
		{
			if (path.Contains(".unity"))
			{
				//scenePath = Path.GetDirectoryName(path);
				sceneName = Path.GetFileNameWithoutExtension(path);
			}
		}

		if (sceneName.Length == 0)
		{
			return paths;
		}

		if(NJGMap.instance != null && !Application.isPlaying) NJGMap.instance.UpdateBounds();

		return paths;
	}
}