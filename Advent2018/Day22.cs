using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Utils.Alogrithms;
using Xunit;

namespace Advent2018
{
    public class Day22
    {
        [Theory]
        [InlineData(114, 510, 10, 10)]
        [InlineData(7402, 11817, 9, 751)]
        void Problem1(int expectedRisk, ulong depth, int targetX, int targetY)
        {
            var cave = new (ulong el, int type)[targetX + 1, targetY + 1];
            var risk = 0L;
            foreach (var (x, y) in cave.Rectangle())
            {
                ulong gi;
                if (x == 0 && y == 0)
                    gi = 0;
                else if (x == targetX && y == targetY)
                    gi = 0;
                else if (y == 0)
                    gi = (ulong)x * 16807;
                else if (x == 0)
                    gi = (ulong)y * 48271;
                else
                    gi = cave[x - 1, y].el * cave[x, y - 1].el;

                var el = (gi + depth) % 20183;
                var type = (int)(el % 3);
                cave[x, y] = (el, type);
                risk += type;
            }

            risk.Should().Be(expectedRisk);
        }

        enum AreaType
        {
            Rocky = 0,
            Wet = 1,
            Narrow = 2

        }

        enum Item
        {
            Nothing,
            Torch,
            Rope,
        }

        class CaveAdapter : Astar.IGraphAdapter<(int x, int y, Item item)>
        {
            private readonly (ulong el, AreaType type)[,] _cave;

            public CaveAdapter((ulong el, AreaType type)[,] cave)
            {
                _cave = cave;
            }

            private Item GetAlternativeItem((int x, int y, Item item) node)
            {
                switch (_cave[node.x, node.y].type)
                {
                    case AreaType.Rocky:
                        if (node.item == Item.Torch) return Item.Rope;
                        return Item.Torch;

                    case AreaType.Wet:
                        if (node.item == Item.Nothing) return Item.Rope;
                        return Item.Nothing;

                    case AreaType.Narrow:
                        if (node.item == Item.Nothing) return Item.Torch;
                        return Item.Nothing;
                }

                throw new Exception("Invalid area type");
            }

            private bool IsItemValidFor(int x, int y, Item item)
            {
                switch (_cave[x, y].type)
                {
                    case AreaType.Rocky:
                        return item == Item.Torch || item == Item.Rope;

                    case AreaType.Wet:
                        return item == Item.Rope || item == Item.Nothing;

                    case AreaType.Narrow:
                        return item == Item.Torch || item == Item.Nothing;
                }

                throw new Exception("Invalid area type");
            }

            public IEnumerable<(int x, int y, Item item)> GetLinked((int x, int y, Item item) node)
            {
                yield return (node.x, node.y, GetAlternativeItem(node));

                if (node.y > 0 && IsItemValidFor(node.x, node.y - 1, node.item))
                    yield return (node.x, node.y - 1, node.item);

                if (node.x > 0 && IsItemValidFor(node.x - 1, node.y, node.item))
                    yield return (node.x - 1, node.y, node.item);

                if (IsItemValidFor(node.x + 1, node.y, node.item))
                    yield return (node.x + 1, node.y, node.item);

                if (IsItemValidFor(node.x, node.y + 1, node.item))
                    yield return (node.x, node.y + 1, node.item);
            }

            public int GetMoveCost((int x, int y, Item item) from, (int x, int y, Item item) to)
            {
                if (from.x != to.x || from.y != to.y)
                    return 1;
                return 7;
            }

            public int GetScore((int x, int y, Item item) from, (int x, int y, Item item) to)
            {
                var distance = Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
                return from.item == to.item ? distance : distance + 1;
            }
        }

        Item GetInitialItem(AreaType type)
        {
            switch (type)
            {
                case AreaType.Rocky:
                case AreaType.Narrow:
                    return Item.Torch;

                case AreaType.Wet:
                    return Item.Rope;

                default:
                    throw new Exception("Invalid area type");
            }
        }

        [Theory]
        [InlineData(45, 510, 10, 10)]
        [InlineData(1025, 11817, 9, 751)]
        void Problem2(int expectedMinutes, ulong depth, int targetX, int targetY)
        {
            var cave = new (ulong el, AreaType type)[136, 756];
            foreach (var (x, y) in cave.Rectangle())
            {
                ulong gi;
                if (x == 0 && y == 0)
                    gi = 0;
                else if (x == targetX && y == targetY)
                    gi = 0;
                else if (y == 0)
                    gi = (ulong)x * 16807;
                else if (x == 0)
                    gi = (ulong)y * 48271;
                else
                    gi = cave[x - 1, y].el * cave[x, y - 1].el;

                var el = (gi + depth) % 20183;
                var type = (AreaType)(el % 3);
                cave[x, y] = (el, type);
            }

            var adapter = new CaveAdapter(cave);
            var path = Astar.FindPath(adapter,
                                      (0, 0, GetInitialItem(cave[0, 0].type)),
                                      (targetX, targetY, GetInitialItem(cave[targetX, targetY].type)));

            var minutes = 0;
            var previous = path[0];
            for (var i = 1; i < path.Count; i++)
            {
                var current = path[i];
                if (current.Item1 != previous.Item1 || current.Item2 != previous.Item2)
                    minutes++;
                else
                    minutes += 7;

                previous = current;
            }

            minutes.Should().Be(expectedMinutes);
        }
    }
}
