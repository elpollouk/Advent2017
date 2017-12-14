using System.Collections.Generic;

namespace Adevent2017.DataStructures
{
    public class GraphNode<T>
    {
        public GraphNode(int id) { Id = id; }
        public readonly int Id;
        public GraphNode<T> Parent = null;
        public readonly List<int> Links = new List<int>();
        public T Item = default(T);
    }
}
