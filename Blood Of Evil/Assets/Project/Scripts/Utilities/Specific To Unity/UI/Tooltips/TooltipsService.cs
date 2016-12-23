using System;
using UnityEngine;

namespace BloodOfEvil.Utilities.UI
{
    using Scene.Services.References;

    /// <summary>
    /// Permet de placer nos tooltips en profondeur au dessus de tout, ce script n'est applicable que sur un rectTransform contenant toute nos infobulles.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class TooltipsService : AReferenceContainer<RectTransform>
    {
        #region Fields
        private Transform myTransform;
        #endregion

        #region Unity Methods
        void Awake()
        {
            this.myTransform = transform;

            name = "Tooltips Container";

            this.Initialize();

            Array.ForEach(base.references, reference => reference.gameObject.SetActive(false));

            // Permet de placer cette objet tout en haut de la hiérarchie de la scène.
            //myTransform.SetParent(null);
        }
        #endregion

        #region Behaviour Methods
        void Update()
        {
            // Permet de placer cette objet en tant que dernière enfant de la scène et donc faire en sorte que son contenu d'UI soit affiché en dernier.
            this.myTransform.SetAsLastSibling();
        }
        #endregion

        #region Public Behaviour
        public void DisableAllTooltips()
        {
            foreach (RectTransform rectTransform in base.references)
                rectTransform.gameObject.SetActive(false);
        }
        #endregion
    }
}