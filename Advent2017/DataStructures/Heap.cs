﻿using System;
using System.Collections.Generic;

namespace Adevent2017.DataStructures
{
    class Heap<Key, Item> where Key : IComparable<Key>
    {
        private class HeapElement
        {
            public readonly Key Key;
            public readonly Item Item;

            public HeapElement(Key key, Item item)
            {
                this.Key = key;
                this.Item = item;
            }

            public override string ToString()
            {
                return $"Key: {Key}, Item: {Item}";
            }
        }

        private List<HeapElement> _heap = new List<HeapElement>();

        public int Count => _heap.Count;

        public void Push(Key key, Item item)
        {
            _heap.Add(new HeapElement(key, item));
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
            var item = _heap[0].Item;
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

        private bool ShouldPromote(int higher, int lower)
        {
            if (lower == 0) return false;
            if (lower >= _heap.Count) return false;

            return _heap[higher].Key.CompareTo(_heap[lower].Key) > 0;
        }
    }
}
