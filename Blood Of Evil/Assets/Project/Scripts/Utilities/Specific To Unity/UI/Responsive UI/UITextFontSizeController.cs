using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[RequireComponent(typeof(Text)), ExecuteInEditMode]
public class UITextFontSizeController : MonoBehaviour
{
	public RectTransform	customReference = null;
	[Range(0, 1)]
	public float size = 0.05f;
	public bool baseOnWidth = false;
	private RectTransform m_canvasRect = null;
	private RectTransform canvasRect
	{
		get
		{
			if (customReference != null)
				return customReference;
			if (this.m_canvasRect == null)
				this.RefreshCanvasRectSource();
			return this.m_canvasRect;
		}
	}
	private Text m_text;
	private Text text
	{

		get
		{
			if (this.m_text == null)
				this.m_text = this.GetComponent<Text>();
			return this.m_text;
		}
	}
	private float lastRefSize = -1;

	void Start()
	{
		this.Refresh();
	}

	void OnEnable()
	{
		this.Refresh();
	}

#if UNITY_EDITOR
	void Update()
	{
		if (this.lastRefSize != (this.baseOnWidth ? this.canvasRect.rect.width : this.canvasRect.rect.height))
			this.RefreshFontSize();
	}
#else
    private float lastRefresh = -100;

    void Update()
    {
        if (this.lastRefresh + 1 < Time.time)
        {
            if (this.lastRefSize != (this.baseOnWidth ? this.canvasRect.rect.width : this.canvasRect.rect.height))
                this.RefreshFontSize();
            this.lastRefresh = Time.time;
        }
    }
#endif
	public void Refresh()
	{
		this.RefreshCanvasRectSource();
		this.RefreshFontSize();
	}
	private void RefreshFontSize()
	{
		this.lastRefSize = this.baseOnWidth ? this.canvasRect.rect.width : this.canvasRect.rect.height;
		this.text.resizeTextForBestFit = false;
		this.text.fontSize = Mathf.RoundToInt(this.lastRefSize * this.size);
	}
	private void RefreshCanvasRectSource()
	{
		m_canvasRect = this.GetComponent<RectTransform>();

		while (m_canvasRect.parent != null)
		{
			if (canvasRect.parent.GetComponent<RectTransform>() != null)
				m_canvasRect = canvasRect.parent.GetComponent<RectTransform>();
			else
				break;
		}
	}

#if UNITY_EDITOR

	public bool refreshSize = false;
	void OnValidate()
	{
		this.lastRefSize = -1; // Force Refresh when inspector's value changes
		if (this.refreshSize)
		{
			this.Refresh();
			this.refreshSize = false;
		}
	}
#endif
}
