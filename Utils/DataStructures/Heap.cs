using System;
using System.Collections.Generic;

namespace Utils.DataStructures
{
    class Heap<Key, Item> where Key : IComparable<Key>
    {
        private List<KeyValuePair<Key, Item>> _heap = new List<KeyValuePair<Key, Item>>();
        private Func<int, int, bool> ShouldPromote;

        public Heap() : this(false)
        {
        }

        public Heap(bool maxHeap)
        {
            if (maxHeap)
                ShouldPromote = ShouldPromote_MaxHeap;
            else
                ShouldPromote = ShouldPromote_MinHeap;
        }

        public int Count => _heap.Count;

        public void Push(Key key, Item item)
        {
            _heap.Add(new KeyValuePair<Key, Item>(key, item));
            var index = _heap.Count - 1;
            while (index != 0)
            {
                var parentIndex = Parent(index);
                if (!ShouldPromote(parentIndex, index))
                    return;

                Swap(parentIndex, index);
                index = parentIndex;
            }
        }

        public Item Pop()
        {
            var item = _heap[0].Value;
            var lastIndex = _heap.Count - 1;
            var last = _heap[lastIndex];
            _heap[0] = last;
            _heap.RemoveAt(lastIndex);

            var index = 0;

            while (true)
            {
                var leftIndex = Left(index);
                var rightIndex = Right(index);
                if (ShouldPromote(index, leftIndex) || ShouldPromote(index, rightIndex))
                {
                    if (ShouldPromote(leftIndex, rightIndex))
                    {
                        Swap(rightIndex, index);
                        index = rightIndex;
                    }
                    else
                    {
                        Swap(leftIndex, index);
                        index = leftIndex;
                    }
                }
                else break;
            }

            return item;
        }

        private static int Parent(int index) => index / 2;
        private static int Left(int index) => index * 2;
        private static int Right(int index) => (index * 2) + 1;

        private void Swap(int i1, int i2)
        {
            var t = _heap[i1];
            _heap[i1] = _heap[i2];
            _heap[i2] = t;
        }

        private bool ShouldPromote_MinHeap(int higher, int lower)
        {
            if (lower == 0) return false;
            if (lower >= _heap.Count) return false;

            return _heap[higher].Key.CompareTo(_heap[lower].Key) > 0;
        }

        private bool ShouldPromote_MaxHeap(int higher, int lower)
        {
            if (lower == 0) return false;
            if (lower >= _heap.Count) return false;

            return _heap[higher].Key.CompareTo(_heap[lower].Key) < 0;
        }
    }
}
