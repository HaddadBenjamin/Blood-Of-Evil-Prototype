using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    using Scene;

    using Player;
    using Utilities.UI;
    using Entities.Modules.Attributes;

    using Services.Audio;
    using Modules.Attributes;
    
    public class GlobButton : AButton
    {
        #region Fields
        [SerializeField]
        private OrbFill orbFill;
        [SerializeField]
        private PlayerAttributesModule attributeModule;
        [SerializeField]
        private EEntityCategoriesAttributes attributeCategory;
        [SerializeField]
        private string attributeID;
        [SerializeField]
        private Text buttonText;
        private bool percentageFormat = false;
        private Image imageToFill;

        private const float NORMALIZED_VALUE_TO_PERCENTAGE = 100.0f;

        private Attribute currentAttribute;
        private Attribute maximumAttribute;
        #endregion

        #region Properties
        #endregion

        #region Override Behaviour
        public override void ButtonActionOnClick()
        {
            this.percentageFormat = !this.percentageFormat;

            this.UdpdateText();

            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
        }
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.currentAttribute = this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current;
            this.maximumAttribute = this.attributeModule.GetAttribute(this.attributeCategory, string.Format("Maximum {0}", this.attributeID)).Current;

            this.currentAttribute.ValueListener(delegate (float value)
            {
                this.UpdateOrbFillAmount();
            });

            this.maximumAttribute.ValueListener(delegate (float value)
            {
                this.UpdateOrbFillAmount();
            });

            this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current.CallOnValueModified();
        }
        #endregion

        #region Intern Behaviour
        private void UpdateOrbFillAmount()
        {
            float current = this.currentAttribute.Value;
            float maximum = this.maximumAttribute.Value;

            this.orbFill.Fill = current / maximum;

            this.UdpdateText();
        }

        private void UdpdateText()
        {
            float current = this.currentAttribute.Value;
            float maximum = this.maximumAttribute.Value;

            this.buttonText.text = this.percentageFormat ?
                string.Format("{0}%", (current / maximum * NORMALIZED_VALUE_TO_PERCENTAGE).ToString("F2")) :
                string.Format("{0} / {1}", Mathf.CeilToInt(current), Mathf.CeilToInt(maximum));
        }
        #endregion
    }
}