using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0701
    {
        public class Node
        {
            public string Name;
            public string Parent;
            public string[] ChildNames;
            public List<Node> ChildNodes = new List<Node>();
            public int Weight;
            public int SummedWeight;
        }

        // I can't think of a better name at this time of night
        class SumCache
        {
            public int Count;
            public Node Node;
        }

        Node ParseLine(string line)
        {
            // Bollocks to regex
            var backFront = line.Split('>');
            var name = backFront[0].Split(' ')[0];
            var weight = backFront[0].Split('(')[1].Split(')')[0];
        
            // Does it have children?
            string[] children = new string[0];
            if (backFront.Length == 2)
                children = backFront[1].Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return new Node()
            {
                Name = name,
                ChildNames = children,
                Weight = int.Parse(weight),
                SummedWeight = 0
            };
        }

        Node FindRoot(Dictionary<string, Node> tree)
        {
            foreach (var node in tree.Values)
                if (node.Parent == null)
                    return node;

            throw new Exception("It's fucked");
        }

        int SumChildren(Node node)
        {
            if (node.SummedWeight != 0) return node.SummedWeight;

            var sum = node.Weight;
            foreach (var child in node.ChildNodes)
            {
                sum += SumChildren(child);
            }

            node.SummedWeight = sum;

            return sum;
        }

        int FindWeightAdjustment(Node node)
        {
            // We're looking for the most leafy answer it turns out
            foreach (var child in node.ChildNodes)
            {
                var adjustment = FindWeightAdjustment(child);
                if (adjustment != 0) return adjustment;
            }

            var sums = new Dictionary<int, SumCache>();

            foreach (var child in node.ChildNodes)
            {
                var sum = SumChildren(child);
                if (!sums.ContainsKey(sum)) sums[sum] = new SumCache(){ Count = 0, Node = child };
                sums[sum].Count++;
            }

            var oddWeight = -1;
            var expectedWeight = -1;
            foreach (var sum in sums.Keys)
            {
                if (sums[sum].Count == 1)
                    oddWeight = sum;
                else
                    expectedWeight = sum;
            }

            if (oddWeight != -1)
            {
                var adjustment = (expectedWeight - oddWeight);
                return sums[oddWeight].Node.Weight += adjustment;
            }

            return 0;
        }

        int FindFixedWeight(Node tree)
        {
            return FindWeightAdjustment(tree);
        }

        public Node BuildTree(string[] input)
        {
            var tree = new Dictionary<string, Node>();
            foreach (var line in input)
            {
                var node = ParseLine(line);
                tree[node.Name] = node;
            }

            // Resolve
            foreach (var node in tree.Values)
            {
                foreach (var child in node.ChildNames)
                {
                    var childNode = tree[child];
                    childNode.Parent = node.Name;
                    node.ChildNodes.Add(childNode);
                }
            }

            return FindRoot(tree);
        }

        [Theory]
        [InlineData("Data/0701-example.txt", "tknk")]
        [InlineData("Data/0701.txt", "azqje")]
        public void Part1(string input, string answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            var tree = BuildTree(lines);
            tree.Name.Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/0701-example.txt", 60)]
        [InlineData("Data/0701.txt", 646)]
        public void Part2(string input, int answer)
        {
            var lines = FileIterator.LoadLines<string>(input);
            var tree = BuildTree(lines);
            FindFixedWeight(tree).Should().Be(answer);
        }
    }
}
