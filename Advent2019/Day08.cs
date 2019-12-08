using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day08
    {
        List<int[,]> BuildImageLayers(string input, int width, int height)
        {
            var layers = new List<int[,]>();
            var layer = new int[width, height];
            var x = 0;
            var y = 0;
            foreach (var c in input)
            {
                layer[x, y] = c - '0';
                if (++x == width)
                {
                    x = 0;
                    if (++y == height)
                    {
                        y = 0;
                        layers.Add(layer);
                        layer = new int[width, height];
                    }
                }
            }

            return layers;
        }

        int CountValue(int[,] values, int value)
        {
            var count = 0;
            foreach (var v in values)
                if (v == value)
                    count++;
            return count;
        }

        [Theory]
        [InlineData("Data/Day08-example.txt", 3, 2, 1)]
        [InlineData("Data/Day08-example2.txt", 2, 2, 4)]
        [InlineData("Data/Day08.txt", 25, 6, 2760)]
        public void Problem1(string input, int width, int height, int answer)
        {
            input = FileIterator.Lines(input).First();
            var layers = BuildImageLayers(input, width, height);
            var minZero = 0;
            var minNumZeros = CountValue(layers[0], 0);
            for (var i = 1; i < layers.Count; i++)
            {
                var numZeros = CountValue(layers[i], 0);
                if (numZeros < minNumZeros)
                {
                    minNumZeros = numZeros;
                    minZero = i;
                }
            }

            var numOnes = CountValue(layers[minZero], 1);
            var numTwos = CountValue(layers[minZero], 2);
            (numOnes * numTwos).Should().Be(answer);

            var image = new int[width, height];

            for (var i = layers.Count - 1; i >= 0; i--)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        if (layers[i][x, y] == 2) continue;
                        image[x, y] = layers[i][x, y];
                    }
                }
            }

            var output = "";
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    output += image[x, y] == 0 ? ' ' : '#';
                }
                output += "\n";
            }
            System.Diagnostics.Debug.Write(output);
        }
    }
}
