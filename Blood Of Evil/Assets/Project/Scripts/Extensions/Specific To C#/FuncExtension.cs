using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BloodOfEvil.Extensions
{
    public static class FuncExtension
    {
        /// <summary>
        /// Appelle la méthode de l'action si il y en une pour renvoyer son paramètre de retour.
        /// Si la méthode de l'action n'éxiste pas alors on renvoit la valeur du constructeur par défault du type de retour.
        /// </summary>
        public static TReturnType SafeCall<TReturnType>(
            this Func<TReturnType> actionThatReturnAValue)
        {
            if (null != actionThatReturnAValue)
                return actionThatReturnAValue();

            return default(TReturnType);
        }

        /// <summary>
        /// Appelle la méthode de l'action avec si il y en une en lui spécifiant son paramètre pour renvoyer son paramètre de retour.
        /// Si la méthode de l'action n'éxiste pas alors on renvoit la valeur du constructeur par défault du type de retour.
        /// </summary>
        public static TReturnType SafeCall<TFirstParameterType, TReturnType>(
            this Func<TFirstParameterType, TReturnType> actionThatReturnAValue,
            TFirstParameterType firstParameter)
        {
            if (null != actionThatReturnAValue)
                return actionThatReturnAValue(firstParameter);

            return default(TReturnType);
        }

        /// <summary>
        /// Appelle la méthode de l'action avec si il y en une en lui spécifiant ses paramètres pour renvoyer son paramètre de retour.
        /// Si la méthode de l'action n'éxiste pas alors on renvoit la valeur du constructeur par défault du type de retour.
        /// </summary>
        public static TReturnType SafeCall<TFirstParameterType, TSecondParameterType, TReturnType>(
            this Func<TFirstParameterType, TSecondParameterType, TReturnType> actionThatReturnAValue,
            TFirstParameterType firstParameter,
            TSecondParameterType secondParameter)

        {
            if (null != actionThatReturnAValue)
                return actionThatReturnAValue(firstParameter, secondParameter);

            return default(TReturnType);
        }
    }
}