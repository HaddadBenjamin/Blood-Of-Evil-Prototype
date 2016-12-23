using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class PlaceToImageFillEnd : MonoBehaviour {

	public Image target;
	private RectTransform rt;
	public Text percentText;

	void Update () 
	{
		if (!rt)
		{
			rt = GetComponent<RectTransform>();
		}

		if (target && target.fillMethod == Image.FillMethod.Horizontal)
		{
			if (rt)
			{
				Vector2 pos = rt.anchoredPosition;
				pos.x = target.rectTransform.rect.width * target.fillAmount;
				rt.anchoredPosition = pos;
			}

			if (percentText)
				percentText.text = ((int)(target.fillAmount * 100)).ToString() + '%';
		}
	}
}
