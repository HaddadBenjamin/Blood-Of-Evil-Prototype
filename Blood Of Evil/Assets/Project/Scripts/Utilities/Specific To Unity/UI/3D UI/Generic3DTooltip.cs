using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil
{
    using Player.Services.Language;
    using Player.Services.Language.UI;

    namespace Utilities
    {
        namespace UI
        {
            public class Generic3DTooltip : A3DTooltipHolder
            {
                #region Fields
                private UpdateLanguageTextAndAddASuit updateLanguageTextTooltip;
                [SerializeField]
                private string textToTranslate;
                [SerializeField]
                private Color tooltipColor = Color.red;
                [SerializeField]
                private FontStyle tooltipFontStyle = FontStyle.Bold;
                private Text tooltipText;
                [SerializeField]
                private ELanguageCategory languageCategory = ELanguageCategory.Tooltips;
                #endregion

                #region Unity Methods
                void Start()
                {
                    base.Initialize();

                    this.tooltipText = base.tooltilpGameObject.transform.FindChild("Text").GetComponent<Text>();

                    this.updateLanguageTextTooltip = this.tooltipText.GetComponent<UpdateLanguageTextAndAddASuit>();

                    this.updateLanguageTextTooltip.Reinitialize(this.textToTranslate, this.languageCategory);
                }
                #endregion

                #region Unity Methods
                void Update()
                {
                    base.UpdateTooltipPosition();
                }
                #endregion

                #region Public Behaviour
                public void Reinitialize(string textToTranslate, Color tooltipColor)
                {
                    this.textToTranslate = textToTranslate;
                    this.tooltipColor = tooltipColor;

                    if (null != this.updateLanguageTextTooltip)
                        this.updateLanguageTextTooltip.ReinitializeAndUpdateText(this.textToTranslate, this.languageCategory);
                }
                #endregion

                #region Behaviour Methods
                protected override void SetTooltilContent()
                {
                    this.updateLanguageTextTooltip.ReinitializeAndUpdateText(this.textToTranslate, this.languageCategory);

                    this.tooltipText.color = this.tooltipColor;
                    this.tooltipText.fontStyle = this.tooltipFontStyle;
                }
                #endregion
            }
        }
    }
}