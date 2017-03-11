using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class CanvasGroupFade : MonoBehaviour
{
    #region Fields
    private CanvasGroup canvasGroup;
    [SerializeField]
    public float fadeSpeed = 1.0f;

    private EFadeMode fadeMode;
    #endregion

    #region Unity Behaviour
    private void Awake()
    {
        this.canvasGroup = GetComponent<CanvasGroup>();
    }
    #endregion

    #region Public Behaviour
    public void FadeIn()
    {
        this.fadeMode = EFadeMode.FadeIn;

        StartCoroutine(this.FadeInCoroutine());
    }

    public void FadeOut()
    {
        this.fadeMode = EFadeMode.FadeOut;

        StartCoroutine(this.FadeOutCoroutine());
    }

    public void Stop()
    {
        this.fadeMode = EFadeMode.NoFade;
    }
    #endregion

    #region Intern Behaviour
    private IEnumerator FadeOutCoroutine()
    {
        yield return null;

        this.canvasGroup.interactable = true;

        while (this.canvasGroup.alpha < 1.0f &&
               EFadeMode.FadeOut == this.fadeMode)
        {
            this.canvasGroup.alpha += Time.deltaTime * this.fadeSpeed;

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return null;

        while (this.canvasGroup.alpha > 0.0f &&
               EFadeMode.FadeIn == this.fadeMode)
        {
            this.canvasGroup.alpha -= Time.deltaTime * this.fadeSpeed;

            yield return new WaitForEndOfFrame();
        }

        this.canvasGroup.interactable = false;
    }
    #endregion
}
