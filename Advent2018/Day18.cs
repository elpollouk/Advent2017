using FluentAssertions;
using System.Collections.Generic;
using System.Security.Cryptography;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day18
    {
        enum CellState
        {
            Clear,
            Tree,
            LumberYard
        }

        class Simulation
        {
            public CellState[,] Environment => _iterationCount % 2 == 0 ? _buffer1 : _buffer2;
            private readonly CellState[,] _buffer1;
            private readonly CellState[,] _buffer2;
            private int _iterationCount = 0;
            private string _hashCache = null;
            private int _hashCacheIteration = -1;

            public Simulation(CellState[,] initialState)
            {
                _buffer1 = initialState;
                _buffer2 = new CellState[_buffer1.GetLength(0), _buffer1.GetLength(1)];
            }

            public void Step()
            {
                var front = _iterationCount % 2 == 0 ? _buffer1 : _buffer2;
                var back = _iterationCount % 2 != 0 ? _buffer1 : _buffer2;

                foreach (var (x, y) in front.Rectangle())
                {
                    var trees = 0;
                    var yards = 0;
                    foreach (var cell in front.GetNeighbours(x, y))
                    {
                        switch (cell)
                        {
                            case CellState.Tree:
                                trees++;
                                break;

                            case CellState.LumberYard:
                                yards++;
                                break;
                        }
                    }

                    switch (front[x, y])
                    {
                        case CellState.Clear:
                            if (trees >= 3)
                                back[x, y] = CellState.Tree;
                            else
                                back[x, y] = CellState.Clear;
                            break;

                        case CellState.Tree:
                            if (yards >= 3)
                                back[x, y] = CellState.LumberYard;
                            else
                                back[x, y] = CellState.Tree;
                            break;

                        case CellState.LumberYard:
                            if (trees >= 1 && yards >= 1)
                                back[x, y] = CellState.LumberYard;
                            else
                                back[x, y] = CellState.Clear;
                            break;
                    }
                }

                _iterationCount++;
            }

            public string HashState()
            {
                if (_iterationCount == _hashCacheIteration)
                    return _hashCache;

                var width = Environment.GetLength(0);
                var buffer = new byte[width * Environment.GetLength(1)];
                foreach (var (x, y) in Environment.Rectangle())
                {
                    switch (Environment[x, y])
                    {
                        case CellState.Clear:
                            buffer[(y * width) + x] = 1;
                            break;

                        case CellState.Tree:
                            buffer[(y * width) + x] = 7;
                            break;

                        case CellState.LumberYard:
                            buffer[(y * width) + x] = 13;
                            break;
                    }
                }

                using (var sha1 = SHA1.Create())
                {
                    _hashCache = sha1.ComputeHash(buffer).ToHexString();
                    _hashCacheIteration = _iterationCount;
                    return _hashCache;
                }
            }
        }

        CellState[,] LoadEnvironment(string inputFile)
        {
            var lines = FileIterator.LoadLines<string>(inputFile);
            var environment = new CellState[lines[0].Length, lines.Length];

            foreach (var (x, y) in environment.Rectangle())
            {
                switch (lines[y][x])
                {
                    case '.':
                        environment[x, y] = CellState.Clear;
                        break;

                    case '|':
                        environment[x, y] = CellState.Tree;
                        break;

                    case '#':
                        environment[x, y] = CellState.LumberYard;
                        break;

                    default:
                        Oh.Bugger();
                        break;
                }
            }

            return environment;
        }

        int CalculateLumberScore(CellState[,] environment)
        {
            var trees = 0;
            var yards = 0;

            foreach (var cell in environment)
            {
                if (cell == CellState.Tree)
                    trees++;
                else if (cell == CellState.LumberYard)
                    yards++;
            }

            return trees * yards;
        }

        [Theory]
        [InlineData(1147, 10, "Data/Day18-Test.txt")]
        [InlineData(495236, 10, "Data/Day18.txt")] // Solution
        void Problem1(int expectedAnswer, int iterations, string inputfile)
        {
            var environment = LoadEnvironment(inputfile);
            var simulation = new Simulation(environment);

            for (var i = 0; i < iterations; i++)
                simulation.Step();

            CalculateLumberScore(simulation.Environment).Should().Be(expectedAnswer);
        }

        [Fact]
        void Problem2()
        {
            var environment = LoadEnvironment("Data/Day18.txt");
            var simulation = new Simulation(environment);

            // Work out when we first repeat a simulated state
            var repeatCounts = new Dictionary<string, int>();
            var iteration = 0;
            while (!repeatCounts.ContainsKey(simulation.HashState()))
            {
                repeatCounts[simulation.HashState()] = iteration++;
                simulation.Step();
            }

            // Calculate the cycle period
            var firstSeen = repeatCounts[simulation.HashState()];
            var cyclePeriod = iteration - firstSeen;

            // Run the simulation a bit more until the remaining number of iterations
            // is evenly divisible by our cycle period
            var targetIteration = 1000000000;
            while (((targetIteration - iteration) % cyclePeriod) != 0)
            {
                simulation.Step();
                iteration++;
            }

            CalculateLumberScore(simulation.Environment).Should().Be(201348);
        }
    }
}
