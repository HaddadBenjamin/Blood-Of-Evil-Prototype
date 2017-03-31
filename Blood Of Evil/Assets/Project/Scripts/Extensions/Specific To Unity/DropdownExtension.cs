using UnityEngine.UI;

namespace BloodOfEvil.Extensions
{
    public static class DropdownExtension
    {
        /// <summary>
        /// Permet de mettre à jour l'index de la valeur du menu déroulant met à jour son texte.
        /// </summary>
        public static void ModifyIndexValueThenUpdateText(this Dropdown dropdown, int indexValue)
        {
            dropdown.onValueChanged.SafeInvoke(indexValue);

            dropdown.RefreshShownValue();
        }
    }
}