using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day03
    {
        class Request
        {
            public readonly int Id;
            public readonly int[] Pos;
            public readonly int[] Size;

            public Request(string request)
            {
                var split = request.Split('@', ',', ':', 'x');
                if (split.Length != 5)
                    Oh.Bugger();

                Id = int.Parse(split[0].Substring(1));
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

            public void Apply(Dictionary<ValueTuple<int, int>, List<int>> cloth, HashSet<int> safeRequests)
            {
                safeRequests.Add(Id);

                for (var y = 0; y < Size[1]; y++)
                    for (var x = 0; x < Size[0]; x++)
                    {
                        var pos = new ValueTuple<int, int>(Pos[0] + x, Pos[1] + y);
                        var list = cloth.GetOrCreate(pos, () => new List<int>());
                        list.Add(Id);
                        if (list.Count != 1)
                            foreach (var request in list)
                                safeRequests.Remove(request);
                    }
            }
        }

        [Theory]
        [InlineData(4, 3, "#1 @ 1,3: 4x4", "#2 @ 3,1: 4x4", "#3 @ 5,5: 2x2")]
        public void Problem_Test(int expectedInches, int expectedUniqueRequest, params string[] requests)
        {
            var cloth = new Dictionary<ValueTuple<int, int>, List<int>>();
            var safeRequests = new HashSet<int>();

            foreach (var request in requests)
            {
                var r = new Request(request);
                r.Apply(cloth, safeRequests);
            }

            var overlaps = 0;

            foreach (var value in cloth.Values)
                if (value.Count > 1)
                    overlaps++;

            overlaps.Should().Be(expectedInches);

            safeRequests.Count.Should().Be(1);
            safeRequests.First().Should().Be(expectedUniqueRequest);
        }

        [Fact]
        public void Problem_Solution()
        {
            var requests = FileIterator.LoadLines<string>("Data/Day03.txt");
            Problem_Test(104126, 695, requests);
        }
    }
}
