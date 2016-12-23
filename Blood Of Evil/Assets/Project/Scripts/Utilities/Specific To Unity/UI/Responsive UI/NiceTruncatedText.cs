using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Cette classe vient de Manzalab.
/// </summary>
[RequireComponent(typeof(Text))]
public class NiceTruncatedText : UIBehaviour 
{
	public string wantedText = "Ceci est un test funky qui a du groove !!";
	public string ellipsis = "...";

#if UNITY_EDITOR
	public bool refreshEditor;
#endif

	public Text GetText()
	{
		return GetComponent<Text>();
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
 		base.OnValidate();
		if(refreshEditor)
			this.Refresh();
		refreshEditor = false;
	}
#endif

	public void Refresh()
	{
		Text txt = this.GetText();

		if (txt)
		{
			txt.text = wantedText;

			if (txt.preferredWidth > txt.rectTransform.rect.width)
			{
				string replacement = wantedText;
				txt.text = replacement + ellipsis;

				while (txt.preferredWidth > txt.rectTransform.rect.width)
				{
					replacement = replacement.Remove(replacement.Length - 1);
					txt.text = replacement.Trim() + ellipsis;
				}
			}
		}
	}
}
