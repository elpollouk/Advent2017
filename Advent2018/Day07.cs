using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utils;
using Utils.DataStructures;
using Xunit;

namespace Advent2018
{
    public class Day07
    {
        class Node
        {
            public Node(char value)
            {
                Value = value;
                Done = false;
            }

            public readonly char Value;
            public bool Done;
            public List<Node> Parents = new List<Node>();
            public List<Node> Children = new List<Node>();
        }

        static Regex s_OrderReg = new Regex(@" (.) ");
        (char Parent, char Child) ParseOrder(string input)
        {
            var matches = s_OrderReg.Matches(input);
            if (matches.Count != 2) Oh.Bugger();
            return (matches[0].Groups[1].Value[0], matches[1].Groups[1].Value[0]);
        }

        PriorityQueue<Node, char> BuildGraph(string inputFile)
        {
            var dependencies = FileIterator.Lines(inputFile).Select(l => ParseOrder(l));

            var graph = new Dictionary<char, Node>();
            foreach (var (Parent, Child) in dependencies)
            {
                if (!graph.ContainsKey(Parent))
                    graph[Parent] = new Node(Parent);
                if (!graph.ContainsKey(Child))
                    graph[Child] = new Node(Child);

                graph[Parent].Children.Add(graph[Child]);
                graph[Child].Parents.Add(graph[Parent]);
            }

            var queue = new PriorityQueue<Node, char>();
            foreach (var item in graph.Values)
                if (item.Parents.Count == 0)
                    queue.Enqueue(item, item.Value);

            return queue;
        }

        [Theory]
        [InlineData('C', 'A', "Step C must be finished before step A can begin.")]
        [InlineData('F', 'E', "Step F must be finished before step E can begin.")]
        public void Test_ParseOrder(char parent, char child, string input) => ParseOrder(input).Should().Be((parent, child));

        [Theory]
        [InlineData("CABDFE", "Data/Day07-Test1.txt")]
        [InlineData("CABEFGD", "Data/Day07-Test2.txt")]
        [InlineData("FDSEGJLPKNRYOAMQIUHTCVWZXB", "Data/Day07.txt")]
        public void Problem1_Solve(string answer, string inputFile)
        {
            var solution = "";
            var queue = BuildGraph(inputFile);

            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                node.Done = true;
                solution += node.Value;

                foreach (var child in node.Children)
                    if (child.Parents.All(p => p.Done))
                        queue.Enqueue(child, child.Value);
            }

            solution.Should().Be(answer);
        }
    }
}
