
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace BloodOfEvil.Helpers
{
    public delegate void DelegateInstantiateWithIndex(int index);

    // Ce code est très vieux et dégueulasse, il faudrait que je le refasse.
    public static class ProbabilitHelper
    {
        /// <summary>
        /// Réinitialise un tableau de probabilités.
        /// </summary>
        public static void ResetProbabilities(ref float[] probabilities)
        {

            for (short i = 0; i < probabilities.Length; i++)
                probabilities[i] = 0;
        }

        /// <summary>
        /// Affiche un tableau de probabilités.
        /// </summary>
        /// <param name="probabilities"></param>
        public static void DisplayProbabilities(float[] probabilities)
        {
            string stringPercent = "";

            for (uint i = 0; i < probabilities.Length; i++)
                stringPercent += probabilities[i] + "%\t";

            Debug.Log(stringPercent);
        }

        public static EnumerationType ProbabilitiesToEnumeration<EnumerationType>(float[] probabilities) where EnumerationType : struct, IConvertible
        {
            return EnumerationHelper.IntegerToEnumeration<EnumerationType>(GetProbabilityIndex(probabilities));
        }


        public static int GetProbabilityIndex(float[] probabilities, bool displayPercent = false) //delegate en default si il y en a on lappel
        {
            int index = 0;
            float total = 0.0f;

            for (uint x = 0; x < probabilities.Length; total += probabilities[x], x++) ;

            float random = UnityEngine.Random.Range(0.0f, total);

            for (float product = probabilities[0]; random > product; ++index, product += probabilities[index]) ;//!(random >= productTmp && random <= product)) //index++;

            if (displayPercent)
            {
                string percents = "";

                for (uint i = 0; i < probabilities.Length; i++)
                    percents += (probabilities[i] / total * 100.00f) + "%\t";
                Debug.Log(percents);
            }

            return index;
        }

        public static float[] ProbabilityConvertorPartToPercent(float[] probabilities)
        {
            float[] percentProbabilities = new float[probabilities.Length];
            float total = 0.0f;

            for (uint x = 0; x < probabilities.Length; total += probabilities[x], x++) ;

            float ratio = 1 / total * 100;

            for (uint i = 0; i < probabilities.Length; i++)
                percentProbabilities[i] = probabilities[i] * ratio;

            DisplayProbabilities(percentProbabilities);

            return percentProbabilities;
        }


        //public e_entityAttribute GetProbabilityIndex(List<EquipmentPossibleAttribute> possibleAttribute, bool displayPercent = false)
        //{
        //    int index = 0;
        //    float total = 0.0f;

        //    for (int x = 0; x < possibleAttribute.Count; x++)
        //        total += possibleAttribute[x].probability;

        //    float random = Random.Range(0.0f, total);

        //    for (float product = possibleAttribute[0].probability; random > product; ++index, product += possibleAttribute[index].probability) ;//!(random >= productTmp && random <= product)) //index++;

        //    if (displayPercent)
        //    {
        //        string percents = "";

        //        for (short i = 0; i < possibleAttribute.Count; i++)
        //            percents += (possibleAttribute[i].probability / total * 100.00f) + "%\t";
        //        Debug.Log(percents);
        //    }

        //    return possibleAttribute[index].attribute;
        //}


        #region Exemple d'utilisation
        public static List<int> MakeFallMyItemWithFunction(float[] probabilities) { return MakeFallMyItemWithFunction(probabilities, new Vector2(1.0f, 1.0f)); }
        public static List<int> MakeFallMyItemWithFunction(float[] probabilities, DelegateInstantiateWithIndex del) { return MakeFallMyItemWithFunction(probabilities, new Vector2(1.0f, 1.0f), del); }
        public static List<int> MakeFallMyItemWithFunction(float[] probabilities, Vector2 itemFallChance)
        {
            List<int> indexArray = new List<int>();

            float itemFall = UnityEngine.Random.Range(itemFallChance.x, itemFallChance.y);

            for (; itemFall >= 1; itemFall--)
                indexArray.Add(GetProbabilityIndex(probabilities));

            if (itemFall < 1 && itemFall != 0)
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= itemFall)
                    indexArray.Add(GetProbabilityIndex(probabilities));

            return indexArray;
        }

        public static List<int> MakeFallMyItemWithFunction(float[] probabilities, Vector2 itemFallChance, DelegateInstantiateWithIndex del)
        {
            List<int> indexArray = MakeFallMyItemWithFunction(probabilities, itemFallChance);

            foreach (int val in indexArray)
                del(val);

            return indexArray;
        }
        #endregion
    }
}