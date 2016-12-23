using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
public class HorizontalResizeAs : UIBehaviour, ILayoutElement
{
	public RectTransform rt;
	public float maxSize;
	public bool CheckUpdate;
	
	public float preferredHeight
	{
		get
		{
			if (!rt)
				return 0;
			return Mathf.Min(rt.rect.height, maxSize);
		}
	}

	public float minWidth
	{
		get
		{
			return 0;
		}
	}

	public float preferredWidth
	{
		get
		{
			return 0;
		}
	}

	public float flexibleWidth
	{
		get
		{
			return 0;
		}
	}

	public float minHeight
	{
		get
		{
			return 0;
		}
	}

	public float flexibleHeight
	{
		get
		{
			return 0;
		}
	}

	public int layoutPriority
	{
		get
		{
			return 0;
		}
	}

	public void CalculateLayoutInputVertical()
	{
		if(rt)
		{
			(this.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				preferredHeight);

			ScrollRect r = this.GetComponent<ScrollRect>();
			if (r)
				r.CalculateLayoutInputVertical();
		}
	}

	public void ForceRefresh()
	{
		CalculateLayoutInputVertical();
	}

	void Update()
	{
		if (CheckUpdate)
			ForceRefresh();
	}

	public void CalculateLayoutInputHorizontal()
	{
		
	}
}
