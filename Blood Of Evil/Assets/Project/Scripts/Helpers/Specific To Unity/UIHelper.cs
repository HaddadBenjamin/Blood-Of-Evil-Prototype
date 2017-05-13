using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace BloodOfEvil.Helpers
{
    using Extensions;
    
    public static class UIHelper
    {
        public static void OpenAndCloseMenu(GameObject menuToOpen, GameObject menuToClose, Action callback = null)
        {
            menuToOpen.SetActive(true);

            callback.SafeCall();

            menuToClose.SetActive(false);
        }


        public static EventTriggerType GetCrossPlatformOnClickEventTriggerType()
        {
    #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return EventTriggerType.PointerUp;
    #else
            return EventTriggerType.PointerClick;
    #endif
        }
    }
}
