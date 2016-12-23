using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class placeToStartOfText : MonoBehaviour {

	public Text target;
	public float offset;

	void Update () 
	{
		if (target)
		{
			RectTransform rt = (this.transform as RectTransform);
			if (rt)
			{
				Vector2 pos = rt.anchoredPosition;
				pos.x = target.rectTransform.anchoredPosition.x - (target.preferredWidth * target.rectTransform.pivot.x) - offset;
				rt.anchoredPosition = pos;
			}
		}
	}
}
