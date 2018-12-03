using FluentAssertions;
using System;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day03
    {
        class Request
        {
            public readonly string Id;
            public readonly int[] Pos;
            public readonly int[] Size;

            public Request(string request)
            {
                var split = request.Split('@', ',', ':', 'x');
                if (split.Length != 5)
                    Oh.Bugger();

                Id = split[0].Trim();
                Pos = new int[]
                {
                    int.Parse(split[1]),
                    int.Parse(split[2])
                };
                Size = new int[]
                {
                    int.Parse(split[3]),
                    int.Parse(split[4])
                };
            }

            public void Apply(int[,] cloth)
            {
                for (var y = 0; y < Size[1]; y++)
                    for (var x = 0; x < Size[0]; x++)
                        cloth[Pos[0] + x, Pos[1] + y]++;
            }
        }

        [Theory]
        [InlineData(4, "#1 @ 1,3: 4x4", "#2 @ 3,1: 4x4", "#3 @ 5,5: 2x2")]
        public void Problem1_Test(int expectedInches, params string[] requests)
        {
            var cloth = new int[1000, 1000];
            foreach (var request in requests)
            {
                var r = new Request(request);
                r.Apply(cloth);
            }

            var overlaps = 0;
            foreach (var value in cloth)
                if (value > 1)
                    overlaps++;

            overlaps.Should().Be(expectedInches);
        }

        [Fact]
        public void Problem1_Solution()
        {
            var requests = FileIterator.LoadLines<string>("Day03.txt");
            Problem1_Test(104126, requests);
        }
    }
}
