using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BloodOfEvil.Helpers
{
    public class UICallbackData
    {
        public EventTriggerType Type { get; private set; }
        public UnityAction<BaseEventData> Callback { get; private set; }

        public UICallbackData(EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            this.Type = type;
            this.Callback = callback;
        }
    }
}