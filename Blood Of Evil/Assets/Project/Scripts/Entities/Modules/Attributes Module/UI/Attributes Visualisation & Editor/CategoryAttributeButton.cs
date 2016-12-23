using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Entities.Modules.Attributes.UI
{
    using Utilities.UI;

    public class CategoryAttributeButton : AButton
    {
        #region Fields
        public EEntityCategoriesAttributes CategoryAttribute { get; private set; }
        #endregion

        #region Abstract Behaviour
        public override void ButtonActionOnClick()
        {
            transform.parent.GetComponent<AttributesNodeUIManager>().CategoryAttributeSelected = this.CategoryAttribute;
        }
        #endregion

        #region Public Behaviour
        public void Initialize(EEntityCategoriesAttributes categoryAttribute)
        {
            this.CategoryAttribute = categoryAttribute;

            transform.Find("Text").
                GetComponent<Text>().
                text = this.CategoryAttribute.ToString();
        }
        #endregion
    }
}