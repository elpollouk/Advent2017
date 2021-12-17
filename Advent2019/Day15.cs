using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2019
{
    public class Day15
    {
        class Map :Astar.IGraphAdapter<(int x, int y)>
        {
            private readonly char[,] _map;

            public Map(char[,] map)
            {
                _map = map;
            }

            public IEnumerable<(int x, int y)> GetLinked((int x, int y) node)
            {
                foreach (var pos in _map.GetAdjecentPos(node.x, node.y))
                {
                    if (_map[pos.x, pos.y] != '#')
                    {
                        yield return pos;
                    }
                }
            }

            public int GetMoveCost((int x, int y) from, (int x, int y) to)
            {
                return 1;
            }

            public int GetScore((int x, int y) from, (int x, int y) to)
            {
                return Math.Abs(to.x - from.x) + Math.Abs(to.y - from.y);
            }
        }

        static int MoveToAction((int x, int y) from, (int x, int y) to)
        {
            if (to == (from.x, from.y - 1)) return 1;
            if (to == (from.x, from.y + 1)) return 2;
            if (to == (from.x - 1, from.y)) return 3;
            if (to == (from.x + 1, from.y)) return 4;
            throw new InvalidOperationException();
        }

        [Fact]
        public void Part1()
        {
            var start = (x: -1, y: -1);
            var exit = (x: -1, y: -1);

            var grid = FileIterator.LoadGrid<char>("Data/Day15.txt", (c, x, y) =>
            {
                if (c == 'D') start = (x, y);
                else if (c == 'X') exit = (x, y);
                
                if (c != '#')  c = ' ';

                return c;
            });

            var map = new Map(grid);

            var path = Astar.FindPath(map, start, exit).ToArray();
            path.Length.Should().Be(305);
            var actions = "";
            for (var i = 1; i < path.Length; i++)
            {
                if (i != 1) actions += ',';
                actions += MoveToAction(path[i - 1], path[i]);
            }
            Debug.WriteLine(actions);
        }

        [Fact]
        public void Part2()
        {

        }
    }
}
