using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace BloodOfEvil.Player.Services.Video.UI
{
    using ObjectInScene;
    using Utilities.UI;

    public sealed class AddResolutionDropdownElement : ADropdownElementAdder
    {
        #region Unity Behaviour
        void Start()
        {
            CustomDropdown dropdown = GetComponent<CustomDropdown>();

            for (int resolutionIndex = 0; resolutionIndex < VideoService.Resolutions.Length; resolutionIndex++)
            {
                dropdown.ModifyDropdownTitle(
                    dropdown.AddDropdownElement(VideoService.Resolutions[resolutionIndex], resolutionIndex, base.GetLanguageCategory(), delegate (CustomDropdownElement customDropdownElement)
                    {
                        PlayerServicesAndModulesContainer.Instance.VideoService.Resolution = VideoService.Resolutions[customDropdownElement.Index];

                        ((ISerializable)PlayerServicesAndModulesContainer.Instance.VideoService).Save();
                    })
                );
            }
        }
        #endregion
    }
}