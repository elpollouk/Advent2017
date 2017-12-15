using Adevent2017.DataStructures;
using System.Collections.Generic;

namespace Adevent2017.Alogrithms
{
    abstract class StandardGraphAdapterBase<T> : Astar.IGraphAdapter<GraphNode<T>> where T : class
    {
        private readonly Graph<T> _graph;

        public StandardGraphAdapterBase(Graph<T> graph)
        {
            _graph = graph;
        }

        public IEnumerable<GraphNode<T>> GetLinked(GraphNode<T> node) => _graph.GetLinked(node.Id);

        public virtual int GetMoveCost(GraphNode<T> from, GraphNode<T> to) => 1;

        public abstract int GetScore(GraphNode<T> from, GraphNode<T> to);
    }
}
