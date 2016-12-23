using UnityEngine;
using System.Collections;
using UnityEditor;

public class RectTransformCustomEditor : Editor
{
	[MenuItem("CONTEXT/RectTransform/AnchorFit")]
	private static void AnchorFit(MenuCommand menuCommand)
	{
		RectTransform r = menuCommand.context as RectTransform;
		if (r != null && r.parent != null)
		{
			Undo.RecordObject(r, "AnchorFit");
			AnchorFitPoint(menuCommand, false);
			RectTransform parentRt = r.parent.GetComponent<RectTransform>();
			Vector2 newAnchorMin, newAnchorMax;
			Vector3 position = r.position;

			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.zero;
			r.position = position;
			{
				Debug.Log(r.anchoredPosition);
				Debug.Log(parentRt.anchoredPosition);
				Debug.Log(r.anchoredPosition.x / parentRt.rect.width);
				Debug.Log(r.anchoredPosition.y / parentRt.rect.height);
				newAnchorMin.x = (r.anchoredPosition.x - (r.rect.width * 0.5f)) / parentRt.rect.width;
				newAnchorMax.x = (r.anchoredPosition.x + (r.rect.width * 0.5f)) / parentRt.rect.width;
				newAnchorMin.y = (r.anchoredPosition.y - (r.rect.height * 0.5f)) / parentRt.rect.height;
				newAnchorMax.y = (r.anchoredPosition.y + (r.rect.height * 0.5f)) / parentRt.rect.height;
			}
			r.anchorMin = newAnchorMin;
			r.anchorMax = newAnchorMax;
			r.sizeDelta = Vector2.zero;
			r.position = position;
		}
	}
	[MenuItem("CONTEXT/RectTransform/AnchorFitPoint")]
	private static void _AnchorFitPoint(MenuCommand menuCommand)
	{
		AnchorFitPoint(menuCommand);
	}
	private static void AnchorFitPoint(MenuCommand menuCommand, bool undo = true)
	{
		RectTransform r = menuCommand.context as RectTransform;
		if (r != null && r.parent != null)
		{
			if (undo)
				Undo.RecordObject(r, "AnchorFitPoint");
			RectTransform parentRt = r.parent.GetComponent<RectTransform>();
			Vector2 newAnchorMin, newAnchorMax;
			Vector3 position = r.position;
			Vector2 sizeDelta = new Vector2(r.rect.width, r.rect.height);

			r.anchorMin = Vector2.zero;
			r.anchorMax = Vector2.zero;
			r.position = position;
			{
				Debug.Log(r.anchoredPosition);
				Debug.Log(parentRt.anchoredPosition);
				newAnchorMin.x = r.anchoredPosition.x / parentRt.rect.width;
				newAnchorMax.x = newAnchorMin.x;
				newAnchorMin.y = r.anchoredPosition.y / parentRt.rect.height;
				newAnchorMax.y = newAnchorMin.y;
			}
			r.anchorMin = newAnchorMin;
			r.anchorMax = newAnchorMax;
			r.sizeDelta = sizeDelta;
			r.position = position;
		}
	}
}
