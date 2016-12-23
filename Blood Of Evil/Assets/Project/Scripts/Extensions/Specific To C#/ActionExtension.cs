using UnityEngine;
using System.Collections;
using System;

namespace BloodOfEvil.Extensions
{
    public static class ActionExtension
    {
        /// <summary>
        /// Appelle la méthode de l'action si il y en a une.
        /// </summary>
        public static void SafeCall(this Action action)
        {
            if (null != action)
                action();
        }

        /// <summary>
        /// Appelle la méthode de l'action si il y en a une en lui spécifiant son paramètre d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType>(
            this Action<TFirstParameterType> action,
            TFirstParameterType firstParameter)
        {
            if (null != action)
                action(firstParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'action si il y en a une en lui spécifiant ses 2 paramètres d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType, TSecondParameterType>(
            this Action<TFirstParameterType, TSecondParameterType> action,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter)
        {
            if (null != action)
                action(firstParameter, secondParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'action si il y en a une en lui spécifiant ses 3 paramètres d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType, TSecondParameterType, TThirdParameterType>(
            this Action<TFirstParameterType, TSecondParameterType, TThirdParameterType> action,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter,
            TThirdParameterType thirdParameter)
        {
            if (null != action)
                action(firstParameter, secondParameter, thirdParameter);
        }

        /// <summary>
        /// Appelle la méthode de l'action si il y en a une en lui spécifiant ses 4 paramètres d'entré.
        /// </summary>
        public static void SafeCall<TFirstParameterType, TSecondParameterType, TThirdParameterType, TFourthParameterType>(
            this Action<TFirstParameterType, TSecondParameterType, TThirdParameterType, TFourthParameterType> action,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter,
            TThirdParameterType thirdParameter,
            TFourthParameterType fourthParameterType)
        {
            if (null != action)
                action(firstParameter, secondParameter, thirdParameter, fourthParameterType);
        }
    }
}