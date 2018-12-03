using Utils.DataStructures;
using System;
using System.Collections.Generic;

namespace Utils
{
    static class GraphBuilder
    {
        public static Graph<T> BuildGrid<T>(int width, int height, Func<int, int, T> newNode)
        {
            Func<int, int, int> GetNodeId = (x, y) => (y * width) + x;
            var graph = new Graph<T>();
            var itemMap = new Dictionary<int, T>();

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var nodeId = GetNodeId(x, y);
                    var node = newNode(x, y);
                    graph.AddNode(node);
                    itemMap[nodeId] = node;

                    if (x != 0) graph.AddTwoWayLink(node, itemMap[GetNodeId(x - 1, y)]);
                    if (y != 0) graph.AddTwoWayLink(node, itemMap[GetNodeId(x, y - 1)]);
                }
            }

            return graph;
        }
    }
}
