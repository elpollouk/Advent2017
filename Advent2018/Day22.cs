using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        struct CaveNode
        {
            public readonly (int x, int y) Pos;
            public readonly Item Item;

            public CaveNode((int x, int y) pos, Item item)
            {
                Pos = pos;
                Item = item;
            }

            public override string ToString() => $"({Pos.x}, {Pos.y}), {Item}";
        }

        class CaveAdapter : Astar.IGraphAdapter<CaveNode>
        {
            private readonly (ulong el, AreaType type)[,] _cave;

            public CaveAdapter((ulong el, AreaType type)[,] cave)
            {
                _cave = cave;
            }

            private Item GetAlternativeItem(CaveNode node)
            {
                switch (_cave[node.Pos.x, node.Pos.y].type)
                {
                    case AreaType.Rocky:
                        if (node.Item == Item.Torch) return Item.Rope;
                        return Item.Torch;

                    case AreaType.Wet:
                        if (node.Item == Item.Nothing) return Item.Rope;
                        return Item.Nothing;

                    case AreaType.Narrow:
                        if (node.Item == Item.Nothing) return Item.Torch;
                        return Item.Nothing;
                }

                throw new Exception("Invalid area type");
            }

            private bool IsItemValidFor((int x, int y) pos, Item item)
            {
                switch (_cave[pos.x, pos.y].type)
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

            public IEnumerable<CaveNode> GetLinked(CaveNode node)
            {
                yield return new CaveNode(node.Pos, GetAlternativeItem(node));

                if (node.Pos.y > 0 && IsItemValidFor((node.Pos.x, node.Pos.y - 1), node.Item))
                    yield return new CaveNode((node.Pos.x, node.Pos.y - 1), node.Item);

                if (node.Pos.x > 0 && IsItemValidFor((node.Pos.x - 1, node.Pos.y), node.Item))
                    yield return new CaveNode((node.Pos.x - 1, node.Pos.y), node.Item);

                if (IsItemValidFor((node.Pos.x + 1, node.Pos.y), node.Item))
                    yield return new CaveNode((node.Pos.x + 1, node.Pos.y), node.Item);

                if (IsItemValidFor((node.Pos.x, node.Pos.y + 1), node.Item))
                    yield return new CaveNode((node.Pos.x, node.Pos.y + 1), node.Item);
            }

            public int GetMoveCost(CaveNode from, CaveNode to)
            {
                if (from.Pos.x != to.Pos.x || from.Pos.y != to.Pos.y)
                    return 1;
                return 7;
            }

            public int GetScore(CaveNode from, CaveNode to)
            {
                return Math.Abs(from.Pos.x - to.Pos.x) + Math.Abs(from.Pos.y - to.Pos.y);
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
            var cave = new (ulong el, AreaType type)[2000, 2000];
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
                                      new CaveNode((0, 0), GetInitialItem(cave[0, 0].type)),
                                      new CaveNode((targetX, targetY), GetInitialItem(cave[targetX, targetY].type)));

            var minutes = 0;
            var previous = path[0];
            for (var i = 1; i < path.Count; i++)
            {
                var current = path[i];
                if (current.Pos.x != previous.Pos.x || current.Pos.y != previous.Pos.y)
                    minutes++;
                else
                    minutes += 7;

                previous = current;
            }

            minutes.Should().Be(expectedMinutes);
        }
    }
}
