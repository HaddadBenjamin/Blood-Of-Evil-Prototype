using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// C'est un attribut pour spécifier qu'une énumération de type bitmask est affiché dans l'inspecteur.
    /// </summary>
    public class EnumerationBitMaskAttribute : PropertyAttribute
    {
        public System.Type propertyType;

        public EnumerationBitMaskAttribute(System.Type type)
        {
            propertyType = type;
        }
    }
}
