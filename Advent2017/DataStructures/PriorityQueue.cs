using System;
using System.Collections.Generic;
using System.Linq;

namespace Adevent2017.DataStructures
{
    class PriorityQueue<T, PriorityType> where PriorityType : IComparable<PriorityType>
    {
        private Heap<PriorityType, T> _heap = new Heap<PriorityType, T>();

        public int Count => _heap.Count;

        public void Enqueue(T item, PriorityType priority)
        {
            _heap.Push(priority, item);
        }

        public T Dequeue()
        {
            return _heap.Pop();
        }
    }

    class PriorityQueue<T> : PriorityQueue<T, int> { }
}
