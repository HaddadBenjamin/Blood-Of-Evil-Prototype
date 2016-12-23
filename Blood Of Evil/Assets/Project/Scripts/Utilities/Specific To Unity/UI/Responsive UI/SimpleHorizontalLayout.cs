using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Cette classe vient de Manzalab.
/// Horizontal layout that doesn't change its children size
/// </summary>
public class SimpleHorizontalLayout : LayoutGroup
{
	/// <summary>
	/// Layout mode
	/// </summary>
	public enum Mode
	{
		/// <summary>
		/// Don't change any rectTransform width
		/// </summary>
		DontSetWidth,

		/// <summary>
		/// Changes the layout width
		/// </summary>
		SetLayoutWidth,

		/// <summary>
		/// Changes children width
		/// </summary>
		SetElementsWidth,

		/// <summary>
		/// Changes last child width
		/// </summary>
		SetLastElementWidth
	}

	/// <summary>
	/// Layout mode
	/// </summary>
	public Mode layoutMode;

	/// <summary>
	/// Maximum child width if SetElementsWidth setted
	/// </summary>
	[Tooltip("If layout mode is set to \"Set Elements Width\"")]
	public float maxChildrenWidth = 250;

	/// <summary>
	/// Minimum layout width
	/// </summary>
	public override float minWidth
	{
		get
		{
			return widthSum;
		}
	}

	/// <summary>
	/// Preferred layout width (=0)
	/// </summary>
	public override float preferredWidth
	{
		get
		{
			return 0;
		}
	}

	/// <summary>
	/// Flexible layout width (=0)
	/// </summary>
	public override float flexibleWidth
	{
		get
		{
			return 0;
		}
	}

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
	
	/// <summary>
	/// Space between children
	/// </summary>
	public float spacing = 0;

	/// <summary>
	/// Sets the Y value
	/// </summary>
	public bool SetY;

	/// <summary>
	/// Sets the width of the layout and of the children
	/// </summary>
	public override void SetLayoutHorizontal()
	{
		// Callback invoked by the auto layout system which handles horizontal aspects of the layout.

		float x = 0;

		this.CalculateLayoutInputVertical();

		if (this.childAlignment == TextAnchor.LowerCenter || this.childAlignment == TextAnchor.MiddleCenter || this.childAlignment == TextAnchor.UpperCenter)
			x = (this.rectTransform.rect.width - widthSum) * 0.5f;
		if(this.childAlignment == TextAnchor.LowerLeft || this.childAlignment == TextAnchor.MiddleLeft || this.childAlignment == TextAnchor.UpperLeft)
			x = padding.left;
		if (this.childAlignment == TextAnchor.LowerRight || this.childAlignment == TextAnchor.MiddleRight || this.childAlignment == TextAnchor.UpperRight)
			x = this.rectTransform.rect.width - (widthSum + padding.right);

		float y;

		//if (this.childAlignment == TextAnchor.LowerLeft || this.childAlignment == TextAnchor.LowerCenter || this.childAlignment == TextAnchor.LowerRight)
		//
		//if (this.childAlignment == TextAnchor.MiddleLeft || this.childAlignment == TextAnchor.MiddleCenter || this.childAlignment == TextAnchor.MiddleRight)
		//
		//if (this.childAlignment == TextAnchor.UpperLeft || this.childAlignment == TextAnchor.UpperCenter || this.childAlignment == TextAnchor.UpperRight)
			y = padding.top;

		float childWidth = 0;
		if (layoutMode == Mode.SetElementsWidth && this.rectChildren.Count > 0)
		{
			childWidth = (this.rectTransform.rect.width / this.rectChildren.Count) - spacing;
			if (maxChildrenWidth != 0)
				childWidth = Mathf.Min(this.maxChildrenWidth, childWidth);
		}

		RectTransform last = null;
		float lastx = 0;
		foreach (RectTransform r in this.rectChildren)
		{
			float w = r.rect.width;

			if (layoutMode == Mode.SetElementsWidth)
			{
				r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childWidth);
				w = childWidth;
			}

			r.anchoredPosition = new Vector2(x + (r.pivot.x * w), SetY ? y : r.anchoredPosition.y);
			
			last = r;
			lastx = x;
			x += w + spacing;
		}

		if (layoutMode == Mode.SetLastElementWidth && last != null)
		{
			last.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				this.rectTransform.rect.width - lastx);
		}
	}

	/// <summary>
	/// Get next children X position
	/// </summary>
	/// <returns>next child x-position</returns>
	public float NextXPosition()
	{
		CalculateLayoutInputVertical();
		return widthSum + spacing;
	}

	/// <summary>
	/// Does nothing
	/// </summary>
	public override void SetLayoutVertical()
	{
		// Callback invoked by the auto layout system which handles vertical aspects of the layout.
				
		
	}

	private float ComputeWantedHeight()
	{
		float h = this.padding.top + this.padding.bottom;

		float max = 0;
		float current;

		foreach (RectTransform r in this.rectChildren)
		{
			if (r.gameObject.activeSelf)
			{
				current = r.rect.height + r.anchoredPosition.y;
				max = Mathf.Max(max, current);
			}
		}

		h += max;
		
		if (this.MinimumLayoutHeight > 0)
			return Mathf.Max(h, this.MinimumLayoutHeight);

		return h;
	}

	/// <summary>
	/// Sets the width of the layout
	/// </summary>
	public override void CalculateLayoutInputVertical()
	{
		widthSum = 0;

		if (layoutMode == Mode.SetElementsWidth)
		{
			if (this.rectChildren.Count > 0)
			{
				float childWidth = (this.rectTransform.rect.width / this.rectChildren.Count) - spacing;
				if (maxChildrenWidth != 0)
					childWidth = Mathf.Min(this.maxChildrenWidth, childWidth);

				widthSum = (this.rectChildren.Count * (childWidth + spacing)) - spacing;
			}
		}
		else
		{
			foreach (RectTransform r in this.rectChildren)
			{
				widthSum += r.rect.width + spacing;
			}

			if (widthSum > spacing)
				widthSum -= spacing;

			if (layoutMode == Mode.SetLayoutWidth)
			{
				this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, widthSum);
			}
		}

		heightSum = this.ComputeWantedHeight();

		if (ArrangeHeight)
		{
			this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				heightSum);

			if (Resized != null)
				Resized(heightSum);
		}
	}

}
