using Adevent2017.Utils;
using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace Adevent2017
{
    public class Problem1101
    {
        int maxX = 0;
        int maxY = 0;

        int WalkBack(int x, int y)
        {
            x = Math.Abs(x);
            var halfX = x / 2;

            if ((y < 0) && ((x & 1) == 1))
                y++;

            y = Math.Abs(y);
            if (halfX < y)
                y -= halfX;
            else
                y = 0;

            return x + y;
        }

        int Solve(string[] steps)
        {
            var x = 0;
            var y = 0;

            // Follow path
            for (var i = 0; i < steps.Length; i++)
            {
                switch (steps[i])
                {
                    case "nw":
                        if ((x & 1) == 0) y--;
                        x--;
                        break;

                    case "n":
                        y--;
                        break;

                    case "ne":
                        if ((x & 1) == 0) y--;
                        x++;
                        break;

                    case "sw":
                        if ((x & 1) == 1) y++;
                        x--;
                        break;

                    case "s":
                        y++;
                        break;

                    case "se":
                        if ((x & 1) == 1) y++;
                        x++;
                        break;

                    default:
                        Oh.PissingNora();
                        break;
                }

                if (Math.Abs(maxX) < Math.Abs(x)) maxX = x;
                if (Math.Abs(maxY) < Math.Abs(y)) maxY = y;
            }

            return WalkBack(x, y);
        }

        [Theory]
        [InlineData("ne,ne,ne", 3, 3)]
        [InlineData("ne,ne,ne,n", 4, 4)]
        [InlineData("ne,ne,ne,ne", 4, 4)]
        [InlineData("ne,ne,ne,s", 3, 3)]
        [InlineData("ne,ne,sw,sw", 0, 2)]
        [InlineData("ne,ne,s,s", 2, 2)]
        [InlineData("se,sw,se,sw,sw", 3, 3)]
        [InlineData("sw,sw,sw,sw,sw,s,s", 7, 7)]
        [InlineData("sw,sw,sw,sw,sw,sw,s", 7, 7)]
        [InlineData("s,s,s,n,n,n,n,n,n,n,s", 3, 4)]
        public void Examples(string input, int answer, int maxDistance)
        {
            var steps = input.Split(',');
            Solve(steps).Should().Be(answer);
            WalkBack(maxX, maxY).Should().Be(maxDistance);
        }

        [Fact]
        public void Solution()
        {
            var steps = File.ReadAllText("Data/1101.txt").Split(',');
            Solve(steps).Should().Be(812);
            WalkBack(maxX, maxY).Should().Be(1603);
        }
    }
}
