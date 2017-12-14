using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adevent2017.Alogrithms
{
    static class Astar<Node, Graph>
    {
        public interface IGraph
        {
            IList<Node> GetChildrean(Node node);
            int GetMoveCost(Node node1, Node node2);
            int GetScore(Node node);
        }

        public static IList<Node> FindPath(Graph graph, Node start, Node goal, Func<Graph, Node, IList<Node>> getChildren, )
        {
            var searchSpace = new PriorityQueue<Node>();
            var pathMap = new Dictionary<Node, Node>();
            var costSoFat = new Dictionary<Node, int>();

            return ResolvePath(pathMap, start, goal);
        }

        private static IList<Node> ResolvePath(Dictionary<Node, Node> pathMap, Node start, Node goal)
        {
            var path = new LinkedList<Node>();
            if (!pathMap.ContainsKey(goal)) return null;

            var node = goal;
            do
            {
                node = pathMap[node];
            }
            while (node != start);
        }
    }
}
