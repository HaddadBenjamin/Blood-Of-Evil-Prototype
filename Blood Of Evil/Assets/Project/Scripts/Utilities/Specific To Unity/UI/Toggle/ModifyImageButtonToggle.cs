using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Permet de changer l'image en fonction de l'état de notre case à coché.
    /// </summary>
    [   RequireComponent(typeof(Image)),
        RequireComponent(typeof(ButtonToggle))]
    public class ModifyImageButtonToggle : MonoBehaviour
    {
        #region Fields
        [SerializeField, Tooltip("C'est le sprite affiché lorsque l'état de notre case à coché est allumé.")]
        private Sprite onImage;
        [SerializeField, Tooltip("C'est le sprite affiché lorsque l'état de notre case à coché est éteind.")]
        private Sprite offImage;
        #endregion

        #region Unity Behaviour
        private void Awake()
        {
            GetComponent<ButtonToggle>().OnIsOnChanged.AddListener((isOn) =>
            {
                GetComponent<Image>().sprite = isOn ? this.onImage : this.offImage;
            });
        }
        #endregion
    }
}
