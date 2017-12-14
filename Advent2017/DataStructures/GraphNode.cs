using System.Collections.Generic;

namespace Adevent2017.DataStructures
{
    class GraphNode
    {
        public GraphNode(int id) { Id = id; }
        public readonly int Id;
        public readonly List<int> Links = new List<int>();
    }
}
