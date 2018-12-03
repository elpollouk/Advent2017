using System;
using System.Collections.Generic;

namespace Utils.Alogrithms
{
    static class HeapSort
    {
        public static void Sort<T>(IList<T> collection, Func<T, T, int> compare)
        {
            BuildHeap(collection, compare);

            for (var i = collection.Count - 1; i > 0; i--)
            {
                // Move the highest index from the start of the array to where the end of the unsorted list is
                Swap(collection, 0, i);

                // Rebuild the heap
                var index = 0;
                while (true)
                {
                    var leftIndex = Left(index);
                    var rightIndex = Right(index);
                    if (ShouldPromote(collection, compare, i, index, leftIndex) || ShouldPromote(collection, compare, i, index, rightIndex))
                    {
                        if (ShouldPromote(collection, compare, i, leftIndex, rightIndex))
                        {
                            Swap(collection, rightIndex, index);
                            index = rightIndex;
                        }
                        else
                        {
                            Swap(collection, leftIndex, index);
                            index = leftIndex;
                        }
                    }
                    else break;
                }

            }
        }

        private static void BuildHeap<T>(IList<T> collection, Func<T, T, int> compare)
        {
            for (var heapSize = 0; heapSize < collection.Count; heapSize++)
            {
                var index = heapSize;
                while (index != 0)
                {
                    var parentIndex = Parent(index);
                    if (!ShouldPromote(collection, compare, heapSize + 1, parentIndex, index))
                        break;

                    Swap(collection, parentIndex, index);
                    index = parentIndex;
                }
            }
        }

        private static void Swap<T>(IList<T> collection, int index1, int index2)
        {
            var t = collection[index1];
            collection[index1] = collection[index2];
            collection[index2] = t;
        }

        private static int Parent(int index) => index / 2;
        private static int Left(int index) => index * 2;
        private static int Right(int index) => (index * 2) + 1;

        private static bool ShouldPromote<T>(IList<T> collection, Func<T, T, int> compare, int limit, int higherIndex, int lowerIndex)
        {
            if (lowerIndex == 0) return false;
            if (limit <= lowerIndex) return false;
            return compare(collection[higherIndex], collection[lowerIndex]) < 0;
        }
    }
}
