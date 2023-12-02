using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day02
    {
        record Draw(int Red, int Green, int Blue);

        Draw ParseDraw(string text)
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            string[] parts = text.Split(',');
            foreach (string part in parts)
            {
                string[] details = part.Trim().Split(' ');
                switch (details[1])
                {
                    case "red":
                        red = int.Parse(details[0]);
                        break;

                    case "green":
                        green = int.Parse(details[0]);
                        break;

                    case "blue":
                        blue = int.Parse(details[0]);
                        break;
                }
            }

            return new Draw(red, green, blue);
        }

        int IsPossible(string line, int red, int green, int blue)
        {
            var id = line.Match("^Game (\\d+):").Groups[1].Value;
            var draws = line.Split(':')[1].Trim().Split(';');

            foreach (var d in draws)
            {
                var draw = ParseDraw(d);
                if (red < draw.Red) return 0;
                if (green < draw.Green) return 0;
                if (blue < draw.Blue) return 0;
            }

            return int.Parse(id);
        }

        long Power(string line)
        {
            int maxRed = 0;
            int maxGreen = 0;
            int maxBlue = 0;
            var draws = line.Split(':')[1].Trim().Split(';');

            foreach (var d in draws)
            {
                var draw = ParseDraw(d);
                if (maxRed < draw.Red) maxRed = draw.Red;
                if (maxGreen < draw.Green) maxGreen = draw.Green;
                if (maxBlue < draw.Blue) maxBlue = draw.Blue;
            }

            return maxRed * maxGreen * maxBlue;
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", 8)]
        [InlineData("Data/Day02.txt", 2416)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                total += IsPossible(line, 12, 13, 14);
            }
            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day02_Test.txt", 2286)]
        [InlineData("Data/Day02.txt", 63307)]
        public void Part2(string filename, long expectedAnswer)
        {
            long total = 0;
            foreach (var line in FileIterator.Lines(filename))
            {
                total += Power(line);
            }
            total.Should().Be(expectedAnswer);
        }
    }
}
