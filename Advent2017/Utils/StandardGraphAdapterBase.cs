using Adevent2017.Alogrithms;
using Adevent2017.DataStructures;
using System.Collections.Generic;

namespace Adevent2017.Utils
{
    abstract class StandardGraphAdapterBase<T> : Astar.IGraphAdapter<T> where T : class
    {
        public readonly Graph<T> Graph;

        public StandardGraphAdapterBase(Graph<T> graph)
        {
            Graph = graph;
        }

        public IEnumerable<T> GetLinked(T node) => Graph.GetLinked(node);

        public virtual int GetMoveCost(T from, T to) => 1;

        public abstract int GetScore(T from, T to);
    }
}
