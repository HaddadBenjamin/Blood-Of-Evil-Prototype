using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
    /// <summary>
    /// Cette classe vient de Manzalab.
    /// </summary>
	public static class MutlicastDelegateExtension
	{
		/// <summary>
		/// Raise an event with routines-delegates.
		/// </summary>
		/// <param name="evnt">Event to raise</param>
		/// <param name="action">Routines to start</param>
		/// <returns>Routine</returns>
		public static IEnumerator YieldForEach(this System.MulticastDelegate evnt, System.Func<System.Delegate, Coroutine> action)
		{
			if (evnt != null)
			{
				System.Delegate[] delegates = evnt.GetInvocationList();
				foreach (System.Delegate d in delegates)
				{
					yield return action(d);
				}
			}
		}

		/// <summary>
		/// Raise an event with routines-delegates.
		/// </summary>
		/// <param name="evnt">Event to raise</param>
		/// <param name="runner">Routine runner</param>
		/// <param name="action">Routines to start</param>
		/// <returns>Coroutine</returns>
		public static Coroutine YieldForEach(this System.MulticastDelegate evnt, MonoBehaviour runner, System.Func<System.Delegate, IEnumerator> action)
		{
			return runner.StartCoroutine(evnt.YieldForEach((d) => runner.StartCoroutine(action(d))));
		}
	}
}
