using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace BloodOfEvil.Extensions
{
    /// Permet de voir notre êvênement unity dans l'inspecteur car les types teplates ne sont pas visible.
    /// Alors que les types prédéfinis le sont.
    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [System.Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }
    
    public static class UnityEventExtension
    {
        public static void SafeInvoke(this UnityBoolEvent unityEvent, bool parameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(parameter);
        }

        public static void SafeInvoke(this UnityFloatEvent unityEvent, float parameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(parameter);
        }
        
        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une.
        /// </summary>
        public static void SafeCall(this UnityEvent unityEvent)
        {
            if (null != unityEvent)
                unityEvent.Invoke();
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant son paramètre d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType>(
            this UnityEvent<TFirstParameterType> unityEvent,
            TFirstParameterType firstParameter)
        {
            if (null != unityEvent)
                unityEvent.Invoke(firstParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'unity event si il y en a une en lui spécifiant ses 2 paramètres d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType, TSecondParameterType>(
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
        public static void SafeCall<TFirstParameterType, TSecondParameterType, TThirdParameterType>(
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
        public static void SafeCall<TFirstParameterType, TSecondParameterType, TThirdParameterType, TFourthParameterType>(
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
