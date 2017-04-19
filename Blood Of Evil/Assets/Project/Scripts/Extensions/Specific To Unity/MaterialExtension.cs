using UnityEngine;
using System.Collections;

namspace BloodOfEvil.Extensions
{
    public static class MaterialExtension
    {
        /// <summary>
        /// Permet de modifier la valeur d'intensité de la couleur d'émission.
        /// Exemple d'utilisation : 
        /// this.shaderMaterial = GetComponent<MeshRenderer>().materials[0];
        /// this.defaultEmissionColor = this.shaderMaterial.GetColor("_EmissionColor");
        /// this.shaderMaterial.ModifyEmissionScale(this.defaultEmissionColor, this.emissionColorIntensity);
        /// </summary>
        public static void ModifyEmissionScale(this Material material, Color defaultEmissionColor, float intensity = 1.0f)
        {
            material.EnableKeyword("_EMISSION");

            material.SetColor("_EmissionColor", defaultEmissionColor * intensity);
        }
    }
}
