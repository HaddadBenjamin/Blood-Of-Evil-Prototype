using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace BloodOfEvil.Utilities.UI
{
    using Scene;

    using Player;
    using Player.Services.Audio;

    [RequireComponent(typeof(UnityEngine.UI.Scrollbar))]
    public class ScrollBar : MonoBehaviour, IPointerDownHandler
    {
        #region Fields
        protected UnityEngine.UI.Scrollbar scrollBar;
        protected string soundName = "Scrollbar Modified Value";
        #endregion

        #region Unity Behaviour
        void Awake()
        {
            this.scrollBar = GetComponent<UnityEngine.UI.Scrollbar>();

            this.scrollBar.onValueChanged.AddListener(this.OnScrollBarModifiedValue);
        }

        public virtual void OnScrollBarModifiedValue(float value)
        {
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.SFX, this.soundName);
        }
        #endregion
    }
}