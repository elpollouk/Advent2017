using Utils.Alogrithms;
using Utils.DataStructures;
using System.Collections.Generic;

namespace Utils
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

        public abstract bool NodesEqual(T a, T b);
    }
}
