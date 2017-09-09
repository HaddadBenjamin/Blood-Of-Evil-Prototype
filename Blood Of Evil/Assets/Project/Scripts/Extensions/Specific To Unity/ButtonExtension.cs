using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace BloodOfEvil.Extensions
{
    using Helpers;
    
    public static class ButtonExtension
    {
        public static void AddResponsiveOnClickListener(this Button button, Action listener)
        {
            button.ForceGetEventTrigger().
                AddUIListener(UIHelper.GetCrossPlatformOnClickEventTriggerType(), (data) => listener.SafeCall());
        }

        public static void AddResponsiveOnClickListener(this Button button, Action<BaseEventData> listener)
        {
            button.ForceGetEventTrigger().
                GetComponent<EventTrigger>().AddUIListener(UIHelper.GetCrossPlatformOnClickEventTriggerType(), (data) => listener.SafeCall(data));
        }

        public static EventTrigger ForceGetEventTrigger(this Button button)
        {
            EventTrigger eventTrigger = button.GetComponent<EventTrigger>();

            if (null == eventTrigger)
                eventTrigger = button.gameObject.AddComponent<EventTrigger>();

            return eventTrigger;
        }
    }
}
