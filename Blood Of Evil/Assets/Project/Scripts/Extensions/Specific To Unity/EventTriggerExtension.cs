using UnityEngine.EventSystems;

namespace BloodOfEvil.Extensions
{
	public static class EventTriggerExtension
	{
		/// <summary>
		/// Permet de rajouter une action lorsqu'un évênement de widget est récupéré.
		/// </summary>
		public static void AddUIListener(
			this EventTrigger eventTrigger,
			EventTriggerType eventType,
			UnityAction<BaseEventData> listener)
		{
			EventTrigger.Entry eventTriggerEntry = new EventTrigger.Entry();

			eventTriggerEntry.eventID = eventType;

			eventTriggerEntry.callback.AddListener((data) =>
			{
				listener((PointerEventData)data);
			});

			eventTrigger.triggers.Add(eventTriggerEntry);
		}

		/// <summary>
		/// Permet de supprimer une action lorsqu'un évênement de widget est récupéré.
		/// </summary>
		public static void RemoveUIListener(
			this EventTrigger eventTrigger,
			EventTriggerType eventType,
			UnityAction<BaseEventData> listener)
		{
			EventTrigger.Entry eventTiggerEntr = new EventTrigger.Entry();

			eventTiggerEntr.eventID = eventType;

			eventTiggerEntr.callback.AddListener((data) =>
			{
				listener((PointerEventData)data);
			});

			eventTrigger.triggers.Remove(eventTiggerEntr);
		}
	}
}
