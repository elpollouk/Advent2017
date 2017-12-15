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
            public readonly Graph<Position> Graph;

            private readonly int _width;

            private readonly int _height;

            public TestGraphAdapter(Graph<Position> graph, int width, int height) : base(graph)
            {
                Graph = graph;
                _width = width;
                _height = height;
            }

            public override int GetScore(GraphNode<Position> from, GraphNode<Position> to)
            {
                return Math.Abs(to.Item.x - from.Item.x) + Math.Abs(to.Item.y - from.Item.y);
            }

            public GraphNode<Position> GetNode(int x, int y)
            {
                var id = (y * _width) + x;
                return Graph.GetNode(id);
            }
        }

        TestGraphAdapter BuildGraph(int width, int height)
        {
            var graph = GraphBuilder.BuildGrid<Position>(width, height, (x, y, node) =>
            {
                node.Item = new Position();
                node.Item.x = x;
                node.Item.y = y;
            });

            return new TestGraphAdapter(graph, width, height);
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
                path[i].Item.x.Should().Be(i + 1);
                path[i].Item.y.Should().Be(3);
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
                path[i].Item.x.Should().Be(3);
                path[i].Item.y.Should().Be(5 - i);
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
                path[i].Item.x.Should().Be(i);
                path[i].Item.y.Should().Be(0);
            }
            for (var i = 1; i < 4; i++)
            {
                path[i + 3].Item.x.Should().Be(3);
                path[i + 3].Item.y.Should().Be(i);
            }
        }

        [Fact]
        public void ObstructedPath()
        {
            var adapter = BuildGraph(3, 2);
            var leftId = adapter.GetNode(1, 1).Id;
            var rightId = adapter.GetNode(2, 1).Id;
            adapter.Graph.RemoveTwoWayLink(leftId, rightId);

            var start = adapter.GetNode(0, 1);
            var goal = adapter.GetNode(2, 1);

            var path = Astar.FindPath(adapter, start, goal);

            path.Count.Should().Be(5);
            path[0].Item.x.Should().Be(0);
            path[0].Item.y.Should().Be(1);
            path[1].Item.x.Should().Be(1);
            path[1].Item.y.Should().Be(1);
            path[2].Item.x.Should().Be(1);
            path[2].Item.y.Should().Be(0);
            path[3].Item.x.Should().Be(2);
            path[3].Item.y.Should().Be(0);
            path[4].Item.x.Should().Be(2);
            path[4].Item.y.Should().Be(1);
        }
    }
}
