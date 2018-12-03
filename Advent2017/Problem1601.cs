using Utils;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Advent2017
{
    public class Problem1601
    {
        char[] Shift(int request, char[] buffer)
        {
            var r = new char[buffer.Length];

            for (var i = 0; i < request; i++)
                r[i] = buffer[i + (buffer.Length - request)];

            for (var i = 0; i < buffer.Length - request; i++)
                r[i + request] = buffer[i]; 

            return r;
        }

        char[] Exchange(string request, char[] current)
        {
            var parts = request.Split('/');
            var a = int.Parse(parts[0]);
            var b = int.Parse(parts[1]);

            var t = current[b];
            current[b] = current[a];
            current[a] = t;

            return current;
        }

        char[] ExchangeByName(string request, char[] current)
        {
            var parts = request.Split('/');
            var ac = parts[0][0];
            var bc = parts[1][0];
            int a = -1;
            int b = -1;

            for (var i = 0; i < current.Length && (a == -1 || b == -1); i++)
            {
                if (current[i] == ac) a = i;
                if (current[i] == bc) b = i;
            }

            var t = current[b];
            current[b] = current[a];
            current[a] = t;

            return current;
        }

        char[] NextPos(string action, char[] current)
        {
            switch (action[0])
            {
                case 's':
                    return Shift(int.Parse(action.Substring(1)), current);

                case 'x':
                    return Exchange(action.Substring(1), current);

                case 'p':
                    return ExchangeByName(action.Substring(1), current);
            }

            Oh.ShttingHell();
            return null;
        }

        class CacheData
        {
            public readonly string Result;
            public readonly ulong FirstSeen;
            
            public CacheData(string result, ulong firstSeen)
            {
                Result = result;
                FirstSeen = firstSeen;
            } 
        }

        [Theory]
        [InlineData("Data/1601-example.txt", "abcde", "baedc", 1)]
        [InlineData("Data/1601.txt", "abcdefghijklmnop", "iabmedjhclofgknp", 1)]
        [InlineData("Data/1601-example.txt", "abcde", "ceadb", 2)]
        [InlineData("Data/1601.txt", "abcdefghijklmnop", "oildcmfeajhbpngk", 1000000000)]
        void Solve(string filename, string starting, string ending, ulong times)
        {
            var moves = FileIterator.LoadCSV<string>(filename);
            var result = starting;
            var cache = new Dictionary<string, CacheData>();
            var foundCycle = false;

            for (ulong i = 0; i < times; i++)
            {
                CacheData cacheHit;
                if (cache.TryGetValue(result, out cacheHit) == false)
                {
                    var current = result.ToCharArray();
                    for (var step = 0; step < moves.Length; step++)
                    {
                        current = NextPos(moves[step], current);
                    }
                    cacheHit = new CacheData(new string(current), i);
                    cache[result] = cacheHit;
                }
                else if (!foundCycle)
                {
                    cacheHit.FirstSeen.Should().Be(0);
                    times = times % i;
                    i = 0;
                    foundCycle = true;
                }

                result = cacheHit.Result;
            }

            result.Should().Be(ending);
        }
    }
}
