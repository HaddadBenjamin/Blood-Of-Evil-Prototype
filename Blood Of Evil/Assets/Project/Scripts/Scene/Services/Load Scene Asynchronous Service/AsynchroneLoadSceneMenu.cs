using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AsynchroneLoadSceneMenu : MonoBehaviour
{
    #region Fields
    [Header("Les textes du menu :")]
    [SerializeField, Tooltip("C'est le texte de chargement.")]
    private Text loadingText;
    [SerializeField]
    private Text loadingPercentageText;

    [Header("Les images du menu :")]
    [SerializeField, Tooltip("C'est la barre de progression à remplir.")]
    private Image progressBarImage;

    [Header("Intéractions du canvas :")]
    [SerializeField, Tooltip("Permet de modifier la transaprence du menu.")]
    private CanvasGroupFade menuCanvasGroup;
    [SerializeField, Tooltip("Permet de modifier la possibilité d'intéragir ou non sur le menu.")]
    private GraphicRaycaster menuGrapicCaster;

    private bool enableDownload;
    #endregion

    #region Properties
    public bool EnableDownload
    {
        get { return enableDownload; }

        set
        {
            enableDownload = value;

            if (enableDownload)
                this.EnableMenu();
            else
                DisableMenu();
        }
    }
    #endregion

    #region Unity Behaviour
    private void Update()
    {
        if (this.enableDownload)
        {
            this.UpdateProgressBar(
                this.progressBarImage.fillAmount < 0.9f ?
                    this.progressBarImage.fillAmount + Time.deltaTime :
                    1.0f);
        }

        this.ShowAndUnshowLoadingText();
    }
    #endregion

    #region Unity Behaviour
    /// <summary>
    /// Fait apparaître le menu et lui active la possibilité d'intéragir dessus.
    /// </summary>
    private void EnableMenu()
    {
        this.menuCanvasGroup.FadeIn();

        this.menuGrapicCaster.enabled = true;
    }

    /// <summary>
    /// Fait disparaître le menu et lui active la possibilité d'intéragir dessus.
    /// </summary>
    private void DisableMenu()
    {
        this.menuCanvasGroup.FadeOut();

        this.menuGrapicCaster.enabled = false;
    }

    /// <summary>
    /// Met à jour le remplissage de la barre de progression et son texte affiché en pourcentage.
    /// </summary>
    private void UpdateProgressBar(float fillAmount)
    {
        if (fillAmount > 0.99f)
            fillAmount = 1.0f;

        if (this.EnableDownload &&
            this.progressBarImage.fillAmount > 0.99f)
            this.EnableDownload = false;

        this.progressBarImage.fillAmount = fillAmount < 0.99f ?
            Mathf.Lerp(this.progressBarImage.fillAmount, fillAmount, Time.deltaTime * 10.0f) :
            1.0f;

        this.loadingPercentageText.text = (fillAmount * 100.0f).ToString("F2") + "%";
    }

    /// <summary>
    /// Fait apparaître puis disparaître le texte de chargement.
    /// </summary>
    private void ShowAndUnshowLoadingText()
    {
        var color = this.loadingText.color;

        this.loadingText.color = new Color(color.r, color.g, color.g, Mathf.PingPong(Time.time, 1.0f));
    }
    #endregion
}