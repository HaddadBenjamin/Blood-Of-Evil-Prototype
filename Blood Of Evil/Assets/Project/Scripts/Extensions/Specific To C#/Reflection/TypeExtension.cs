using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodOfEvil.Helpers;

namespace BloodOfEvil.Extensions
{
    using Extensions;

    public static class TypeExtension
    {
        /// <summary>
        /// Renvoi une classe de type : return new (typeof(type)).
        /// </summary>
        public static object TypeToInstance(this Type type)
        {
            return Activator.CreateInstance(type);
        }

        /// <summary>
        /// Renvoit si le parent du type est de type "TParentType". (marche pour les classes, les interfaces).
        /// </summary>
        public static bool DoesTypeUnheritOf<TParentType>(this Type type)
        {
            return type.IsAssignableFrom(typeof(TParentType));
        }

        /// <summary>
        /// Renvoit tous les noms des champs de la classe.
        /// </summary>
        public static string GetAllFieldsName(this Type type)
        {
            return
                String.Join("\n", type.GetFields(ReflectionHelper.GetAllBaseFlags())
                    .Select(field => field.Name)
                    .ToArray());
        }

        /// <summary>
        /// Renvoit la valeur de tous les champs du type.
        /// </summary>
        public static string GetAllFieldsValue(this Type type)
        {
            var instance = TypeExtension.TypeToInstance(type);
            // Récupère toutes les membres qui font partie des propriétés de "GetAllFlags".
            MemberInfo[] members = type.GetMembers(ReflectionHelper.GetAllBaseFlags());
            List<string> memberFieldsValues = new List<string>();

            foreach (var member in members)
            {
                /// Si le membre est un champ, je récupère sa valeur et je la stoque.
                /// Lorsque l'on récupère la valeur d'une champ, on a besoin de son nom et d'une instance.
                /// exemple : Timer timerInstance=  new Timer();
                /// float elaspedTimeValue = (float)(typeof(Timer).GetField("elapsedTime", ReflectionHelper.GetAllFlags()).GetValue(timerInstance));
                if (member.MemberType == MemberTypes.Field)
                    memberFieldsValues.Add(type.GetField(member.Name, ReflectionHelper.GetAllBaseFlags()).
                        GetValue(instance).ToString());
            }

            return String.Join("\n", memberFieldsValues.ToArray());
        }

        /// <summary>
        /// Renvoit le nom de tous les membres.
        /// </summary>
        public static string GetAllMembersName(this Type type)
        {
            return
                String.Join("\n", type.GetMembers(ReflectionHelper.GetAllBaseFlags())
                    .Select(member => member.GetMemberInfoMoreInformations())
                    .ToArray());
        }

        /// <summary>
        /// Renvoit le nom de toutes les méthodes.
        /// </summary>
        public static string GetAllMethodsName(this Type type)
        {
            var instance = TypeExtension.TypeToInstance(type);
            MemberInfo[] members = type.GetMembers(ReflectionHelper.GetAllBaseFlags());
            List<string> memberFieldsValues = new List<string>();

            foreach (var member in members)
            {
                /// Si le champ est une méthode, je stoque son nom.
                if (member.MemberType == MemberTypes.Method)
                {
                    /// On récupère une méthode avec type.GetMethod(methodName, methodFlags).
                    MethodInfo method = type.GetMethod(member.Name, ReflectionHelper.GetAllBaseFlags());

                    memberFieldsValues.Add(method.ToString());
                }
            }

            return String.Join("\n", memberFieldsValues.ToArray());
        }

        /// <summary>
        /// Appelle et affiche le résultat des méthodes renvoyant une string et ne prenant aucun paramètres.
        /// </summary>
        public static void InvokeAllMethodsThatReturnAStringAndDontNeedAnyParameter(this Type type)
        {
            var instance = TypeExtension.TypeToInstance(type);
            MemberInfo[] members = type.GetMembers(ReflectionHelper.GetAllBaseFlags());

            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Method)
                {
                    MethodInfo method = type.GetMethod(member.Name, ReflectionHelper.GetAllBaseFlags());

                    /// Si la méthode renvoit une string et qu'elle n'a pas besoin de paramètres, je l'appele et affiche son résultat.
                    if (method.GetParameters().Length == 0 &&
                        method.ReturnParameter.ParameterType == typeof(string))
                        /// Pour appeler une méthode on utilise : method.Invoke(instance, parameters);
                        Console.WriteLine(method.Invoke(instance, null));
                }
            }
        }
    }
}
