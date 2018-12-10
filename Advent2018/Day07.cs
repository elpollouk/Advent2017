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
        class WorkItem
        {
            public WorkItem(char value)
            {
                Value = value;
                Done = false;
            }

            public int TimeRequired => (Value - 'A') + 1;

            public readonly char Value;
            public bool Done;
            public readonly List<WorkItem> Parents = new List<WorkItem>();
            public readonly List<WorkItem> Children = new List<WorkItem>();

            public override string ToString() => $"'{Value}'";
        }

        class Job
        {
            public Job(WorkItem workItem, int lag)
            {
                WorkItem = workItem;
                CompleteTime = workItem.TimeRequired + lag;
            }

            public readonly int CompleteTime;
            public readonly WorkItem WorkItem;

            public override string ToString() => $"'{WorkItem}', CompleteTime={CompleteTime}";
        }

        static Regex s_OrderReg = new Regex(@" (.) ");
        (char Parent, char Child) ParseOrder(string input)
        {
            var matches = s_OrderReg.Matches(input);
            if (matches.Count != 2) Oh.Bugger();
            return (matches[0].Groups[1].Value[0], matches[1].Groups[1].Value[0]);
        }

        PriorityQueue<WorkItem, char> BuildGraph(string inputFile)
        {
            var dependencies = FileIterator.Lines(inputFile).Select(l => ParseOrder(l));

            var graph = new Dictionary<char, WorkItem>();
            foreach (var (Parent, Child) in dependencies)
            {
                if (!graph.ContainsKey(Parent))
                    graph[Parent] = new WorkItem(Parent);
                if (!graph.ContainsKey(Child))
                    graph[Child] = new WorkItem(Child);

                graph[Parent].Children.Add(graph[Child]);
                graph[Child].Parents.Add(graph[Parent]);
            }

            var queue = new PriorityQueue<WorkItem, char>();
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
            var currentTime = 0;
            var tasks = BuildGraph(inputFile);
            var inprogress = new PriorityQueue<Job>();

            while (tasks.Count != 0 || inprogress.Count != 0)
            {
                // Complete current work
                if (inprogress.Count != 0)
                {
                    var job = inprogress.Dequeue();
                    job.WorkItem.Done = true;

                    foreach (var child in job.WorkItem.Children)
                        if (child.Parents.All(p => p.Done))
                            tasks.Enqueue(child, child.Value);

                    currentTime = job.CompleteTime;
                }

                // Schedule out new work to available workers
                while (tasks.Count != 0 && inprogress.Count < numWorkers)
                {
                    var job = new Job(tasks.Dequeue(), currentTime + lag);
                    inprogress.Enqueue(job, job.CompleteTime);
                }
            }

            (currentTime).Should().Be(answer);
        }
    }
}
