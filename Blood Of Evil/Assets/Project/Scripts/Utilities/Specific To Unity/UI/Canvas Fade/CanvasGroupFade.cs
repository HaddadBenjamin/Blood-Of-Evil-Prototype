using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Canvas))]
public class CanvasGroupFade : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// C'est le canvas group de l'objet actuel, il permet de gérer la transparence du widget ainsi que ceux de ses enfants.
    /// </summary>
    private CanvasGroup canvasGroup;
    [SerializeField, Tooltip("C'est la vitesse de fade in fade out.")]
    private float fadeSpeed = 1.0f;

    /// <summary>
    /// C'est le mode de fade, il peut être en fade in, en fade out, ou rien du tout.
    /// </summary>
    private EFadeMode fadeMode;

    /// <summary>
    /// C'est l'évênement qui est appelé lorsqu'un effet de fade in vient de se terminer.
    /// </summary>
    public Action OnFadeInFinish;
    /// <summary>
    /// C'est l'évênement qui est appelé lorsqu'un effet de fade out vient de se terminer.
    /// </summary>
    public Action OnFadeOutFinish;
    #endregion

    #region Properties
    /// <summary>
    /// Permet de récupérer la valeur de transparence du widget.
    /// </summary>
    public float Alpha
    {
        get { return this.canvasGroup.alpha; }
    }

    /// <summary>
    /// Permet de récupérer le mode de fade courant.
    /// </summary>
    public EFadeMode Mode
    {
        get { return this.fadeMode; }
    }
    #endregion

    #region Unity Behaviour
    private void Awake()
    {
        /// Récupère le composant permettant de gérer la transpanrence du widget.
        this.canvasGroup = GetComponent<CanvasGroup>();
    }
    #endregion

    #region Public Behaviour
    /// <summary>
    /// Modifie la valeur de transpanrence du widget.
    /// </summary>
    public void SetAlpha(float alpha)
    {
        this.canvasGroup.alpha = alpha;
    }

    /// <summary>
    /// Modifie la vitesse de fade in fade out.
    /// </summary>
    public void SetSpeed(float speed)
    {
        this.fadeSpeed = speed;
    }

    /// <summary>
    /// Lance un effet de fade in.
    /// </summary>
    public void FadeIn()
    {
        this.fadeMode = EFadeMode.FadeIn;

        StartCoroutine(this.FadeInCoroutine());
    }

    /// <summary>
    /// Lance un effet de fade out.
    /// </summary>
    public void FadeOut()
    {
        this.fadeMode = EFadeMode.FadeOut;

        StartCoroutine(this.FadeOutCoroutine());
    }

    /// <summary>
    /// Arrête l'effet de fade actuel.
    /// </summary>
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

        while (this.canvasGroup.alpha > 0.0f &&
               EFadeMode.FadeOut == this.fadeMode)
        {
            this.canvasGroup.alpha -= Time.deltaTime * this.fadeSpeed;

            yield return new WaitForEndOfFrame();
        }

        this.OnFadeOutFinish.SafeCall();
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return null;

        while (this.canvasGroup.alpha < 1.0f &&
               EFadeMode.FadeIn == this.fadeMode)
        {
            this.canvasGroup.alpha += Time.deltaTime * this.fadeSpeed;

            yield return new WaitForEndOfFrame();
        }

        this.canvasGroup.interactable = false;

        this.OnFadeInFinish.SafeCall();
    }
    #endregion
}
