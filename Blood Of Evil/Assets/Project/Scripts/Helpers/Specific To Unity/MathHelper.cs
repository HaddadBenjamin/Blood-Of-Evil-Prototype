using System;
using UnityEngine;

namespace BloodOfEvil.Helpers
{
    /// <summary>
    /// Ce code devraît être découpe dans les différentes extensions : float, int, vector2-3, quaternion, etc..
    /// </summary>
    public static class MathHelper
    {
        //- Angle2DSigned et Angled3Signed dans les classes d'extensions de vecteur.
        // Float : ClampAngle(360), InverseAngle360, IsBetweenOrEqual

        /// <summary>
        /// Permet d'avoir un angle compris entre 0 et 360.0f.
        /// </summary>
        public static float ClampAngle360(float angle)
        {
            return angle -
                    (angle > 360.0f ? 360.0f : 0.0f) +
                    (angle < 0.0f ? 360.0f : 0.0f);
        }

        /// <summary>
        /// Inverse la mesure d'un angle compris en [0;360].
        /// </summary>
        public static float InverseAngle360(float angle360)
        {
            return ClampAngle360(angle360 + 180.0f);
        }

        /// <summary>
        /// Détermine si une valeur est plus petite ou égale à 2 valeurs "min" et "max".
        /// </summary>
        public static bool IsBetweenOrEqual(float value, float min, float max)
        {
            return value >= min &&
                   value <= max;
        }

        /// <summary>
        /// Génère un nombre aléatoire entre 2 entiers.
        /// </summary>
        public static int GenerateRandomBeetweenTwoInts(int minimumValue, int maximumValue)
        {
            return UnityEngine.Random.Range(minimumValue, maximumValue + 1);
        }

        /// <summary>
        /// Génère un nombre aléatoire entre 2 floatants.
        /// </summary>
        public static float GenerateRandomBeetweenTwoFloats(float minimumValue, float maximumValue)
        {
            return UnityEngine.Random.Range(minimumValue, maximumValue);
        }

        /// <summary>
        /// Calcul du produit en croix pour trouver une inconnue du type : "4/5 = 23/d" -> recherche x.
        /// De la forme a / b = c / d. On calcule le produit de la diagonale où il y a 2 nombres puis on divise par le nombre restant.
        /// </summary>
        public static float CrossProductForScalar(float a, float b, float c, float d, char whoIsTheUnknown)
        {
            switch (whoIsTheUnknown)
            {
                case 'a': return b * c / d;
                case 'b': return a * d / c;
                case 'c': return a * d / b;
                default:  return b * c / a;
            }
        }
        
        /// <summary>
        /// Calcul du plus grand dénominateur commum par la méthode d'euclide.
        /// </summary>
        public static float PGCD(float firstNumber, float secondNumber)
        {
            return secondNumber == 0 ? firstNumber : PGCD(secondNumber, firstNumber % secondNumber);
        }

        /// <summary>
        /// Par exemple pour déterminer si un coup critique à lieu : 
        /// PercentageOfProbabilityToBoolean(base.GetAttribute(EEntityCategoriesAttributes.Attack, "Critical Chance Percentage").Current.Value)
        /// </summary>
        public static bool PercentageOfProbabilityToBoolean(float probability)
        {
            return probability > MathHelper.GenerateRandomBeetweenTwoFloats(0, 100.0f);
        }

        /// <summary> 
        /// Convertie une valeur comprise entre [0;1] vers [-1;1]. 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static float NormalizedValueToStandarisedValue(float value)
        {
            return value * 2.0f - 1.0f;
        }

        /// <summary>
        /// Convertit un double en fraction.
        /// </summary>
        public static string DoubleToFraction(double number, double epsilon = 0.0001f, int maxIterations = 40)
        {
            double[] fraction = new double[maxIterations + 2];
            fraction[1] = 1;
            double numberTmp = number;
            double denominator = 1;
            int fractionIndex = 1;

            int wholeNumberPart = (int)number;
            double decimalNumberPart = number - Convert.ToDouble(wholeNumberPart);

            while (fractionIndex < maxIterations && Math.Abs(denominator / fraction[fractionIndex] - number) > epsilon)
            {
                fractionIndex++;
                numberTmp = 1 / (numberTmp - (int)numberTmp);
                fraction[fractionIndex] = fraction[fractionIndex - 1] * (int)numberTmp + fraction[fractionIndex - 2];
                numberTmp = (int)(decimalNumberPart * fraction[fractionIndex] + 0.5);
            }

            return string.Format((wholeNumberPart > 0 ?
                wholeNumberPart.ToString() + " " :
                "") + "{0}/{1}", denominator.ToString(), fraction[fractionIndex].ToString());
        }

        public static double FractionToDouble(double numerator, double denominator)
        {
            return numerator / denominator;
        }

        #region Equations
        /// <summary>
        /// Sous la forme : n * x = total, exemple 5x = 8, return 8 / 5
        /// </summary>
        public static float EquationFirstOrder(float numberOfUnknown, float total)
        {
            return total / numberOfUnknown;
        }

        /// <summary>
        /// Equation de la forme : ax² + bx + c = 0.
        public static void EquationSecondOrder(float a, float b, float c)
        {
            float descriminant = descriminant = (b * b) - (4 * a * c);

            if (descriminant > 0)
            {
                Debug.LogFormat("First solution : {0}", (-b + Mathf.Sqrt(descriminant)) / (2 * a));
                Debug.LogFormat("Second solution : {0}", (-b - Mathf.Sqrt(descriminant)) / (2 * a));
            }
            else if (descriminant == 0)
                Debug.LogFormat("Unique solution : {0}", -b / (2 * a));
            else
                Debug.Log("There is none solution");
        }

        /// <summary>
        /// Affiche un lien pour calculer une équation avec 2 inconnus.
        /// </summary>
        public static void EquationWith2Unknows()
        {
            Debug.Log("http://www.webmath.com/solver2.html");
        }

        public static float EquationStraigthLine(float AAxixHorizontal, float AAxisVertical, float BAxixHorizontal, float BAxisVertical)
        {
            return (BAxisVertical - AAxisVertical) / (BAxixHorizontal - AAxixHorizontal);
        }
        #endregion

        #region Trigonometry
        /// <summary>
        /// Il faudra définir la longueur du côté non défini à 0 pour que ce code marche. Pour un triangle rectangle en A respectant la figure suivante : http://www.google.fr/imgres?imgurl=http%3A%2F%2Fcalculis.net%2Fimages%2Ftriangle-rectangle.png&imgrefurl=http%3A%2F%2Fcalculis.net%2Fhypotenuse&h=180&w=300&tbnid=zOp8O64m1ZLKZM%3A&docid=MUP2o478awwjEM&ei=zskkVun5JYmsa5eJgNgF&tbm=isch&iact=rc&uact=3&dur=214&page=1&start=0&ndsp=20&ved=0CDYQrQMwAGoVChMI6ZjUwq3OyAIVCdYaCh2XBABb
        /// </summary>
        public static float TrigonometryFindAngleThanks2Sides(float ABLength, float BCLength, float CALength, char angleNeeded = 'B')
        {
            if (angleNeeded == 'B') //AB : adjacent, BC : hypothenuse, CA : opposé
            {
                //Côte que ne l'on connaît pas
                if (0.0f == ABLength) //Arcsinus : CA : oppose, BC : hypothenuse
                    return Mathf.Rad2Deg * Mathf.Asin(CALength / BCLength);
                if (0.0f == BCLength) //Arctangente : AB : adjacent, CA : opposé
                    return Mathf.Rad2Deg * Mathf.Atan(CALength / ABLength);
                //if (0.0f == CALength) //ArcCosinus : AB : adjacent, BC : hypothenuse
                return Mathf.Rad2Deg * Mathf.Acos(ABLength / BCLength);
            }

            //if (angleNeeded = 'C') AB : opposé, BC : hypothénuse, CA : adjacent
            if (0.0f == ABLength) //BC : hypothénuse, CA : adjacent
                return Mathf.Rad2Deg * Mathf.Acos(CALength / BCLength);
            else if (0.0f == BCLength) //AB : opposé, CA : adjacent
                return Mathf.Rad2Deg * Mathf.Atan(ABLength / CALength);
            //if (0.0f == CALength) // AB : opposé, BC : hypothenusé
            return Mathf.Rad2Deg * Mathf.Asin(ABLength / BCLength);
        }
        /// <summary>
        /// Il faudra définir les longueurs des côtés et des angles non défini à 0, lengthSideNeeded peut prendre que 3 valeurs : AB, BC, CA pour que ce code marche. Pour un triangle rectangle en A respectant la figure suivante : http://www.google.fr/imgres?imgurl=http%3A%2F%2Fcalculis.net%2Fimages%2Ftriangle-rectangle.png&imgrefurl=http%3A%2F%2Fcalculis.net%2Fhypotenuse&h=180&w=300&tbnid=zOp8O64m1ZLKZM%3A&docid=MUP2o478awwjEM&ei=zskkVun5JYmsa5eJgNgF&tbm=isch&iact=rc&uact=3&dur=214&page=1&start=0&ndsp=20&ved=0CDYQrQMwAGoVChMI6ZjUwq3OyAIVCdYaCh2XBABb
        /// </summary>
        ///A VERIFIER !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public static float TrigonometryFindLengthSideWithAngleAndOtherLengthSide(float ABLength, float BCLength, float CALength, float angleB, float angleC, string lengthSideNeeded = "AB")
        {
            if (0.0f != angleB) //AB : adjacent, BC : hypothénuse, CA :  opposé
            {
                if ("AB" == lengthSideNeeded) //adjacent
                {
                    if (0.0f != BCLength) //AB : adjacent, BC : hypothénuse
                        return CrossProductForScalar(Mathf.Cos(angleB), 1, ABLength, BCLength, 'c');
                    if (0.0f != CALength) //AB : adjacent, CA : opposé
                        return CrossProductForScalar(Mathf.Tan(angleB), 1, CALength, ABLength, 'd');
                }

                if ("BC" == lengthSideNeeded) //hypothénuse
                {
                    if (0.0f != ABLength) //AB : adjacent, BC : hypothénuse
                        return CrossProductForScalar(Mathf.Cos(angleB), 1, ABLength, CALength, 'd');
                    if (0.0 != CALength) //BC : hypothénuse, CA : opposé
                        return CrossProductForScalar(Mathf.Sin(angleB), 1, CALength, BCLength, 'd');
                }

                if ("CA" == lengthSideNeeded) //opposé
                {
                    if (0.0f != ABLength) //AB : adjacent, CA : opposé
                        return CrossProductForScalar(Mathf.Tan(angleB), 1, ABLength, CALength, 'c');
                    if (0.0f != BCLength) //BC : hypothénuse, CA : opposé
                        return CrossProductForScalar(Mathf.Sin(angleB), 1, CALength, BCLength, 'c');
                }
            }

            if (0.0f != angleC)
            {
                if ("AB" == lengthSideNeeded) //opposé
                {
                    if (0.0f != BCLength) //AB : opposé, BC : hypothenuse
                        return CrossProductForScalar(Mathf.Sin(angleC), 1, ABLength, BCLength, 'c');
                    if (0.0f != CALength) //AB : opposé, CA : adjacent
                        return CrossProductForScalar(Mathf.Tan(angleC), 1, ABLength, CALength, 'c');
                }

                if ("BC" == lengthSideNeeded) //hypothénuse
                {
                    if (0.0f != ABLength) //AB : opposé, BC : hypothénuse
                        return CrossProductForScalar(Mathf.Sin(angleC), 1, ABLength, BCLength, 'd');
                    if (0.0f != CALength) //BC : hypothénuse, CA : adjacent
                        return CrossProductForScalar(Mathf.Cos(angleC), 1, CALength, BCLength, 'd');
                }

                if ("CA" == lengthSideNeeded) //adjacent
                {
                    if (0.0f != ABLength) //AB : opposé, CA : adjacent
                        return CrossProductForScalar(Mathf.Tan(angleC), 1, ABLength, CALength, 'd');
                    if (0.0f != BCLength) //BC : hypothénuse, CA : adjacent
                        return CrossProductForScalar(Mathf.Cos(angleC), 1, CALength, BCLength, 'c');
                }
            }

            return 0.0f;
        }
        #endregion

        #region Statistics
        /// <summary>
        /// On retourne la différence entre le nombre maximal et minimal.
        /// </summary>
        public static float StatisticsExtent(float[] tab)
        {
            float min = 0.0f, max = 0.0f;

            foreach (float number in tab)
            {
                if (number > max)
                    max = number;

                if (number < min)
                    min = number;
            }

            return max - min;
        }

        /// <summary>
        /// Calcul de la moyenne.
        /// </summary>
        public static float StatisticsAverage(float[] tab)
        {
            float sum = 0.0f;

            foreach (float number in tab)
                sum += number;

            return sum / tab.Length;
        }

        /// <summary>
        /// Si tu cherche à comprendre à quoi ça correspond : http://www.cmath.fr/3eme/statistiques/cours.php#e2
        /// </summary>
        public static float StatisticsAverageWithCoefficient(float[] numbers, int[] coefficients)
        {
            float sum = 0.0f;
            int numberOfNumbers = 0;

            for (int i = 0; i < numbers.Length; i++)
            {
                sum += numbers[i] * coefficients[i];
                numberOfNumbers += coefficients[i];
            }

            return sum / numberOfNumbers;
        }

        /// <summary>
        /// Si tu cherche à comprendre à quoi ça correspond : http://www.cmath.fr/3eme/statistiques/cours.php#e2
        /// </summary>
        public static float StatisticsMedian(float[] numbers)
        {
            Array.Sort(numbers);

            int middle = numbers.Length / 2;

            return (numbers.Length % 2 == 0) ? (numbers[middle] + numbers[middle - 1]) / 2 : numbers[middle];
        }
        #endregion
    }
}