using System;
using Adevent2017.DataStructures;
using Adevent2017.Utils;
using Xunit;
using FluentAssertions;

namespace Adevent2017.Alogrithms
{
    public class AstarTests
    {
        class Position
        {
            public int x;
            public int y;
        }

        class TestGraphAdapter : StandardGraphAdapterBase<Position>
        {
            private readonly int _width;

            private readonly int _height;

            private readonly Position[] _cells;

            public TestGraphAdapter(Graph<Position> graph, int width, int height, Position[] cells) : base(graph)
            {
                _width = width;
                _height = height;
                _cells = cells;
            }

            public override int GetScore(Position from, Position to)
            {
                return Math.Abs(to.x - from.x) + Math.Abs(to.y - from.y);
            }

            public Position GetNode(int x, int y)
            {
                var index = (y * _width) + x;
                return _cells[index];
            }
        }

        TestGraphAdapter BuildGraph(int width, int height)
        {
            var cells = new Position[width * height];
            var graph = GraphBuilder.BuildGrid(width, height, (x, y) =>
            {
                var cell = new Position();
                cell.x = x;
                cell.y = y;
                cells[(y * width) + x] = cell;
                return cell;
            });

            return new TestGraphAdapter(graph, width, height, cells);
        }

        [Fact]
        public void DirectPath_Horizontal()
        {
            var adapter = BuildGraph(7, 7);

            var start = adapter.GetNode(1, 3);
            var goal = adapter.GetNode(5, 3);
            var path = Astar.FindPath(adapter, start, goal);

            path.Count.Should().Be(5);
            for (var i = 0; i < 5; i++)
            {
                path[i].x.Should().Be(i + 1);
                path[i].y.Should().Be(3);
            }
        }

        [Fact]
        public void DirectPath_Vertical()
        {
            var adapter = BuildGraph(7, 7);

            var start = adapter.GetNode(3, 5);
            var goal = adapter.GetNode(3, 1);
            var path = Astar.FindPath(adapter, start, goal);

            path.Count.Should().Be(5);
            for (var i = 0; i < 5; i++)
            {
                path[i].x.Should().Be(3);
                path[i].y.Should().Be(5 - i);
            }
        }

        [Fact]
        public void DirectPath_Diagonal()
        {
            var adapter = BuildGraph(4, 4);

            var start = adapter.GetNode(0, 0);
            var goal = adapter.GetNode(3, 3);
            var path = Astar.FindPath(adapter, start, goal);

            path.Count.Should().Be(7);
            for (var i = 0; i < 4; i++)
            {
                path[i].x.Should().Be(i);
                path[i].y.Should().Be(0);
            }
            for (var i = 1; i < 4; i++)
            {
                path[i + 3].x.Should().Be(3);
                path[i + 3].y.Should().Be(i);
            }
        }

        [Fact]
        public void ObstructedPath()
        {
            var adapter = BuildGraph(3, 2);
            var left = adapter.GetNode(1, 1);
            var right = adapter.GetNode(2, 1);
            adapter.Graph.RemoveTwoWayLink(left, right);

            var start = adapter.GetNode(0, 1);
            var goal = adapter.GetNode(2, 1);

            var path = Astar.FindPath(adapter, start, goal);

            path.Count.Should().Be(5);
            path[0].x.Should().Be(0);
            path[0].y.Should().Be(1);
            path[1].x.Should().Be(1);
            path[1].y.Should().Be(1);
            path[2].x.Should().Be(1);
            path[2].y.Should().Be(0);
            path[3].x.Should().Be(2);
            path[3].y.Should().Be(0);
            path[4].x.Should().Be(2);
            path[4].y.Should().Be(1);
        }
    }
}
