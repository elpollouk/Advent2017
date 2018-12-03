using Utils;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Advent2017
{
    public class Problem1301
    {
        int[] ParseLayers(string filename)
        {
            var intermediate = new List<Tuple<int, int>>();
            FileIterator.ForEachLine<string>(filename, line =>
            {
                var spec = line.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (spec.Length != 2) Oh.ForFucksSake();

                var range = int.Parse(spec[0]);
                var depth = int.Parse(spec[1]);
                intermediate.Add(new Tuple<int, int>(range, depth));
            });

            var r = new int[intermediate.Last().Item1 + 1];
            foreach (var t in intermediate)
                r[t.Item1] = t.Item2;

            return r;
        }

        int GetScore(int[] layers, int delay)
        {
            var total = 0;

            for (var i = 0; i < layers.Length; i++)
            {
                if (layers[i] == 0) continue;

                var period = (layers[i] - 1) * 2;
                var position = (delay + i) % period;
                if (position == 0)
                    if (delay == 0)
                        total += i * layers[i];
                    else
                        return 1;
            }

            return total;
        }

        [Theory]
        [InlineData("Data/1301-example.txt", 24)]
        [InlineData("Data/1301.txt", 2160)]
        public void Part1(string datafile, int answer)
        {
            var layers = ParseLayers(datafile);
            GetScore(layers, 0).Should().Be(answer);
        }

        [Theory]
        [InlineData("Data/1301-example.txt", 10)]
        [InlineData("Data/1301.txt", 3907470)]
        public void Part2(string datafile, int answer)
        {
            var layers = ParseLayers(datafile);

            var delay = 1;
            while (true)
            {
                if (GetScore(layers,delay) == 0)
                {
                    delay.Should().Be(answer);
                    return;
                }
                delay++;
            }
        }
    }
}
