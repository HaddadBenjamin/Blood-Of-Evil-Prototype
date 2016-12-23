using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class TextToImageSizeAdapter : MonoBehaviour {
	
	public Text text;
	public Image image;
	public Vector2 padding;
	public Vector2 minSize;

	public bool x;
	public bool y;
	public bool hideIfNoText;
	public bool checkOnUpdate;

	private Vector2 size;

	void OnEnable()
	{
		if (!text || !image)
		{
			this.enabled = false;
			return;
		}

		size = this.image.rectTransform.rect.size;
	}
	
	void Update ()
	{
		if(checkOnUpdate)
			this.CheckResize();
	}
	
	public Vector2 CheckResize()
	{
		if (!text || !image)
		{
			this.enabled = false;
			return Vector2.zero;
		}

		Vector2 wantedSize;

		wantedSize.x = Mathf.Max(Mathf.Min(text.preferredWidth, text.rectTransform.rect.width) + padding.x, minSize.x);
		wantedSize.y = Mathf.Max(Mathf.Min(text.preferredHeight, text.rectTransform.rect.height) + padding.y, minSize.y);
				
		if (x && size.x != wantedSize.x)
		{
			this.image.rectTransform.SetSizeWithCurrentAnchors(
				RectTransform.Axis.Horizontal,
				wantedSize.x
			);
		}

		if (y && size.y != wantedSize.y)
		{
			this.image.rectTransform.SetSizeWithCurrentAnchors(
				RectTransform.Axis.Vertical,
				wantedSize.y
			);
		}

		if (hideIfNoText)
		{
			this.image.enabled = (text.text != string.Empty);
		}

		size = wantedSize;
		return size;
	}
}
