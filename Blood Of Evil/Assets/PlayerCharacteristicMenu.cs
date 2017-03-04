using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    using Entities.Modules.Attributes;
    using Extensions;
    using Helpers;

    public class PlayerCharacteristicMenu : MonoBehaviour
    {
        #region Fields
        private List<PlayerCharacteristicUILine> characteristicsLines = new List<PlayerCharacteristicUILine>();
        [SerializeField]
        private RectTransform window;
        [SerializeField]
        private Text remainPointTextValue;
        [SerializeField]
        private Button applyButton;
        [SerializeField]
        private Button cancelButton;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            this.GenerateLines();

            // Mettra à jour le texte du nombre de point de charactéristique restante.
            this.UpdateRemainTextValue(PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, "Remain Characteristics").
                Current.ValueListenerAndGetValue((value
                    =>
                {
                    this.UpdateRemainTextValue(value);
                })));


        }
        #endregion

        #region Intern Behaviour
        private void GenerateLines()
        {
            for (int characteristicIndex = 0; characteristicIndex < EnumerationHelper.Count<EAttributeCharacteristics>();
                characteristicIndex++)
            {
                GameObject characteristicLineGO = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.
                    AddObjectInPool("Player Characteristic UI Line", transform);
                PlayerCharacteristicUILine characteristicLine =
                    characteristicLineGO.GetComponent<PlayerCharacteristicUILine>();

                characteristicLineGO.GetComponent<RectTransform>().ResetPositionAndScaleForResponsivity();

                characteristicLine.Initialize(EnumerationHelper.IntegerToEnumeration<EAttributeCharacteristics>(characteristicIndex));

                this.characteristicsLines.Add(characteristicLine);
            }

            window.SetHeight(200.0f + 65 * this.characteristicsLines.Count);
        }

        private void UpdateRemainTextValue(float value)
        {
            this.remainPointTextValue.text = Mathf.RoundToInt(value).ToString();
        }
        #endregion
    }
}

// GENERATOR
// Apply || Cancel.ButtonSetActive(PointsAddedNotApplied >= 1);
//Aply -& cancel : multiligne
// Apply :
// for (e < charaNotApplied.length)
// stat += charNoApplied[e];
// charNoApplied[e] = 0;
// Button reset : remet tous les attributs à 0

//Cancel