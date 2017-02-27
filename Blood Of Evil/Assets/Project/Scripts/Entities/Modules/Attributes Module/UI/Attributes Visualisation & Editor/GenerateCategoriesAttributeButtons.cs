using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Helpers;
    using Player;

    public class GenerateCategoriesAttributeButtons : MonoBehaviour
    {
        #region Fields
        private Transform myTransform;
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.myTransform = transform;

            this.GenerateAttributeCategoriesButtons();
        }
        #endregion

        #region Intern Behaviour
        private void GenerateAttributeCategoriesButtons()
        {
            for (int i = 0; i < EnumerationHelper.EnumerationToEnumerationValuesArray<EEntityCategoriesAttributes>().Length; i++)
            {
                PlayerServicesAndModulesContainer.Instance.PrefabReferencesService.Instantiate("Category Attribute Button", this.myTransform, responsive: true).
                    GetComponent<CategoryAttributeButton>().
                    Initialize(EnumerationHelper.EnumerationToEnumerationValuesArray<EEntityCategoriesAttributes>()[i]);
            }
        }
        #endregion
    }
}