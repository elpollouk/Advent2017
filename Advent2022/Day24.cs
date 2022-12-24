using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2022
{
    public class Day24
    {
        class BlizzardAdapter : Astar.IGraphAdapter<(int x, int y, int state)>
        {
            public readonly HashSet<(int x, int y)>[] AllStates;
            private readonly int MaxX;
            private readonly int MaxY;
            public XY Exit;

            public BlizzardAdapter(HashSet<(int x, int y)>[] allStates, int maxX, int maxY)
            {
                AllStates = allStates;
                MaxX = maxX;
                MaxY = maxY;
            }

            IEnumerable<(int x, int y, int state)> Astar.IGraphAdapter<(int x, int y, int state)>.GetLinked((int x, int y, int state) node)
            {
                int nextState = node.state + 1;
                if (nextState == AllStates.Length) nextState = 0;
                var state = AllStates[nextState];

                // Wait
                if (!state.Contains((node.x, node.y)))
                    yield return (node.x, node.y, nextState);

                // Down
                (int x, int y) check = (node.x, node.y + 1);
                if (Exit == check)
                {
                    yield return (check.x, check.y, -1);
                    yield break;
                }
                if (node.y < MaxY && !state.Contains(check))
                    yield return (check.x, check.y, nextState);

                if (node.y == 0) // Down is the only valid move from the start
                    yield break;

                // Up
                check = (node.x, node.y - 1);
                if (Exit == check)
                {
                    yield return (check.x, check.y, -1);
                    yield break;
                }

                if (node.y > 1 && !state.Contains(check))
                    yield return (check.x, check.y, nextState);

                if (node.y == MaxY + 1) // Up is the only valid move from the other start;
                    yield break;

                // Left
                if (node.x < MaxX)
                {
                    check = (node.x + 1, node.y);
                    if (!state.Contains(check))
                        yield return (check.x, check.y, nextState);
                }

                // Right
                if (node.x > 1)
                {
                    check = (node.x - 1, node.y);
                    if (!state.Contains(check))
                        yield return (check.x, check.y, nextState);
                }
            }

            int Astar.IGraphAdapter<(int x, int y, int state)>.GetMoveCost((int x, int y, int state) from, (int x, int y, int state) to)
            {
                return 1;
            }

            int Astar.IGraphAdapter<(int x, int y, int state)>.GetScore((int x, int y, int state) from, (int x, int y, int state) to)
            {
                return Math.Abs(to.x - from.x) + Math.Abs(to.y - from.y);
            }
        }

        static XY Up(XY pos) => pos.Clone().Add(0, -1);
        static XY Down(XY pos) => pos.Clone().Add(0, 1);
        static XY Left(XY pos) => pos.Clone().Add(-1, 0);
        static XY Right(XY pos) => pos.Clone().Add(1, 0);

        (XY pos, Func<XY, XY> direction)[] Parse(string filename, out XY exit)
        {
            exit = new();
            List<(XY, Func<XY, XY>)> list = new();

            var y = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                for (int x = 0; x < line.Length; x++)
                {
                    switch (line[x])
                    {
                        case '^':
                            list.Add(((x, y), Up));
                            break;

                        case 'v':
                            list.Add(((x, y), Down));
                            break;

                        case '<':
                            list.Add(((x, y), Left));
                            break;

                        case '>':
                            list.Add(((x, y), Right));
                            break;

                        case '.':
                            exit.Set(x, y);
                            break;
                    }
                }
                y++;
            }

            return list.ToArray();
        }

        (XY pos, Func<XY, XY> direction)[] Step((XY pos, Func<XY, XY> direction)[] blizzards, XY exit)
        {
            var newList = new (XY pos, Func<XY, XY> direction)[blizzards.Length];

            for (int i = 0; i < blizzards.Length; i++)
            {
                var direction = blizzards[i].direction;
                var newPos = direction(blizzards[i].pos);
                if (newPos.x == 0) newPos.x = exit.x;
                else if (newPos.x == exit.x + 1) newPos.x = 1;
                else if (newPos.y == 0) newPos.y = exit.y - 1;
                else if (newPos.y == exit.y) newPos.y = 1;

                newList[i] = (newPos, direction);
            }

            return newList;
        }

        HashSet<(int x, int y)> BuildMap((XY pos, Func<XY, XY> direction)[] blizzards)
        {
            HashSet<(int, int)> set = new();

            foreach (var blizzard in blizzards)
            {
                set.Add(blizzard.pos.ToTuple());
            }

            return set;
        }

        (BlizzardAdapter, XY) Load(string filename)
        {
            var blizzards = Parse(filename, out var exit);
            var width = exit.x;
            var height = exit.y - 1;

            var allStates = new HashSet<(int x, int y)>[width * height];

            for (int i = 0; i < width * height; i++)
            {
                allStates[i] = BuildMap(blizzards);
                blizzards = Step(blizzards, exit);
            }

            BlizzardAdapter adapter = new(allStates, exit.x, exit.y - 1);

            return (adapter, exit);
        }

        [Theory]
        [InlineData("Data/Day24_Test.txt", 18)]
        [InlineData("Data/Day24.txt", 295)]
        public void Part1(string filename, int expectedAnswer)
        {
            var (adapter, exit) = Load(filename);
            adapter.Exit = exit;
            var path = Astar.FindPath(adapter, (1, 0, 0), (exit.x, exit.y, -1));
            (path.Count - 1).Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day24_Test.txt", 54)]
        [InlineData("Data/Day24.txt", 851)]
        public void Part2(string filename, int expectedAnswer)
        {
            var (adapter, exit) = Load(filename);

            adapter.Exit = exit;
            var path = Astar.FindPath(adapter, (1, 0, 0), (exit.x, exit.y, -1));
            var minutes = path.Count - 1;

            adapter.Exit = (1, 0);
            path = Astar.FindPath(adapter, (exit.x, exit.y, minutes % adapter.AllStates.Length), (1, 0, -1));
            minutes += path.Count - 1;

            adapter.Exit = exit;
            path = Astar.FindPath(adapter, (1, 0, minutes % adapter.AllStates.Length), (exit.x, exit.y, -1));
            minutes += path.Count - 1;

            minutes.Should().Be(expectedAnswer);
        }
    }
}
