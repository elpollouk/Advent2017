using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Adevent2017
{
    public class Problem0701
    {
        class Node
        {
            public string Name;
            public string Parent;
            public string[] Children;
        }

        Node ParseLine(string line)
        {
            var backFront = line.Split('>');
            var name = backFront[0].Split(' ')[0];
            // Does it have childrean
            string[] children = new string[0];
            if (backFront.Length == 2)
                children = backFront[1].Split(new char[] { ',', ' '}, StringSplitOptions.RemoveEmptyEntries);

            return new Node()
            {
                Name = name,
                Children = children
            };
        }

        public string Solve(string[] input)
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
                foreach (var child in node.Children)
                    tree[child].Parent = node.Name;
            }

            // Search
            foreach (var node in tree.Values)
                if (node.Parent == null)
                    return node.Name;

            return "";
        }

        [Fact]
        public void Example1()
        {
            var lines = FileIterator.LoadLines<string>("Data/0701-example.txt");
            Solve(lines).Should().Be("tknk");
        }

        [Fact]
        public void Solution1()
        {
            var lines = FileIterator.LoadLines<string>("Data/0701.txt");
            Solve(lines).Should().Be("azqje");
        }
    }
}
