using Adevent2017.Alogrithms;
using Adevent2017.DataStructures;
using System.Collections.Generic;

namespace Adevent2017.Utils
{
    abstract class StandardGraphAdapterBase<T> : Astar.IGraphAdapter<T> where T : class
    {
        private readonly Graph<T> _graph;

        public StandardGraphAdapterBase(Graph<T> graph)
        {
            _graph = graph;
        }

        public IEnumerable<T> GetLinked(T node) => _graph.GetLinked(node);

        public virtual int GetMoveCost(T from, T to) => 1;

        public abstract int GetScore(T from, T to);
    }
}
