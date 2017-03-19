using BloodOfEvil.Extensions;
using UnityEditor;
using UnityEngine;

namespace BloodOfEvil.Utilities
{
    /// <summary>
    /// Affiche une énumération de type bitmask dans l'inspecteur.
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumerationBitMaskAttribute))]
    public class EnumerationBitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
        {
            var propertyType = attribute as EnumerationBitMaskAttribute;

            label.text = label.text + "(" + serializedProperty.intValue + ")";
            serializedProperty.intValue = EditorExtension.DrawBitMaskField(position, serializedProperty.intValue, propertyType.propertyType, label);
        }
    }
}
