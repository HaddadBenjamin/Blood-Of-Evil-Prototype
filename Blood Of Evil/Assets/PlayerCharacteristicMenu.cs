using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BloodOfEvil.Extensions;
using BloodOfEvil.Helpers;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    public class PlayerCharacteristicMenu : MonoBehaviour
    {
        #region Fields
        private List<PlayerCharacteristicUILine> characteristicsLines = new List<PlayerCharacteristicUILine>();
        #endregion

        #region Unity Behaviour
        private void Start()
        {
            this.GenerateLines();
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
        }
        #endregion
    }
}