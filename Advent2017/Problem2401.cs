using Utils;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;
using System;

namespace Advent2017
{
    public class Problem2401
    {
        class Connector
        {
            public Connector(int id, int input, int output)
            {
                Id = id;
                Input = input;
                Output = output;
                Value = input + output;
            }

            public override string ToString()
            {
                return $"{Input}/{Output}";
            }

            public readonly int Id;
            public readonly int Input;
            public readonly int Output;
            public readonly int Value;
        }

        Dictionary<int, List<Connector>> LoadConnectors(string datafile)
        {
            var id = 0;
            var connectors = new Dictionary<int, List<Connector>>();
            FileIterator.ForEachLine(datafile, (string line) =>
            {
                var sockets = line.Split('/');
                var input = int.Parse(sockets[0]);
                var output = int.Parse(sockets[1]);

                id++;
                var connector = new Connector(id, input, output);
                var outputs = connectors.GetOrCreate(input, () => new List<Connector>());
                outputs.Add(connector);

                if (connector.Input != connector.Output)
                {
                    connector = new Connector(id, output, input);
                    outputs = connectors.GetOrCreate(output, () => new List<Connector>());
                    outputs.Add(connector);
                }
            });

            return connectors;
        }

        private HashSet<int> _used;

        void Walk(Dictionary<int, List<Connector>> connectors, int output, int count, int currentValue, Action<int, int> stateCallback)
        {
            stateCallback(count, currentValue);

            var validConnectors = connectors.GetOrDefault(output);
            if (validConnectors == null) return;

            for (var i = 0; i < validConnectors.Count; i++)
            {
                var connector = validConnectors[i];
                var id = connector.Id;
                if (!_used.Contains(id))
                {
                    _used.Add(id);
                    Walk(connectors, connector.Output, count + 1, currentValue + connector.Value, stateCallback);
                    _used.Remove(id);
                }
            }
        }

        int MaximumBridge(string datafile)
        {
            _used = new HashSet<int>();
            var connectors = LoadConnectors(datafile);
            int maxValue = 0;

            Walk(connectors, 0, 0, 0, (count, value) => {
                if (maxValue < value)
                    maxValue = value;
            });

            return maxValue;
        }

        int LongestBridge(string datafile)
        {
            _used = new HashSet<int>();
            var connectors = LoadConnectors(datafile);
            int longest = 0;
            int maxValue = 0;

            Walk(connectors, 0, 0, 0, (count, value) => {
                if (count == longest && maxValue < value)
                {
                    maxValue = value;
                }
                else if (longest < count)
                {
                    longest = count;
                    maxValue = value;
                }
            });

            return maxValue;
        }

        [Theory]
        [InlineData("Data/2401-example.txt", 31)]
        [InlineData("Data/2401.txt", 1940)]
        public void Part1(string datafile, int answer) => MaximumBridge(datafile).Should().Be(answer);

        [Theory]
        [InlineData("Data/2401-example.txt", 19)]
        [InlineData("Data/2401.txt", 1928)]
        public void Part2(string datafile, int answer) => LongestBridge(datafile).Should().Be(answer);
    }
}
