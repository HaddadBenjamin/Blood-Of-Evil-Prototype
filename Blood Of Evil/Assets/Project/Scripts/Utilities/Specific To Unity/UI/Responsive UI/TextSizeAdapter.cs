using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class TextSizeAdapter : MonoBehaviour {
	
	public Text text;
	public RectTransform target;
	public Vector2 padding;
	public Vector2 minSize;

	public bool x;
	public bool y;
	public bool checkOnUpdate;

	public bool cantBeLess;

	private Vector2 size;

	void OnEnable()
	{
		if (!text || !target)
		{
			this.enabled = false;
			return;
		}
	}
	
	void Update ()
	{
		if(checkOnUpdate)
			this.CheckResize();
	}
	
	public Vector2 CheckResize()
	{
		if (!text || !target)
		{
			this.enabled = false;
			return Vector2.zero;
		}

		size = this.target.rect.size;

		Vector2 wantedSize;
		wantedSize.x = text.preferredWidth;
		wantedSize.y = text.preferredHeight;

		//wantedSize.x = Mathf.Min(wantedSize.x, text.rectTransform.rect.width);
		//wantedSize.y = Mathf.Min(wantedSize.y, text.rectTransform.rect.height);
		
		wantedSize.x = Mathf.Max(wantedSize.x + padding.x, minSize.x);
		wantedSize.y = Mathf.Max(wantedSize.y + padding.y, minSize.y);
				
		if (x && size.x != wantedSize.x)
		{
			this.target.SetSizeWithCurrentAnchors(
				RectTransform.Axis.Horizontal,
				wantedSize.x
			);
		}

		if (y && size.y != wantedSize.y)
		{
			this.target.SetSizeWithCurrentAnchors(
				RectTransform.Axis.Vertical,
				wantedSize.y
			);
		}

		size = wantedSize;
		return size;
	}
}
