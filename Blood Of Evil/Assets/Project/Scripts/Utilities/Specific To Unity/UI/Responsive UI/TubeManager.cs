using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class TubeManager : LayoutGroup
{
	public bool UpdateColor = false;
	public Color color = Color.white;
	private Color prev;

	public override void CalculateLayoutInputVertical()
	{

	}

	public override void SetLayoutHorizontal()
	{
		if (this.rectChildren.Count == 3)
		{
			RectTransform left = this.rectChildren[0];
			RectTransform center = this.rectChildren[1];
			RectTransform right = this.rectChildren[2];
			
			left.anchorMin = new Vector2(0, 0.5f);
			left.anchorMax = new Vector2(0, 0.5f);
			left.pivot = new Vector2(0.5f, 0.5f);
			left.sizeDelta = Vector2.one * this.rectTransform.rect.height;
			left.anchoredPosition = Vector3.zero;
			left.localScale = Vector3.one;
			
			center.anchorMin = new Vector2(0, 0);
			center.anchorMax = new Vector2(1, 1);
			center.pivot = new Vector2(0.5f, 0.5f);
			center.sizeDelta = -Vector2.right * this.rectTransform.rect.height;
			center.anchoredPosition = Vector3.zero;
			center.localScale = Vector3.one;

			right.anchorMin = new Vector2(1, 0.5f);
			right.anchorMax = new Vector2(1, 0.5f);
			right.pivot = new Vector2(0.5f, 0.5f);
			right.sizeDelta = Vector2.one * this.rectTransform.rect.height;
			right.anchoredPosition = Vector3.zero;
			right.localScale = Vector3.one;
			
			m_Tracker.Add(this, left, DrivenTransformProperties.All);
			m_Tracker.Add(this, center, DrivenTransformProperties.All);
			m_Tracker.Add(this, right, DrivenTransformProperties.All);
		}
	}

	public override void SetLayoutVertical()
	{

	}

	void Update()
	{
		if (color != prev && UpdateColor)
		{
			prev = color;
			foreach (RectTransform rt in rectChildren)
			{
				Graphic [] graphics = rt.GetComponents<Graphic>();
				foreach (Graphic g in graphics)
					g.color = color;
			}
		}
	}
}
