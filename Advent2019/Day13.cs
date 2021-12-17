using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    public class Day13
    {
        const long SCORE = -1;
        const long BLOCK = 2;
        const long PADDLE = 3;
        const long BALL = 4;

        class GameState
        {
            public readonly Dictionary<(long, long), long> View = new();
            public long NumBlocks => View.Values.Where(t => t == BLOCK).Count();

            public long Score = 0;
            public (long x, long y) Ball = (-1, -1);
            public (long x, long y) Paddle = (-1, -1);
        }

        static void Render(GameState game, IntCode.VmState state)
        {
            while (state.OutputQueue.Count != 0)
            {
                var x = state.OutputQueue.Dequeue();
                var y = state.OutputQueue.Dequeue();
                var tile = state.OutputQueue.Dequeue();

                if (x == SCORE)
                {
                    game.Score = tile;
                }
                else
                {
                    game.View[(x, y)] = tile;
                    if (tile == BALL)
                    {
                        game.Ball = (x, y);
                    }
                    else if (tile == PADDLE)
                    {
                        game.Paddle = (x, y);
                    }
                }
            }
        }

        [Theory]
        [InlineData("Data/Day13.txt", 348)]
        public void Part1(string filename, int expectedAnswer)
        {
            var prog = FileIterator.LoadCSV<int>(filename);
            var vm = IntCode.CreateVM(prog);
            vm.Execute();
            var game = new GameState();
            Render(game, vm.State);
            game.NumBlocks.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day13.txt", 16999)]
        public void Part2(string filename, long expectedAnswer)
        {
            var prog = FileIterator.LoadCSV<int>(filename);
            prog[0] = 2;
            var vm = IntCode.CreateVM(prog);
            var game = new GameState();

            vm.State.Input = () =>
            {
                Render(game, vm.State);
                if (game.Ball.x < game.Paddle.x) return -1;
                if (game.Ball.x > game.Paddle.x) return 1;
                return 0;
            };

            vm.Execute();
            Render(game, vm.State);
            game.Score.Should().Be(expectedAnswer);
        }
    }
}
