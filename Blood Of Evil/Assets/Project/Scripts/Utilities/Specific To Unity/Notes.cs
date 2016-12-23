using UnityEngine;
using System.Collections;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
public class Notes : MonoBehaviour {

#if UNITY_EDITOR
	[Multiline(50)]
	public string text;
#endif

	public void Awake()
	{
		GameObject.Destroy(this.gameObject);
	}
}
