using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BloodOfEvil.Entities.Modules.Attributes;
using BloodOfEvil.Helpers;
using BloodOfEvil.ObjectInScene;
using BloodOfEvil.Player.Services.Language.UI;
using BloodOfEvil.Scene;

namespace BloodOfEvil.Player.Modules.Attributes.UI
{
    public class PlayerCharacteristicUILine : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est le nom de la characteristique, exemple : force.")]
        private UpdateLanguageTextAndAddASuit idText;

        [SerializeField, Tooltip("C'est la valeur de la charactéristique, exemple : 322.")]
        private Text textValue;

        [SerializeField, Tooltip("Permet de rajouter un point dans cette charactéristique.")]
        private Button addButton;

        [SerializeField, Tooltip("Permet d'enlever un point dans cette charactéristique.")]
        private Button removeButton;

        private EAttributeCharacteristics characteristicID;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            // Comportement du bouton -.
            this.AddRemoveButtonListener();

            this.RemoveButtonAddListener();
        }
        #endregion

        #region Public Behaviour
        public void Initialize(EAttributeCharacteristics characteristic)
        {
            this.characteristicID = characteristic;

            this.idText.UpdateDefaultTextThenUpdateText(this.GetCharacteristicName());

            Debug.Log(this.GetCharacteristicName());
            // Met à le texte de charactéristique "strength" par exemple en fonction de la langue choisie par l'utilisateur.
            this.UpdateTextValue(
                PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, this.GetCharacteristicName()).
                    Current.ValueListenerAndGetValue((float value) =>
                    {
                       this.UpdateTextValue(value + PlayerServicesAndModulesContainer.Instance.AttributesModule.GetCharacteristicsPointsAddedButNotApplied(
                        this.characteristicID)); 
                    }) + PlayerServicesAndModulesContainer.Instance.AttributesModule.GetCharacteristicsPointsAddedButNotApplied(
                        this.characteristicID));
        }
        #endregion

        #region Intern Behaviour
        private string GetCharacteristicName()
        {
            return EnumerationHelper.EnumerationToString(this.characteristicID);
        }

        private void UpdateTextValue(float value)
        {
            this.textValue.text = Mathf.RoundToInt(value).ToString();
        }

        #region Buttons Behaviour
        /// <summary>
        /// Comportement du bouton pour enlever 1 dans une charactéristique.
        /// </summary>
        private void RemoveButtonAddListener()
        {
            this.addButton.onClick.AddListener(() =>
            {
                var remainAttribute = PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, "Remain Characteristics");

                if (remainAttribute.Current.Value >= 1.0f)
                {
                    PlayerServicesAndModulesContainer.Instance.AttributesModule.AddCharacteristicsPointsAddedButNotApplied(
                        this.characteristicID);

                    remainAttribute.SetOtherAttributeLessEqual("Level Up Menu", 1);

                    PlayerServicesAndModulesContainer.Instance.AttributesModule.SaveCharacteristicsPointsAddedButNotApplied();

                    this.textValue.text = (Int32.Parse(this.textValue.text) + 1).ToString();
                }
            });
        }

        /// <summary>
        /// Comportement du bouton pour ajouter  1 dans une charactéristique.
        /// </summary>
        private void AddRemoveButtonListener()
        {
            this.removeButton.onClick.AddListener(() =>
            {
                var remainAttribute = PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, "Remain Characteristics");

                if (
                    PlayerServicesAndModulesContainer.Instance.AttributesModule.GetCharacteristicsPointsAddedButNotApplied(
                        this.characteristicID) >= 1.0f)
                {
                    PlayerServicesAndModulesContainer.Instance.AttributesModule.
                        RemoveCharacteristicsPointsAddedButNotApplied(this.characteristicID);

                    remainAttribute.SetOtherAttributePlusEqual("Level Up Menu", 1);

                    PlayerServicesAndModulesContainer.Instance.AttributesModule.SaveCharacteristicsPointsAddedButNotApplied();

                    this.textValue.text = (Int32.Parse(this.textValue.text) - 1).ToString();
                }
            });
        }
        #endregion
        #endregion
    }
}


// MEttre à jour les textes (le nombre) lors d'un clic sur + ou -

// Tooltip
// Multilangue.

// plusButton.SetActive(RemainPoint >1);
// minusButton.SetActive(GetElementNotAppliedInThisCategory() > 1)



//11 Attributs :
//Wisdom,           3% energy.                  : V
//Faith,            1-2 heal, 3% heal           : V 
//Resistance,       10 defence, 2% defence      : V
//Chance,           0.5% CC, 3% item find       : V
//Endurance,        25 life, 2% life            : V
//Power,            15 energy, 2% skill power   : V
//Dexterity,        15 accuracy, 2% accuracy    : V
//Spirit,           0.5 all resistances, 1.5% mana : V
//Constitution,     3% pet life and damage      : V


// Did list :
// Mettre la classe PlayerCharacteristicPointAdded dans le module d'attributs du joueur.
// Sauvegarder ou charger la classe PlayerCharacteristicPointAdded après avoir sauvegarder ou charger le module d'attributs du joueur.
