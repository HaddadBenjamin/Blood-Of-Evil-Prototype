using System;
using System.Reflection;
using BloodOfEvil.Extensions;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    /// <summary>
    /// C'est la classe de base contenant les données que l'on a besoin pour utiliser la reflection.
    /// </summary>
    public class TypeAndInstanceType
    {
        /// <summary>
        /// Le type de l'objet.
        /// </summary>
        public Type Type;
        /// <summary>
        /// L'instance de l'objet.
        /// </summary>
        public object InstanceType;

        public TypeAndInstanceType(Type type, object instance)
        {
            this.Type = type;
            this.InstanceType = instance;
        }
    }

    public static class ReflectionHelper
    {
        /// <summary>
        /// Renvoit les flags que l'on utilise le plus souvent pour récupérer un champs, une propriété ou une méthode.
        /// </summary>
        public static BindingFlags GetAllBaseFlags()
        {
            return BindingFlags.Instance |
                   BindingFlags.NonPublic |
                   BindingFlags.Static |
                   BindingFlags.Public;
        }

        /// <summary>
        /// Récupère tous les types d'une librairie spécifique ou de toutes les librairies trouvées.
        /// </summary>
        public static TypeAndInstanceType[] GetAllTypesAndInstanceTypesFromALibrary(string libraryName)
        {
            Module[] allModules = AppDomain.CurrentDomain.Load(libraryName).GetModules();
            List<TypeAndInstanceType> typeAndInstanceTypes = new List<TypeAndInstanceType>();
            
            foreach (var module in allModules)
            {
                var allTypes = module.GetTypes();

                foreach (var type in allTypes)
                    typeAndInstanceTypes.Add(new TypeAndInstanceType(type, type.TypeToInstance()));
            }

            return typeAndInstanceTypes.ToArray();
        }
}