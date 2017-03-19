using UnityEngine;

namespace BloodOfEvil.Utilities
{
    public enum EBitMaskExample
    {
        Up =    (1 << 0),
        Down =  (1 << 1),
        Left =  (1 << 2),
        Right = (1 << 3),
        Front = (1 << 4),
    }

    /// <summary>
    /// Affiche une énumération de flags (un bitmask) dans l'inspecteur d'Unity.
    /// </summary>
    public class EnumerationBitMaskExample : MonoBehaviour
    {
        [SerializeField, EnumerationBitMask(typeof(EBitMaskExample))]
        private EBitMaskExample bitMaskEnumeration;
    }
}
