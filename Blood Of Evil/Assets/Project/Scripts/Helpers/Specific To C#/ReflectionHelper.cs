using System.Reflection;

namespace BloodOfEvil.Helpers
{
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
    }
}