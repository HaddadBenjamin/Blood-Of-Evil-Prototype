using UnityEngine;
using System.Collections;
using BloodOfEvil.Utilities;
using UnityEngine.Events;

namespace BloodOfEvil.Extensions
{
    public static class UnityEventExtension
    {
        /// <summary>
        /// Appele l'évenement de type boolean avec son paramètre si il éxiste.
        /// </summary>
        public static void SafeInvoke(this UnityBoolEvent unityEvent, bool parameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(parameter);
        }

        /// <summary>
        /// Appele l'évenement de type float avec son paramètre si il éxiste.
        /// </summary>
        public static void SafeInvoke(this UnityFloatEvent unityEvent, float parameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(parameter);
        }

        /// <summary>
        /// Appele l'évenement si il éxiste.
        /// </summary>
        public static void SafeInvoke(this UnityEvent unityEvent)
        {
            if (null != unityEvent)
                unityEvent.Invoke();
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant son paramètre d'entré.
        /// </summary>
        public static void SafeInvoke<TFirstParameterType>(
            this UnityEvent<TFirstParameterType> unityEvent,
            TFirstParameterType firstParameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(firstParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant ses 2 paramètres d'entré.
        /// </summary>
        public static void SafeInvoke<TFirstParameterType, TSecondParameterType>(
            this UnityEvent<TFirstParameterType, TSecondParameterType> unityEvent,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(firstParameter, secondParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant ses 3 paramètres d'entré.
        /// </summary>
        public static void SafeInvoke<TFirstParameterType, TSecondParameterType, TThirdParameterType>(
            this UnityEvent<TFirstParameterType, TSecondParameterType, TThirdParameterType> unityEvent,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter,
            TThirdParameterType thirdParameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(firstParameter, secondParameter, thirdParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant ses 4 paramètres d'entré.
        /// </summary>
        public static void SafeInvoke<TFirstParameterType, TSecondParameterType, TThirdParameterType, TFourthParameterType>(
            this UnityEvent<TFirstParameterType, TSecondParameterType, TThirdParameterType, TFourthParameterType> unityEvent,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter,
            TThirdParameterType thirdParameter,
            TFourthParameterType fourthParameterType)
        {
            if (null != unityEvent)
                unityEvent.Invoke(firstParameter, secondParameter, thirdParameter, fourthParameterType);
        }
    }
}
