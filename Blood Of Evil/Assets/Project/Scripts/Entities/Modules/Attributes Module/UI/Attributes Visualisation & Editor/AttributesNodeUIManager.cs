using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Scene;
    using Scene.Services.ObjectPool;

    using Helpers;
    using Entities.Modules.Attributes;

    using Player;
    using Player.Modules.Attributes;


    public class AttributesNodeUIManager : MonoBehaviour
    {
        #region Fields
        private ObjectsPool pool;
        private EEntityCategoriesAttributes categoryAttributeSelected;
        private Transform attributeNodeUIContentTransform;
        #endregion

        #region Properties
        public EEntityCategoriesAttributes CategoryAttributeSelected
        {
            get { return categoryAttributeSelected; }
            set
            {
                this.categoryAttributeSelected = value;

                this.UpdateAttributeCategory();
            }
        }
        #endregion

        #region Unity Behaviour
        void Start()
        {
            this.attributeNodeUIContentTransform = PlayerServicesAndModulesContainer.Instance.GameObjectInSceneReferencesService.Get("Editor Category - Category Content").transform;
            this.pool = PlayerServicesAndModulesContainer.Instance.ObjectsPoolService.GetPool("Entity Attribute Node UI");

            this.CategoryAttributeSelected = EEntityCategoriesAttributes.Attack;
        }
        #endregion

        #region Intern Behaviour

        /// <summary>
        /// Créé les attributeNodeUI dans une pool puis les initialize de sorte que leur callbacks de widgets soient présentent.
        /// </summary>
        /// <param name="entityAttribute"></param>
        /// <param name="attributeSubCategory"></param>
        private void CreateAndInitializeAttributeNodeUI(EntityAttributes entityAttribute, string attributeSubCategory)
        {
            GameObject attributeNodeGameobject = this.pool.AddResponsiveObjectInPool(this.attributeNodeUIContentTransform);

            AttributeNodeUI attributeNodeUIScript = attributeNodeGameobject.GetComponent<AttributeNodeUI>();

            attributeNodeUIScript.Initialize(entityAttribute, attributeSubCategory, AttributeHelper.EEntityCategoryAttributesToELanguageCategory(this.CategoryAttributeSelected));
        }

        private void UpdateAttributeCategory()
        {
            Transform playerTransform = SceneServicesContainer.Instance.SceneStateModule.Player;

            // Que se passerait-il si le joueur meurt et donc ne soit plus présent dans le GameState ?
            if (null != playerTransform)
            {
                // Désabonne tous widgets des attributeNodeUI puis les désactive de la pool.
                if (this.pool.GetTheNumberOfObjectsInPool() > 0)
                {
                    for (int poolIndex = this.pool.GetGameobjects().Length - 1; poolIndex >= 0; poolIndex--)
                    {
                        GameObject obj = this.pool.GetGameobjects()[poolIndex];

                        if (null != obj && obj.activeSelf)
                        {
                            obj.GetComponent<AttributeNodeUI>().UnsubscribeToUICallbacks();
                            pool.RemoveObjectInPool(obj);
                        }
                    }
                }

                // Créé puis initialize tous les attributeNodeUI.
                EntityAttributes[] attributes = playerTransform.GetComponent<PlayerAttributesModule>().GetCategoryAttribute(this.categoryAttributeSelected);
                for (int attributeIndex = 0; attributeIndex < attributes.Length; attributeIndex++)
                {
                    EntityAttributes entityAttribute = attributes[attributeIndex];

                    this.CreateAndInitializeAttributeNodeUI(entityAttribute, "Current");
                    this.CreateAndInitializeAttributeNodeUI(entityAttribute, "At Start");

                    if (null != entityAttribute.Percent)
                        this.CreateAndInitializeAttributeNodeUI(entityAttribute, "Percent");

                    foreach (var attribute in entityAttribute.GetOtherAttributes())
                        this.CreateAndInitializeAttributeNodeUI(entityAttribute, attribute.Key);
                }
            }
        }
        #endregion
    }
}