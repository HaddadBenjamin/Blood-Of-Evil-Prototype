using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    using Scene;
    using Entities.Modules.Attributes;


    using Services.Audio;
    using Modules.Attributes;


    public class ResourcePlayerFillAmount : MonoBehaviour
    {
        #region Fields
        private OrbFill orbFill;
        private PlayerAttributesModule attributeModule;
        [SerializeField]
        private EEntityCategoriesAttributes attributeCategory;
        [SerializeField]
        private string attributeID;
        [SerializeField]
        private Text text;
        [SerializeField]
        private Button buttonFormat;
        private bool percentageFormat = false;
        private Image imageToFill;

        private const float NORMALIZED_VALUE_TO_PERCENTAGE = 100.0f;

        public bool containAnOrb = false;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            if (this.containAnOrb)
                this.orbFill = GetComponent<OrbFill>();
            else
                this.imageToFill = GetComponent<Image>();

            this.attributeModule = PlayerServicesAndModulesContainer.Instance.AttributesModule;

            this.buttonFormat.onClick.AddListener(delegate ()
            {
                this.percentageFormat = !this.percentageFormat;

                this.UdpdateText();

                SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, "Click Button");
            });

            this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current.ValueListener(delegate (float value)
            {
                this.UpdateOrbFillAmount();
            });

            this.attributeModule.GetAttribute(this.attributeCategory, string.Format("Maximum {0}", this.attributeID)).Current.ValueListener(delegate (float value)
          {
              this.UpdateOrbFillAmount();
          });

            this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current.CallOnValueModified();
        }
        #endregion

        #region Intern Behaviour
        private void UpdateOrbFillAmount()
        {
            float current = this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current.Value;
            float maximum = this.attributeModule.GetAttribute(this.attributeCategory, string.Format("Maximum {0}", this.attributeID)).Current.Value;

            if (this.containAnOrb)
                this.orbFill.Fill = current / maximum;
            else
                this.imageToFill.fillAmount = current / maximum;

            this.UdpdateText();
        }

        private void UdpdateText()
        {
            float current = this.attributeModule.GetAttribute(this.attributeCategory, this.attributeID).Current.Value;
            float maximum = this.attributeModule.GetAttribute(this.attributeCategory, string.Format("Maximum {0}", this.attributeID)).Current.Value;

            this.text.text = this.percentageFormat ?
                string.Format("{0}%", (current / maximum * NORMALIZED_VALUE_TO_PERCENTAGE).ToString("F2")) :
                string.Format("{0} / {1}", Mathf.CeilToInt(current), Mathf.CeilToInt(maximum));
        }
        #endregion
    }
}