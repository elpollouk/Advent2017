using Adevent2017.DataStructures;
using System;

namespace Adevent2017.Utils
{
    static class GraphBuilder
    {
        public static Graph<T> BuildGrid<T>(int width, int height, Action<int, int, GraphNode<T>> initNode = null)
        {
            if (initNode == null) initNode = (x, y, n) => { };
            Func<int, int, int> GetNodeId = (x, y) => (y * width) + x;
            var graph = new Graph<T>();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var nodeId = GetNodeId(x, y);
                    var node = graph.CreateNode(nodeId);
                    initNode(x, y, node);

                    if (x != 0) graph.AddTwoWayLink(nodeId, GetNodeId(x - 1, y));
                    if (y != 0) graph.AddTwoWayLink(nodeId, GetNodeId(x, y - 1));
                }
            }

            return graph;
        }
    }
}
