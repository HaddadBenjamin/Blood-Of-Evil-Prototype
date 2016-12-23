using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[RequireComponent(typeof(Text))]
public class adaptTextHeight : MonoBehaviour {

    public bool forceUpdate;
	public bool adaptOnUpdate = true;

    public float min;
	public float offset;

	public RectTransform toUpdate;
	
	void LateUpdate ()
	{
		if(adaptOnUpdate)
		{
			Adapt();
		}
	}

	public float Adapt()
	{
		Text t = GetComponent<Text>();

		if (toUpdate == null)
			toUpdate = t.rectTransform;

		float h = Mathf.Max(min, t.preferredHeight) + offset;

		toUpdate.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		return h;
	}
	
#if UNITY_EDITOR
    void OnValidate()
    {
        forceUpdate = false;
		Adapt();
    }
#endif
}
