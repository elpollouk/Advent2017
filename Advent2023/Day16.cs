using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day16
    {
        const int ActivationNone = 0;
        const int ActivationHorizontal = 1;
        const int ActivationVertical = 2;

        class Beam(int x, int y, int dX, int dY)
        {
            public XY pos = new(x, y);
            public XY delta = new(dX, dY);

            public void Update()
            {
                pos.Add(delta);
            }

            public Beam Split()
            {
                delta.RotateLeft();
                return new(pos.x, pos.y, -delta.x, -delta.y);
            }

            public void RotateLeft() => delta.RotateLeft();
            
            public void RotateRight() => delta.RotateRight();

            public int ActivationPlane => delta.x == 0 ? ActivationVertical : ActivationHorizontal;
        }

        long Simulate(char[,] grid, Beam initialBeam)
        {
            Dictionary<(int x, int y), int> activations = [];
            Queue<Beam> beams = [];
            beams.Enqueue(initialBeam);

            void Split(Beam beam)
            {
                var newBeam = beam.Split();
                newBeam.Update();
                beams.Enqueue(newBeam);
            }

            while (beams.Count > 0)
            {
                var beam = beams.Dequeue();

                if (!grid.IsInBounds(beam.pos))
                {
                    // Beam has left the grid so it can be discarded
                    continue;
                }

                var c = grid.GetAt(beam.pos);
                var state = activations.GetOrDefault(beam.pos.ToTuple(), ActivationNone);

                switch (c)
                {
                    case '.':
                        if ((state & beam.ActivationPlane) != 0)
                        {
                            // This cell has already been traversed so this beam can also be discarded
                            continue;
                        }
                        break;

                    case '-':
                        if (beam.ActivationPlane == ActivationVertical)
                        {
                            Split(beam);
                        }
                        break;

                    case '|':
                        if (beam.ActivationPlane == ActivationHorizontal)
                        {
                            Split(beam);
                        }
                        break;

                    case '\\':
                        if (beam.ActivationPlane == ActivationHorizontal)
                        {
                            beam.RotateRight();
                        }
                        else
                        {
                            beam.RotateLeft();
                        }
                        break;

                    case '/':
                        if (beam.ActivationPlane == ActivationHorizontal)
                        {
                            beam.RotateLeft();
                        }
                        else
                        {
                            beam.RotateRight();
                        }
                        break;
                }

                activations[beam.pos.ToTuple()] = state | beam.ActivationPlane;
                beam.Update();
                beams.Enqueue(beam);
            }

            return activations.Count;
        }

        [Theory]
        [InlineData("Data/Day16_Test.txt", 46)]
        [InlineData("Data/Day16.txt", 7482)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            var energised = Simulate(grid, new(0, 0, 1, 0));
            energised.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day16_Test.txt", 51)]
        [InlineData("Data/Day16.txt", 7896)]
        public void Part2(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            
            IEnumerable<Beam> Beams()
            {
                for (var x = 0; x < grid.GetLength(0); x++)
                {
                    yield return new(x, 0, 0, 1);
                    yield return new(x, grid.GetLength(0) - 1, 0, -1);
                }

                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    yield return new(0, y, 1, 0);
                    yield return new(grid.GetLength(1) - 1, y, -1, 0);
                }
            }

            Beams().AsParallel()
                .Select(b => Simulate(grid, b))
                .Max()
                .Should().Be(expectedAnswer);
        }
    }
}
