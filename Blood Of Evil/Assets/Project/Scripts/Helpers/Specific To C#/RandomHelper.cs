using System.Collections;
using System;
using System.Collections.Generic;

namespace BloodOfEvil.Helpers
{
    public static class RandomHelper
    {
      /// <summary>
      /// Renvoie une liste de int avec toutes les valeurs comprises entre 0 et count -1.
      /// </summary>
      public static List<int> GetAShuffleIntList(int count)
      {
          Random random = new Random();
          List<int> listOfInt = new List<int>();

          for (; count > 0; count--)
              listOfInt.Add(count - 1);

          int numberOfElements = listOfInt.Count;

          while (numberOfElements > 1)
          {
              int k = random.Next(numberOfElements--);
              int temp = listOfInt[numberOfElements];
              listOfInt[numberOfElements] = listOfInt[k];
              listOfInt[k] = temp;
          }

          return listOfInt;
      }
    }
}
