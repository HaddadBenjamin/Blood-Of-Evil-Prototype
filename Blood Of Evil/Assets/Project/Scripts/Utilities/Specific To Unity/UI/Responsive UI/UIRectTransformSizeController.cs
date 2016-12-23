using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[RequireComponent(typeof(RectTransform)), ExecuteInEditMode]
public class UIRectTransformSizeController : MonoBehaviour, ILayoutElement, ILayoutSelfController
{
	public Vector2			referenceSizeAjust = Vector2.zero;

	[Header("Width")]
	public bool				applyOnWidth = false;
	public RectTransform	customWidthReference = null;
	[Range(0, 2)]
	public float			widthSize = 0.05f;
	public float			widthPositionOffset = 0f;
	public bool				widthBasedOnWidth = false;
	public bool				substractWidth = false;

	[Header("Height")]
	public bool				applyOnHeigth = false;
	public RectTransform	customHeightReference = null;
	[Range(0, 2)]
	public float			heightSize = 0.05f;
	public float			heightPositionOffset = 0f;
	public bool				heightBasedOnHeight = false;
	public bool				substractHeight = false;

	public bool refreshButton = false;

	private Transform lastParent = null;
	private RectTransform m_canvasRectWidth = null;
	private RectTransform canvasRectWidth
	{
		get
		{
			if (customWidthReference != null)
				return customWidthReference;
			if (m_canvasRectWidth == null || lastParent != transform.parent)
			{
				m_canvasRectWidth = GetComponent<RectTransform>();

				while (m_canvasRectWidth.parent != null)
				{
					if (m_canvasRectWidth.parent.GetComponent<RectTransform>() != null)
						m_canvasRectWidth = m_canvasRectWidth.parent.GetComponent<RectTransform>();
					else
						break;
				}
				lastParent = transform.parent;
			}
			return m_canvasRectWidth;
		}
	}
	private RectTransform m_canvasRectHeight = null;
	private RectTransform canvasRectHeight
	{
		get
		{
			if (customHeightReference != null)
				return customHeightReference;
			if (m_canvasRectHeight == null || lastParent != transform.parent)
			{
				m_canvasRectHeight = GetComponent<RectTransform>();

				while (m_canvasRectHeight.parent != null)
				{
					if (m_canvasRectHeight.parent.GetComponent<RectTransform>() != null)
						m_canvasRectHeight = m_canvasRectHeight.parent.GetComponent<RectTransform>();
					else
						break;
				}
				lastParent = transform.parent;
			}
			return m_canvasRectHeight;
		}
	}
	private RectTransform m_rectTransform;
	private RectTransform rectTransform
	{

		get
		{
			if (m_rectTransform == null)
				m_rectTransform = GetComponent<RectTransform>();
			return m_rectTransform;
		}
	}
	private float lastRefWidthSize = -1;
	private float lastRefHeightSize = -1;

	void OnEnable()
	{
		Refresh();
	}
	private float lastRefresh = -100;
	float GetRefWidthSize()
	{
		return (widthBasedOnWidth ? canvasRectWidth.rect.width : canvasRectWidth.rect.height);
	}
	float GetRefHeightSize()
	{
		return (heightBasedOnHeight ? canvasRectHeight.rect.width : canvasRectHeight.rect.height);
	}

	public void Refresh()
	{
		lastRefWidthSize = GetRefWidthSize();
		lastRefHeightSize = GetRefHeightSize();
		rectTransform.sizeDelta = new Vector2(applyOnWidth ? (substractWidth ? -1 : 1) * lastRefWidthSize * widthSize : rectTransform.sizeDelta.x,
											  applyOnHeigth ? (substractHeight ? -1 : 1) * lastRefHeightSize * heightSize : rectTransform.sizeDelta.y);
		rectTransform.anchoredPosition = new Vector2(applyOnWidth && widthPositionOffset != 0 ? widthPositionOffset * lastRefWidthSize : rectTransform.anchoredPosition.x,
													 applyOnHeigth && heightPositionOffset != 0 ? heightPositionOffset * lastRefHeightSize : rectTransform.anchoredPosition.y);
	}

	void Start()
	{
		lastRefWidthSize = -1; // Force Refresh when inspector's value changes
		lastRefHeightSize = -1; // Force Refresh when inspector's value changes
		m_canvasRectWidth = null;
		m_canvasRectHeight = null;
	}
	void Update()
	{
		if (!Application.isPlaying || lastRefresh + 1 < Time.time)
		{
			if (lastRefWidthSize != GetRefWidthSize() || lastRefHeightSize != GetRefHeightSize())
				Refresh();
			lastRefresh = Time.time;
		}
	}
#if UNITY_EDITOR
	void OnValidate()
	{
		lastRefWidthSize = -1; // Force Refresh when inspector's value changes
		lastRefHeightSize = -1; // Force Refresh when inspector's value changes
		if (refreshButton)
		{
			refreshButton = false;
			m_canvasRectWidth = null;
			m_canvasRectHeight = null;
		}
	}
#endif

	void ILayoutElement.CalculateLayoutInputHorizontal()
	{
		Refresh();
	}

	void ILayoutElement.CalculateLayoutInputVertical()
	{
		Refresh();
	}

	int ILayoutElement.layoutPriority
	{
		get { return 1; }
	}

	float ILayoutElement.minHeight
	{
		get { return rectTransform.sizeDelta.y; }
	}

	float ILayoutElement.minWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}

	float ILayoutElement.preferredHeight
	{
		get
		{
			return rectTransform.sizeDelta.y;
		}
	}

	float ILayoutElement.preferredWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}


	float ILayoutElement.flexibleHeight
	{
		get { return rectTransform.sizeDelta.y; }
	}

	float ILayoutElement.flexibleWidth
	{
		get { return rectTransform.sizeDelta.x; }
	}

	void ILayoutController.SetLayoutHorizontal()
	{
		if (applyOnWidth)
			Refresh();
	}

	void ILayoutController.SetLayoutVertical()
	{
		if (applyOnHeigth)
			Refresh();
	}
}
