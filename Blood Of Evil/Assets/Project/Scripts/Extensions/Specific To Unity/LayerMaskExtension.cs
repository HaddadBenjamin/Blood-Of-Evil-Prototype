using UnityEngine;
using System.Collections;

namespace BloodOfEvil.Extensions
{
	public static class LayerMaskExtension
	{
	    /// <summary>
	    /// Permet de créer un layerMask avec des noms de layers pour dire sur quel layer colisionnera un objet, un raycast ou autre.
	    /// </summary>
	    public static LayerMask SetColisionLayerMask(this LayerMask layerMask, params string[] colisionLayerNames)
	    {
			layerMask = 0;

			foreach (string colisionLayerName in colisionLayerNames)
				layerMask = layerMask.AddColisionLayerMask(colisionLayerName);

			return layerMask;
	    }

	    /// <summary>
	    /// Permet de rajouter un layer de colision a un layerMask.
	    /// </summary>
	    public static LayerMask AddColisionLayerMask(this LayerMask layerMask, string colisionLayerName)
	    {
			return layerMask |= LayerNameIndexToBitShiftValue(colisionLayerName);
	    }

	    /// <summary>
	    /// Permet d'enlever un layer de colision a un layerMask.
	    /// </summary>
	    public static LayerMask RemoveColisionLayerMask(this LayerMask layerMask, string colisionLayerName)
	    {
			return layerMask &= ~(LayerNameIndexToBitShiftValue(colisionLayerName));
	    }

	    /// <summary>
	    /// Permet de déterminer si un layer de colision est actif ou non.
	    /// </summary>
	    public static bool TestColisionLayerMask(this LayerMask layerMask, string colisionLayerName)
	    {
			return (layerMask & LayerNameIndexToBitShiftValue(colisionLayerName)) != 0;
	    }

	    /// <summary>
	    /// Exemple : 
	    /// - LayerMask.NameToLayer("ClickableObject") = 8, il semblerait que cela soit l'index du layer et non sa valeur en décallage binaire.
	    /// - LayerNameIndexToBitShiftValue("ClickableObject") = 256.
	    /// </summary>
	    public static int LayerNameIndexToBitShiftValue(string colisionLayerName)
	    {
			return (1 << LayerMask.NameToLayer(colisionLayerName));
	    }
	}
}
