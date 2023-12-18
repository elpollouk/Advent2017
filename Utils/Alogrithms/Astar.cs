using Utils.DataStructures;
using System.Collections.Generic;

namespace Utils.Alogrithms
{
    public static class Astar
    {
        public interface IGraphAdapter<Node>
        {
            // Return list of nodes connected to the specified node
            IEnumerable<Node> GetLinked(Node node);
            // Return the cost of moving from one node to another
            // Usually 1 for simple grids
            int GetMoveCost(Node from, Node to);
            // Get rough estimated score for navigating from one node to another
            // Lower is better, Manhattan distance is a suitable score for simple grids
            // A score of 0 means the nodes are equal
            int GetScore(Node from, Node to);
        }

        public static IList<Node> FindPath<Node>(IGraphAdapter<Node> graph, Node start, Node goal)
        {
            var searchSpace = new PriorityQueue<Node>();
            var pathMap = new Dictionary<Node, Node>();
            var costSoFar = new Dictionary<Node, int>();

            searchSpace.Enqueue(start, 0);
            costSoFar[start] = 0;

            while (searchSpace.Count != 0)
            {
                var current = searchSpace.Dequeue();
                if (graph.GetScore(current, goal) == 0)
                {
                    // We need to substitute the current node for the goal as although the current node might be
                    // equivalent to the goal, the goal itself might not be in the path map if reaching it only
                    // requires partial matching of the node state.
                    goal = current;
                    break;
                }

                var linked = graph.GetLinked(current);
                foreach (var linkedNode in linked)
                {
                    var newCost = costSoFar[current] + graph.GetMoveCost(current, linkedNode);
                    if (newCost < costSoFar.GetOrDefault(linkedNode, int.MaxValue))
                    {
                        costSoFar[linkedNode] = newCost;
                        searchSpace.Enqueue(linkedNode, newCost + graph.GetScore(linkedNode, goal));
                        pathMap[linkedNode] = current;
                    }
                }
            }

            return ResolvePath(pathMap, start, goal);
        }

        private static IList<Node> ResolvePath<Node>(Dictionary<Node, Node> pathMap, Node start, Node goal)
        {
            var comparer = EqualityComparer<Node>.Default;
            var path = new List<Node>();
            if (!pathMap.ContainsKey(goal)) return null;

            var node = goal;
            do
            {
                path.Add(node);
                node = pathMap[node];
            }
            while (!comparer.Equals(node, start));
            path.Add(node);
            path.Reverse();

            return path;
        }
    }
}
