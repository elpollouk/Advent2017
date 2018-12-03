using FluentAssertions;
using Xunit;

namespace Advent2017
{
    public class Problem1501
    {
        const ulong SeedAExample = 65;
        const ulong SeedAActual = 289;
        const ulong SeedBExample = 8921;
        const ulong SeedBActual = 629;
        const ulong FactorA = 16807;
        const ulong FactorB = 48271;

        class Generator
        {
            ulong _factor;
            ulong _value;

            public Generator(ulong seed, ulong factor)
            {
                _factor = factor;
                _value = seed;
            }

            public ulong Next()
            {
                var v = _value * _factor;
                _value = v % 2147483647;
                return _value;
            }

            public ulong NextMultipleOf(ulong value)
            {
                ulong v;
                do
                {
                    v = Next();
                }
                while (v % value != 0);

                return v;
            }
        }

        [Theory]
        [InlineData(SeedAExample, FactorA, 1352636452)]
        [InlineData(SeedBExample, FactorB, 285222916)]
        void Part1Example(ulong seed, ulong factor, ulong answer)
        {
            var generator = new Generator(seed, factor);
            for (var i = 0; i < 4; i++)
                generator.Next();

            generator.Next().Should().Be(answer);
        }

        [Theory]
        [InlineData(5, SeedAExample, SeedBExample, 1)]
        //[InlineData(40000000, SeedAActual, SeedBActual, 638)]
        void Part1(int rounds, ulong seedA, ulong seedB, int answer)
        {
            var genA = new Generator(seedA, FactorA);
            var genB = new Generator(seedB, FactorB);
            var total = 0;

            for (var i = 0; i < rounds; i++)
            {
                var vA = genA.Next() & 0xFFFF;
                var vB = genB.Next() & 0xFFFF;
                if (vA == vB)
                    total++;
            }

            total.Should().Be(answer);
        }

        [Theory]
        [InlineData(SeedAExample, FactorA, 4, 740335192)]
        [InlineData(SeedBExample, FactorB, 8, 412269392)]
        void Part2Example(ulong seed, ulong factor, ulong multiple, ulong answer)
        {
            var generator = new Generator(seed, factor);
            for (var i = 0; i < 4; i++)
                generator.NextMultipleOf(multiple);

            generator.NextMultipleOf(multiple).Should().Be(answer);
        }


        //[Theory]
        //[InlineData(SeedAExample, SeedBExample, 309)]
        //[InlineData(SeedAActual, SeedBActual, 343)]
        void Part2(ulong seedA, ulong seedB, int answer)
        {
            var genA = new Generator(seedA, FactorA);
            var genB = new Generator(seedB, FactorB);
            var total = 0;

            for (var i = 0; i < 5000000; i++)
            {
                var vA = genA.NextMultipleOf(4) & 0xFFFF;
                var vB = genB.NextMultipleOf(8) & 0xFFFF;
                if (vA == vB)
                    total++;
            }

            total.Should().Be(answer);
        }
    }
}
