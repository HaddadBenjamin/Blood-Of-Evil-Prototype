using UnityEngine.EventSystems;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
	public static class EventTriggerExtension
	{
		public static void AddEvent(
            this EventTrigger eventTrigger,
            EventTriggerType eventType,
            UnityEngine.Events.UnityAction<BaseEventData> callback)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();

			entry.eventID = eventType;
			entry.callback.AddListener(callback);
			eventTrigger.triggers.Add(entry);
		}
	}
}

