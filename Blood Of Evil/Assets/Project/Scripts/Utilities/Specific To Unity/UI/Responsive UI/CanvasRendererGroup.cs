using UnityEngine;
using System.Collections;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
public class CanvasRendererGroup : MonoBehaviour
{
	[Range(0, 1), SerializeField]
	private float m_alpha = 1;
	private float applied_alpha
	{
		get
		{
			if (canvasArray != null && canvasArray.Length > 0)
				return canvasArray[0].GetAlpha();
			return 1;
		}
	}

	public float alpha
	{
		set
		{
			m_color.a = value;
			if (this.canvasArray != null)
			{
				for (int i = 0; i < this.canvasArray.Length; i++)
					this.canvasArray[i].SetAlpha(value);
			}
			if (value != this.m_alpha)
				this.m_alpha = value;
		}
	}


	[SerializeField]
	private Color m_color = Color.white;
	private Color applied_color
	{
		get
		{
			if (canvasArray != null && canvasArray.Length > 0)
				return canvasArray[0].GetColor();
			return Color.white;
		}
	}
	public Color color
	{
		get
		{
			return m_color;
		}
		set
		{
			m_alpha = value.a;
			if (this.canvasArray != null)
			{
				for (int i = 0; i < this.canvasArray.Length; i++)
					this.canvasArray[i].SetColor(value);
			}
			if (value != this.m_color)
				this.m_color = value;
		}
	}

	public CanvasRenderer[] canvasArray;

	void Awake()
	{
		this.canvasArray = this.transform.GetComponentsInChildren<CanvasRenderer>();
	}
	void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Awake();
			if (m_alpha != applied_alpha)
				this.alpha = this.m_alpha;
			if (m_color != applied_color)
				this.color = this.m_color;
		}
	}
	void Start()
	{
		OnEnable();
	}
	void OnEnable()
	{
		this.alpha = this.m_alpha;
		this.color = this.m_color;
	}
	void Update()
	{
		if (this.m_alpha != this.applied_alpha)
			this.alpha = this.m_alpha;
		if (this.m_color != this.applied_color)
			this.color = this.m_color;
	}
}
