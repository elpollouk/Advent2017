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

            public int TimeRequired(int lag) => (Value - 'A') + 1 + lag;

            public readonly char Value;
            public bool Done;
            public List<Node> Parents = new List<Node>();
            public List<Node> Children = new List<Node>();

            public override string ToString() => $"'{Value}'";
        }

        class Worker
        {
            public int CompleteTime;
            public Node WorkItem;

            public override string ToString() => $"'{WorkItem}', CompleteTime={CompleteTime}";
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

        [Theory]
        [InlineData(15, 2, 0, "Data/Day07-Test1.txt")]
        [InlineData(258, 2, 60, "Data/Day07-Test1.txt")]
        [InlineData(1000, 5, 60, "Data/Day07.txt")]
        public void Problem2_Solve(int answer, int numWorkers, int lag, string inputFile)
        {
            var queue = BuildGraph(inputFile);
            var currentTime = 0;
            var workers = new Worker[numWorkers];
            for (var i = 0; i < workers.Length; i++)
                workers[i] = new Worker();

            do
            {
                // Complete current work
                foreach (var worker in workers.Where(w => w.CompleteTime <= currentTime && w.WorkItem != null))
                {
                    worker.WorkItem.Done = true;

                    foreach (var child in worker.WorkItem.Children)
                        if (child.Parents.All(p => p.Done))
                            queue.Enqueue(child, child.Value);

                    worker.WorkItem = null;
                }

                // Schedule out new work to available workers
                foreach (var worker in workers.Where(w => w.WorkItem == null))
                {
                    if (queue.Count == 0) break;
                    worker.WorkItem = queue.Dequeue();
                    worker.CompleteTime = currentTime + worker.WorkItem.TimeRequired(lag);
                }

                // Has all the work been done?
                var activeWorkers = workers.Where(w => w.WorkItem != null);
                if (activeWorkers.Count() == 0)
                    break;

                // Advance time
                currentTime = activeWorkers.Select(w => w.CompleteTime).Min();
            }
            while (true);

            (currentTime).Should().Be(answer);
        }
    }
}
