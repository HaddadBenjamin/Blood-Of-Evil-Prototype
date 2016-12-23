using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace BloodOfEvil.Helpers
{
    public static class UICallbackHelper
    {
        public static void AddCallbacksToEventTrigger(EventTrigger eventTrigger, UICallbackData[] callbacksData)
        {
            foreach (UICallbackData callbackData in callbacksData)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry();

                entry.eventID = callbackData.Type;
                entry.callback = new EventTrigger.TriggerEvent();

                UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(callbackData.Callback);
                entry.callback.AddListener(callback);

                eventTrigger.triggers.Add(entry);
            }
        }

        public static void AddCallbackToEventTrigger(EventTrigger eventTrigger, EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            AddCallbacksToEventTrigger(eventTrigger, new UICallbackData[]
            {
            new UICallbackData(type, callback),
            });
        }
    }
}