using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class AdaptHeightLayout : LayoutGroup
{
	public bool ArrangeHeight;

	public float MinimumLayoutHeight;

	/// <summary>
	/// Sum of the children height
	/// </summary>
	private float heightSum;


	/// <summary>
	/// Called when the height of the layout changed
	/// </summary>
	public event Action<float> Resized;

	/// <summary>
	/// Sum of the children width
	/// </summary>
	private float widthSum;

	private float ComputeWantedHeight()
	{
		float h = this.padding.top + this.padding.bottom;

		float max = 0;
		float current;

		foreach (RectTransform r in this.rectChildren)
		{
			if (r.gameObject.activeSelf)
			{
				current = r.rect.height - r.anchoredPosition.y;
				max = Mathf.Max(max, current);
			}
		}

		h += max;

		if (this.MinimumLayoutHeight > 0)
			return Mathf.Max(h, this.MinimumLayoutHeight);

		return h;
	}

	public override void CalculateLayoutInputVertical()
	{
		heightSum = this.ComputeWantedHeight();

		if (ArrangeHeight)
		{
			this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				heightSum);

			if (Resized != null)
				Resized(heightSum);
		}
	}

	public override void SetLayoutHorizontal()
	{

	}

	public override void SetLayoutVertical()
	{

	}
}
