using System;
using System.Collections.Generic;
using System.Linq;

namespace Adevent2017.DataStructures
{
    class PriorityQueue<T, PriorityType> where PriorityType : IComparable<PriorityType>
    {
        private struct QueueElement
        {
            public readonly PriorityType priority;
            public readonly T item;

            public QueueElement(PriorityType priority, T item)
            {
                this.priority = priority;
                this.item = item;
            }
        }

        private List<QueueElement> _queue = new List<QueueElement>();

        public int Count => _queue.Count;

        public void Enqueue(T item, PriorityType priority)
        {
            for (var i = 0; i < _queue.Count; i++)
            {
                if (_queue[i].priority.CompareTo(priority) >= 0)
                {
                    _queue.Insert(i, new QueueElement(priority, item));
                    return;
                }
            }

            _queue.Add(new QueueElement(priority, item));
        }

        public T Dequeue()
        {
            var el = _queue.First();
            _queue.RemoveAt(0);
            return el.item;
        }
    }

    class PriorityQueue<T> : PriorityQueue<T, int> { }
}
