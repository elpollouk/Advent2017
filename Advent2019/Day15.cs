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
        class Map : Astar.IGraphAdapter<(int x, int y)>
        {
            public char[,] Grid { get; init; }

            public Map(char[,] map)
            {
                Grid = map;
            }

            public IEnumerable<(int x, int y)> GetLinked((int x, int y) node) => Grid.GetAdjecentPos(node.x, node.y)
                .Where(pos => Grid[pos.x, pos.y] == ' ');

            public int GetMoveCost((int x, int y) from, (int x, int y) to) => 1;

            public int GetScore((int x, int y) from, (int x, int y) to) => Math.Abs(to.x - from.x) + Math.Abs(to.y - from.y);

            public bool NodesEqual((int x, int y) a, (int x, int y) b) => a == b;
        }

        static Map LoadMap(out (int x, int y) start, out (int x, int y) exit)
        {
            var s = (x: -1, y: -1);
            var e = (x: -1, y: -1);

            var grid = FileIterator.LoadGrid<char>("Data/Day15.txt", (c, x, y) =>
            {
                if (c == 'D') s = (x, y);
                else if (c == 'X') e = (x, y);

                if (c != '#') c = ' ';

                return c;
            });

            start = s;
            exit = e;

            return new Map(grid);
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
            var map = LoadMap(out var start, out var exit);

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
            var map = LoadMap(out var _, out var exit);
            map.Grid[exit.x, exit.y] = 'O';
            var frontier = new List<(int x, int y)>(map.GetLinked(exit));
            var minutes = 0;

            do
            {
                minutes++;
                var newFrontier = new List<(int x, int y)>();

                foreach (var pos in frontier)
                {
                    map.Grid[pos.x, pos.y] = 'O';
                    newFrontier.AddRange(map.GetLinked(pos));
                }

                frontier = newFrontier;
            }
            while (frontier.Count != 0);

            minutes.Should().Be(310);
        }
    }
}
