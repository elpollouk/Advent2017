using Utils;
using FluentAssertions;
using Xunit;
using System.Collections.Generic;

namespace Advent2017
{
    public class Problem2401
    {
        struct Connector
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

            foreach (var outputs in connectors.Values)
                outputs.Sort((a, b) => b.Output - a.Output);

            return connectors;
        }

        private HashSet<int> _used;

        void WalkMaximum(Dictionary<int, List<Connector>> connectors, int output, int currentValue, ref int maxValue)
        {
            var validConnectors = connectors.GetOrDefault(output);
            if (validConnectors == null) return;


            for (var i = 0; i < validConnectors.Count; i++)
            {
                var connector = validConnectors[i];
                var id = connector.Id;
                if (!_used.Contains(id))
                {
                    _used.Add(id);
                    currentValue += connector.Value;
                    if (maxValue < currentValue)
                        maxValue = currentValue;

                    WalkMaximum(connectors, connector.Output, currentValue, ref maxValue);

                    currentValue -= connector.Value;
                    _used.Remove(id);
                }
            }
        }

        int MaximumBridge(string datafile)
        {
            _used = new HashSet<int>();
            var connectors = LoadConnectors(datafile);
            int maxValue = 0;

            WalkMaximum(connectors, 0, 0, ref maxValue);

            return maxValue;
        }

        void WalkLongest(Dictionary<int, List<Connector>> connectors, int output, int count, int currentValue, ref int longest, ref int maxValue)
        {
            var validConnectors = connectors.GetOrDefault(output);
            if (validConnectors == null) return;

            for (var i = 0; i < validConnectors.Count; i++)
            {
                var connector = validConnectors[i];
                var id = connector.Id;
                if (!_used.Contains(id))
                {
                    _used.Add(id);
                    count++;
                    currentValue += connector.Value;
                    if (longest == count && maxValue < currentValue)
                    {
                        maxValue = currentValue;
                    }
                    else if (longest < count)
                    {
                        longest = count;
                        maxValue = currentValue;
                    }

                    WalkLongest(connectors, connector.Output, count, currentValue, ref longest, ref maxValue);

                    currentValue -= connector.Value;
                    count--;
                    _used.Remove(id);
                }
            }
        }

        int LongestBridge(string datafile)
        {
            _used = new HashSet<int>();
            var connectors = LoadConnectors(datafile);
            int longest = 0;
            int maxValue = 0;

            WalkLongest(connectors, 0, 0, 0, ref longest, ref maxValue);

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
