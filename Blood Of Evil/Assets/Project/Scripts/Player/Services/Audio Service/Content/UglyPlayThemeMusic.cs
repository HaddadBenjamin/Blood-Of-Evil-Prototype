using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Player.Services.Audio
{
    using Scene;

    public class UglyPlayThemeMusic : MonoBehaviour
    {
        void Start()
        {
            SceneServicesContainer.Instance.AudioReferencesArraysService.Play2DSound(EAudioCategory.Music, "Music");
        }
    }
}