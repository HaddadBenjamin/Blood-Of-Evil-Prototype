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
    public class CharacteristicUILine : MonoBehaviour
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
        
        #endregion

        #region Public Behaviour
        public void Initialize(EAttributeCharacteristics characteristic)
        {
            this.characteristicID = characteristic;

            this.idText.UpdateDefaultText(this.GetCharacteristicName());

            this.UpdateTextValue(
                PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, this.GetCharacteristicName()).
                    Current.ValueListenerAndGetValue((float value) =>
                    {
                       this.UpdateTextValue(value); 
                    }));

            this.addButton.onClick.AddListener(() =>
            {
                float remainPoints = PlayerServicesAndModulesContainer.Instance.AttributesModule.GetAttribute(
                    EEntityCategoriesAttributes.Characteristics, "Remain Characteristics").Current.Value;

                //if (remainPoints > 1)

                // if (remainPoint > 1)
                // Add 1 in temporary attribute
                // remainPoint -1 = 1.

                // - : if (temporaryAttribute > 0)
                // -1 temporaryAttriute
                // temporaryAttribute--;

                // Apply.
                // Cancel
            });
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
        #endregion
    }
}

namespace BloodOfEvil.Player.Modules.Attributes
{
    /// <summary>
    /// Ce sont tous les points de charactéristiques non appliquées par le joueur lors d'une monté de niveau.
    /// </summary>
    [System.Serializable]
    public class PlayerCharacteristicsPointsAddedButNotApplied
    { 
        #region Fields
        /// <summary>
        /// Ce sont les points distribués dans les charactéristiques du joueur mais qui n'ont pas été appliqués.
        /// </summary>
        public float[] Datas = new float[EnumerationHelper.Count<EAttributeCharacteristics>()];
        #endregion

        #region Constructor
        public PlayerCharacteristicsPointsAddedButNotApplied() { }
        #endregion

        #region Intern Behaviour
        private string GetFilename()
        {
            return
                SceneServicesContainer.Instance.FileSystemConfiguration.
                    CharacteristicsPointsAddedTemporaryPointsAddedFilename;
        }
        #endregion
    }
}

// Mettre la classe PlayerCharacteristicPointAdded dans le module d'attributs du joueur.
// Sauvegarder ou charger la classe PlayerCharacteristicPointAdded après avoir sauvegarder ou charger le module d'attributs du joueur.

// Audio sur les boutons
// Tooltip
// Multilangue.

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