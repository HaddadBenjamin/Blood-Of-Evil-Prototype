using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[ExecuteInEditMode]
public class resizeGridLayoutGroup : MonoBehaviour {

	public bool forceValidate;
	public bool refreshOnUpdate;

	void Update()
	{
		if (refreshOnUpdate)
			Refresh();
	}

	void OnValidate()
	{
		forceValidate = false;
		Refresh();
	}

	public void Refresh()
	{
		GridLayoutGroup layout = GetComponent<GridLayoutGroup>();
		if (layout && layout.constraintCount == 1 && layout.constraint != GridLayoutGroup.Constraint.Flexible)
		{
			bool first;
			float width = layout.padding.left + layout.padding.right;
			float height = layout.padding.top + layout.padding.bottom;

			first = true;
			foreach (Transform t in layout.transform)
			{
				RectTransform rt = t as RectTransform;
				if (rt)
				{
					LayoutElement e = rt.GetComponent<LayoutElement>();
					if(e != null && e.ignoreLayout)
						continue;

					if (!rt.gameObject.activeSelf)
						continue;

					if (!first)
					{
						if (layout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
							height += layout.spacing.y;
						else
							width += layout.spacing.x;
					}
				
					if (layout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
						height += layout.cellSize.y;
					else
						width += layout.cellSize.x;
				}
			}

			if (layout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
				width += layout.cellSize.x;
			else
				height += layout.cellSize.y;
				
			(layout.transform as RectTransform).SetSizeWithCurrentAnchors(
					RectTransform.Axis.Horizontal, width);
			(layout.transform as RectTransform).SetSizeWithCurrentAnchors(
				RectTransform.Axis.Vertical, height);
		}
	}
}
