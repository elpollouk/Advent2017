using System.Collections.Generic;

namespace Adevent2017.Alogrithms
{
    static class Astar
    {
        public interface IGraph<Node> where Node : class
        {
            IEnumerable<Node> GetChildrean(Node node);
            int GetMoveCost(Node from, Node to);
            int GetScore(Node from, Node to);
        }

        public static IEnumerable<Node> FindPath<Node>(IGraph<Node> graph, Node start, Node goal) where Node : class
        {
            var searchSpace = new PriorityQueue<Node>();
            var pathMap = new Dictionary<Node, Node>();
            var costSoFar = new Dictionary<Node, int>();

            searchSpace.Enqueue(start, 0);
            costSoFar[start] = 0;

            while (searchSpace.Count != 0)
            {
                var current = searchSpace.Dequeue();
                if (current == goal) break;

                var children = graph.GetChildrean(current);
                foreach (var child in children)
                {
                    var newCost = costSoFar[current] + graph.GetMoveCost(current, child);
                    searchSpace.Enqueue(child, newCost + graph.GetScore(child, goal));
                    pathMap[child] = current;
                }
            }

            return ResolvePath(pathMap, start, goal);
        }

        private static IEnumerable<Node> ResolvePath<Node>(Dictionary<Node, Node> pathMap, Node start, Node goal) where Node : class
        {
            var path = new LinkedList<Node>();
            if (!pathMap.ContainsKey(goal)) return null;

            var node = goal;
            do
            {
                path.AddFirst(node);
                node = pathMap[node];
            }
            while (node != start);
            path.AddFirst(node);

            return path;
        }
    }
}
