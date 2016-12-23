using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace BloodOfEvil.Helpers
{
    /* Note personnels :
    Trees VS Hasmap
    - dans le cas ou je veux que mes donnees soit tries l'arbre que l'on me propose qui sera casiment dans tous les cas un red black tree (O(log n)), dans le cas ou je ne voudrai pas que mes donnees soit tries et que la longueur de mes donnees est en dessous de 10000 (valeur arbitraire pour l'instant) une hashmap (O(1)).

    Conseil pas chere sur les arbres :
    - ne jamais faire de tonRedBlackTree.Get{Values|Keys}|() dans une boucle et par consequent stoquer ces valeurs :
    int[] values = tonRedBlackTree.GetValues();

    Red Black Tree VS AVL
    Red Black Tree : on insert, supprime, et récupère souvent de la données, plus cette arbre contient de données plus cette arbre est éfficace. 
    AVL Tree : on insert, supprime et récupère rarement de la données, moins cette arbre contient de données moins cette arbre est éfficace.
    Enfin si on a beaucoup trop de données (qu'est-ce que ça veut dire éxactement ? combien de données, quel types de données ? taille mémoire des données ?) il est préférable d'utiliser une base de donnée. (est-ce que l'ORM est une idée cohérente et peut s'allier facilement à cete idée globale ?)*/
    public class Sort : MonoBehaviour
    {
        void Awake()
        {
            int[] datas = { 3, 8, 7, 5, 2, 1, 9, 6, 4 };

            Debug.Log("Find The Fastest Integers Then Display All Their Times");
            SortHelper<int>.FindTheFastestIntegerSortThenDisplayAllTheirTimes(datas);

            Debug.Log(string.Join(",", Array.ConvertAll(datas, data => data.ToString())));
        }
    }

    /* les méthodes qui calcul chaque tri font des actions répéter mais si je fais des appels de fonctions, le temps d'un tri est totalement faussé. */
    static class SortHelper<TypeToSort>
    {
        #region Efficient Sorts For Lists
        #region MergeSort
        /// <summary>
        /// I take n memory and that's a lot but it is a speed sort : Best case : n log n, Average case : n log n, Worst case : n log n.
        /// </summary>
        /// <param name="datas"></param>
        public static void MergeSort(TypeToSort[] datas)
        {
            MergeSortRecursive(datas, 0, datas.Length - 1);
        }

        private static void DoMerge(TypeToSort[] datas, int left, int mid, int right)
        {
            TypeToSort[] temp = new TypeToSort[datas.Length];
            int i, left_end, num_elements, tmp_pos;

            left_end = (mid - 1);
            tmp_pos = left;
            num_elements = (right - left + 1);

            while ((left <= left_end) && (mid <= right))
                temp[tmp_pos++] = (Comparer<TypeToSort>.Default.Compare(datas[left], datas[mid]) <= 0) ? datas[left++] : datas[mid++];

            while (left <= left_end)
                temp[tmp_pos++] = datas[left++];

            while (mid <= right)
                temp[tmp_pos++] = datas[mid++];

            for (i = 0; i < num_elements; i++)
            {
                datas[right] = temp[right];
                right--;
            }
        }

        private static void MergeSortRecursive(TypeToSort[] datas, int left, int right)
        {
            int mid;

            if (right > left)
            {
                mid = (right + left) / 2;
                MergeSortRecursive(datas, left, mid);
                MergeSortRecursive(datas, (mid + 1), right);
                DoMerge(datas, left, (mid + 1), right);
            }
        }
        #endregion

        #region HeapSort
        /// <summary>
        /// It looks to be the best sort : memory : 1, Best case : n log n, Average case : n log n, Worst case : n log n.
        /// </summary>
        /// <typeparam name="TypeToSort"></typeparam>
        /// <param name="datas"></param>
        public static void HeapSort(TypeToSort[] datas)
        {
            int heapSize = datas.Length;

            for (int p = (heapSize - 1) / 2; p >= 0; p--)
                MaxHeapify(datas, heapSize, p);

            for (int i = datas.Length - 1; i > 0; i--)
            {
                Swap<TypeToSort>(datas, i, 0);

                heapSize--;
                MaxHeapify(datas, heapSize, 0);
            }
        }
        private static void MaxHeapify<TypeToSort>(TypeToSort[] datas, int heapSize, int index)
        {
            int left = (index + 1) * 2 - 1;
            int right = (index + 1) * 2;
            int largest = 0;

            largest = (left < heapSize && Comparer<TypeToSort>.Default.Compare(datas[left], datas[index]) > 0) ? largest = left : largest = index;

            if (right < heapSize && Comparer<TypeToSort>.Default.Compare(datas[right], datas[largest]) > 0)
                largest = right;

            if (largest != index)
            {
                Swap<TypeToSort>(datas, index, largest);

                MaxHeapify(datas, heapSize, largest);
            }
        }
        #endregion

        #region QuickSort
        /// <summary>
        /// A very good sort but when the datas are already sorted it is a very bad one : Memory : n log average, Best case : n log n, Average case : n log n, Worst cast : n² it happen when the datas are already sorted.
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static void QuickSort(TypeToSort[] datas)
        {
            QuickSortRecursive(datas, 0, datas.Length - 1);
        }

        public static int QuickSortPartition(TypeToSort[] datas, int left, int right)
        {
            TypeToSort pivot = datas[left];

            while (true)
            {
                while (Comparer<TypeToSort>.Default.Compare(datas[left], pivot) < 0)
                    left++;

                while (Comparer<TypeToSort>.Default.Compare(datas[right], pivot) > 0)
                    right--;

                if (left < right)
                    Swap<TypeToSort>(datas, left, right);
                else
                    return right;
            }
        }

        private static void QuickSortRecursive(TypeToSort[] arr, int left, int right)
        {
            if (left < right)
            {
                int pivot = QuickSortPartition(arr, left, right);

                if (pivot > 1)
                    QuickSortRecursive(arr, left, pivot - 1);

                if (pivot + 1 < right)
                    QuickSortRecursive(arr, pivot + 1, right);
            }
        }
        #endregion
        #endregion

        #region Efficient Sorts For Array Of Ints
        #region Radix Sort
        /// <summary>
        /// This Sort is specially effeciency for array of ints, the efficiency can change by setting bitsOfGroup, try to set this also to 2, 8 or 16 to see if it is quicker or not.
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="bitsOfGroup"></param>
        public static void RadixSort(int[] datas, int bitsOfGroup = 4)
        {
            int[] tmpDatas = new int[datas.Length];
            int[] count = new int[1 << bitsOfGroup];
            int[] pref = new int[1 << bitsOfGroup];

            int bitOfAnInt = 32;
            int groups = (int)Math.Ceiling((double)bitOfAnInt / (double)bitsOfGroup);
            int mask = (1 << bitsOfGroup) - 1;

            for (int c = 0, shift = 0; c < groups; c++, shift += bitsOfGroup)
            {
                for (int i = 0; i < count.Length; i++)
                    count[i] = 0;

                for (int i = 0; i < datas.Length; i++)
                    count[(datas[i] >> shift) & mask]++;

                pref[0] = 0;
                for (int i = 1; i < count.Length; i++)
                    pref[i] = pref[i - 1] + count[i - 1];

                for (int i = 0; i < datas.Length; i++)
                    tmpDatas[pref[(datas[i] >> shift) & mask]++] = datas[i];

                tmpDatas.CopyTo(datas, 0);
            }
        }
        #endregion
        #endregion

        #region Less Efficient Sorts
        #region InsertionSort
        /// <summary>
        /// Equivalent to a Bubble sort.
        /// </summary>
        /// <typeparam name="TypeToSort"></typeparam>
        /// <param name="datas"></param>
        public static void InsertionSort(TypeToSort[] datas)
        {
            for (int i = 0; i < datas.Length - 1; i++)
                for (int j = i + 1; j > 0; j--)
                    if (Comparer<TypeToSort>.Default.Compare(datas[j - 1], datas[j]) > 0)
                        Swap<TypeToSort>(datas, j - 1, j);
        }
        #endregion

        #region BubbleSort
        /// <summary>
        /// Simple sort to implement however it is a very bad one : Memory :n, Best case : n if the data are already sorted, Average case : n², Worst case : n².
        /// </summary>
        /// <typeparam name="TypeToSort"></typeparam>
        /// <param name="datas"></param>
        public static void BubbleSort(TypeToSort[] datas)
        {
            for (int i = 0; i < datas.Length; i++)
                for (int j = 0; j < datas.Length; j++)
                    if (Comparer<TypeToSort>.Default.Compare(datas[j], datas[i]) > 0)
                        Swap<TypeToSort>(datas, i, j);
        }
        #endregion

        #region SelectionSort
        /// <summary>
        /// One of the worst sort, n² of complexity in all case, memory : 1.
        /// </summary>
        /// <typeparam name="TypeToSort"></typeparam>
        /// <param name="datas"></param>
        public static void SelectionSort(TypeToSort[] datas)
        {
            int minPos;

            for (int i = 0; i < datas.Length - 1; i++)
            {
                minPos = i;

                for (int j = i + 1; j < datas.Length; j++)
                    if (Comparer<TypeToSort>.Default.Compare(datas[j], datas[minPos]) < 0)
                        minPos = j;

                if (minPos != i)
                    Swap<TypeToSort>(datas, minPos, i);
            }
        }
        #endregion
        #endregion

        #region Utilities
        private static void Swap<TypeToSwap>(TypeToSwap[] datas, int indexOfFirstData, int indexOfSecondData)
        {
            TypeToSwap temp = datas[indexOfFirstData];

            datas[indexOfFirstData] = datas[indexOfSecondData];
            datas[indexOfSecondData] = temp;
        }
        #endregion

        private static int whenMergeSortCostToMuchMemory = 16332;     /* Arbitrary Value */

        #region Choose Or Find Automatically The Most Adapted & Fastest Sort !
        /// <summary>
        /// this methods will calcul and display the fastest sort for all kind of datas. (if datas is an int, you shouldn't use this method).
        /// </summary>
        /// <typeparam name="TypeToSort"></typeparam>
        /// <param name="datas"></param>
        /// <param name="fastestSort"></param>
        /// <param name="ticks"></param>
        /// <param name="sortHistory"></param>
        public static void FindTheFastestSortThenDisplayAllTheirTimes<TypeToSort>(
            TypeToSort[] datas,
            string fastestSort = "",
            long ticks = 9223372036854775807,
            string sortHistory = "")
            where TypeToSort : new()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            TypeToSort[] tmpDatas = new TypeToSort[datas.Length];

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.MergeSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Merge";
            }
            sortHistory += string.Format("Merge Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.QuickSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Quick";
            }
            sortHistory += string.Format("Quick Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.HeapSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Heap";
            }
            sortHistory += string.Format("Heap Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.BubbleSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Bubble";
            }
            sortHistory += string.Format("Bubble Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.InsertionSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Insertion";
            }
            sortHistory += string.Format("Insertion Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            SortHelper<TypeToSort>.SelectionSort(tmpDatas);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Selection";
            }
            sortHistory += string.Format("Selection Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            Debug.Log(string.Format("The fastest sort is {0} and cost {1} ticks.\n*****History*****\n{2}", fastestSort, ticks, sortHistory));
            stopwatch.Reset();
        }

        /// <summary>
        /// This methods will will calcul the fastest sort for integers datas.
        /// </summary>
        /// <param name="datas"></param>
        public static void FindTheFastestIntegerSortThenDisplayAllTheirTimes(int[] datas)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            int[] tmpDatas = new int[datas.Length];
            string fastestSort = "";
            string sortHistory = "";
            long ticks = 9223372036854775807;

            //FindFastestSortCodeReducer<int>(datas, tmpDatas, ref stopwatch, new Action(() => { RadixSort(tmpDatas, 64); }), ref ticks, ref fastestSort, ref sortHistory, "Radix64");
            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            RadixSort(tmpDatas, 4);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Radix4";
            }
            sortHistory += string.Format("Radix4 Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            RadixSort(tmpDatas, 8);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Radix8";
            }
            sortHistory += string.Format("Radix8 Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            RadixSort(tmpDatas, 16);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Radix16";
            }
            sortHistory += string.Format("Radix16 Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            RadixSort(tmpDatas, 32);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Radix32";
            }
            sortHistory += string.Format("Radix32 Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            RadixSort(tmpDatas, 64);
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = "Radix64";
            }
            sortHistory += string.Format("Radix64 Sort Time : {0} ticks\n", stopwatch.ElapsedTicks);
            stopwatch.Reset();

            FindTheFastestSortThenDisplayAllTheirTimes<int>(datas, fastestSort, ticks, sortHistory);
        }

        /// <summary>
        /// This methods allow to make a benchmark with a single line however I don't use it because it add too much times to each test.
        /// </summary>
        /// <typeparam name="TypeToTest"></typeparam>
        /// <param name="datas"></param>
        /// <param name="tmpDatas"></param>
        /// <param name="stopwatch"></param>
        /// <param name="actionToPerform"></param>
        /// <param name="ticks"></param>
        /// <param name="fastestSort"></param>
        /// <param name="sortHistory"></param>
        /// <param name="sortName"></param>
        private static void FindFastestSortCodeReducer<TypeToTest>(
            TypeToTest[] datas,
            TypeToTest[] tmpDatas,
            System.Diagnostics.Stopwatch stopwatch,
            Action actionToPerform,
            ref long ticks,
            ref string fastestSort,
            ref string sortHistory,
            string sortName)
        {
            Array.Copy(datas, tmpDatas, datas.Length);
            stopwatch.Start();
            actionToPerform();
            stopwatch.Stop();
            if (ticks > stopwatch.ElapsedTicks)
            {
                ticks = stopwatch.ElapsedTicks;
                fastestSort = sortName;
            }
            sortHistory += string.Format("{0} Sort Time : {1} ticks\n", sortName, stopwatch.ElapsedTicks);
            Debug.Log(sortHistory);
            stopwatch.Reset();
        }

        /// <summary>
        /// This method will find and perform the best tree in functions of your datas are sorted or not, the length of your datas, etc... : 
        /// Merge sort is the fastest sort however it cost a lot of memory, so if you have a lot datas or your data class are very heavy, this sort is not recommand.
        /// Quick sort is a good sort but when your datas are already sorted, it's complexity is n² (worst complexity case) so don't choose it if your datas are already sorted.
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="dataAreSorted"></param>
        public static void ChooseAutomaticallyTheMostAdaptedSort(TypeToSort[] datas, bool dataAreSorted)
        {
            int datasMemoryUsed = 0;    /* datasMemoryUsed ~= datas.length * sizeof(TypeToSort) */

            Array.ForEach(datas, delegate (TypeToSort data) { datasMemoryUsed += Marshal.SizeOf(data); });

            if (datasMemoryUsed < whenMergeSortCostToMuchMemory)
                MergeSort(datas);
            else if (!dataAreSorted)
                QuickSort(datas);
            else
                HeapSort(datas);
        }
        #endregion
    }
}