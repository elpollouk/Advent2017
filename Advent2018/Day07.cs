using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            }

            public readonly char Value; 
            public List<char> Parents = new List<char>();
            public List<char> Children = new List<char>();
        }

        static Regex s_OrderReg = new Regex(@" (.) ");
        (char Parent, char Child) ParseOrder(string input)
        {
            var matches = s_OrderReg.Matches(input);
            if (matches.Count != 2) Oh.Bugger();
            return (matches[0].Groups[1].Value[0], matches[1].Groups[1].Value[0]);
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
            var dependencies = FileIterator.Lines(inputFile).Select(l => ParseOrder(l));

            var graph = new Dictionary<char, Node>();
            foreach (var (Parent, Child) in dependencies)
            {
                if (!graph.ContainsKey(Parent))
                    graph[Parent] = new Node(Parent);
                if (!graph.ContainsKey(Child))
                    graph[Child] = new Node(Child);

                graph[Parent].Children.Add(Child);
                graph[Child].Parents.Add(Parent);
            }

            var solution = "";
            var done = new HashSet<char>();
            var queue = new PriorityQueue<char, char>();
            foreach (var item in graph.Values)
                if (item.Parents.Count == 0)
                    queue.Enqueue(item.Value, item.Value);

            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                if (done.Contains(node))
                    continue;

                var allParentsDone = true;
                foreach (var parent in graph[node].Parents)
                    if (!done.Contains(parent))
                        allParentsDone = false;

                if (!allParentsDone)
                    continue;

                done.Add(node);
                solution += node;
                foreach (var child in graph[node].Children)
                    queue.Enqueue(child, child);
            }

            solution.Should().Be(answer);
        }
    }
}
