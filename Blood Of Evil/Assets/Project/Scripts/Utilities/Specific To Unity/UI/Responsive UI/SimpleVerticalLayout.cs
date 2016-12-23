using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

/// <summary>
/// Cette classe vient de Manzalab.
/// Vertical layout that doesn't change its children size
/// </summary>
public class SimpleVerticalLayout : LayoutGroup
{
	/// <summary>
	/// If true : changes the height of the layout
	/// </summary>
    public bool ArrangeHeight = false;

	public bool MinimumLayoutHeightFromParent;
	public float MinimumLayoutHeight;
	public float MinimumElementHeight;

	public bool CheckUpdate = false;
	
	/// <summary>
	/// Called when the height of the layout changed
	/// </summary>
    public event Action<float> Resized;

	/// <summary>
	/// Returns the layout rectTransform
	/// </summary>
    public RectTransform rt
    {
        get
        {
            return this.rectTransform;
        }
    }

	/// <summary>
    /// The minimum height this layout element may be allocated.
	/// </summary>
    public override float minHeight
    {
        get
        {
            return heightSum;
        }
    }

	/// <summary>
    ///  The preferred height this layout element should be allocated if there is sufficient space. (0)
	/// </summary>
    public override float preferredHeight
    {
        get
        {
            return 0;
        }
    }

	/// <summary>
    /// The extra relative height this layout element should be allocated if there is additional available space. (0)
	/// </summary>
    public override float flexibleHeight
    {
        get
        {
            return 0;
        }
    }

	/// <summary>
	/// Sum of the children height
	/// </summary>
    private float heightSum;

	/// <summary>
	/// Space between two children
	/// </summary>
    public float spacing = 0;

	/// <summary>
	/// Forces the layout to compute
	/// </summary>
    public void ForceRefresh()
    {
        this.OnEnable();
    }

    // private void Update()
    // {
    // 	if (ForceRefresh)
    // 	{
    // 		if (this.rectTransform.rect.height != ComputeWantedHeight())
    // 			this.OnEnable();
    // 	}
    // }
	
	/// <summary>
	/// Returns the layout wanted size
	/// </summary>
	/// <returns>The layout wanted size</returns>
    private float ComputeWantedHeight()
    {
        float h = this.padding.top + this.padding.bottom;
		bool one = false;

        foreach (RectTransform r in this.rectChildren)
        {
			if (r.gameObject.activeSelf)
			{
				if (MinimumElementHeight > 0)
					h += Mathf.Max(MinimumElementHeight, r.rect.height) + spacing;
				else
					h += r.rect.height + spacing;
				one = true;
			}
        }

		if(one)
			h -= spacing;

		if(MinimumLayoutHeightFromParent)
			return Mathf.Max(h, (this.transform.parent as RectTransform).rect.height);

		else if (this.MinimumLayoutHeight > 0)
			return Mathf.Max(h, this.MinimumLayoutHeight);

        return h;
    }

	/// <summary>
	/// Sets the layout height
	/// </summary>
    public override void CalculateLayoutInputVertical()
    {
        // The minHeight, preferredHeight, and flexibleHeight values may be calculated in this callback.

        heightSum = this.ComputeWantedHeight();

        if (ArrangeHeight)
        {
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                heightSum);

            if (Resized != null)
                Resized(heightSum);
        }
    }

	/// <summary>
	/// Does nothing
	/// </summary>
    public override void SetLayoutHorizontal()
    {
        // Callback invoked by the auto layout system which handles horizontal aspects of the layout.


    }

	/// <summary>
	/// Sets the children position
	/// </summary>
    public override void SetLayoutVertical()
    {
        // Callback invoked by the auto layout system which handles vertical aspects of the layout.

        float y = -padding.top;
        foreach (RectTransform r in this.rectChildren)
        {
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, y);
            r.pivot = new Vector2(r.pivot.x, 1);
            r.anchorMin = new Vector2(r.anchorMin.x, 1);
            r.anchorMax = new Vector2(r.anchorMax.x, 1);

			if (MinimumElementHeight > 0)
				y -= Mathf.Max(MinimumElementHeight, r.rect.height) + spacing;
			else
				y -= r.rect.height + spacing;
        }
    }

	void Update()
	{
		if (CheckUpdate)
			ForceRefresh();
	}
}
