using FluentAssertions;
using System;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2016
{
    public class Day08
    {
        enum Operation
        {
            RECT,
            ROTATE_ROW,
            ROTATE_COLUMN
        }

        record Command(Operation Op, int A, int B);

        class Display
        {
            readonly bool[,] pixels;

            public Display(int width, int height)
            {
                pixels = new bool[width, height];
            }

            public int GetNumActivePixels()
            {
                int count = 0;
                foreach (var lit in pixels.Items())
                    if (lit)
                        count++;
                return count;
            }

            public void Apply(Command command)
            {
                switch (command.Op)
                {
                    case Operation.RECT:
                        Rect(command.A, command.B);
                        return;

                    case Operation.ROTATE_ROW:
                        RotateRow(command.A, command.B);
                        return;

                    case Operation.ROTATE_COLUMN:
                        RotateColumn(command.A, command.B);
                        return;
                }

                throw new Exception();
            }

            private void Rect(int width, int height)
            {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        pixels[x, y] = true;
            }

            private void RotateRow(int row, int count)
            {
                bool[] output = new bool[pixels.GetLength(0)];
                for (int x = 0; x < output.Length; x++)
                    output.SetAtMod(x + count, pixels[x, row]);

                for (int x = 0; x < output.Length; x++)
                    pixels[x, row] = output[x];
            }

            private void RotateColumn(int column, int count)
            {
                bool[] output = new bool[pixels.GetLength(1)];
                for (int y = 0; y < output.Length; y++)
                    output.SetAtMod(y + count, pixels[column, y]);

                for (int y = 0; y < output.Length; y++)
                    pixels[column, y] = output[y];
            }

            public void Render(Action<string> output) => pixels.DebugDump(output, (b) => b ? '█' : ' ');
        }

        private readonly ITestOutputHelper output;

        public Day08(ITestOutputHelper output)
        {
            this.output = output;
        }

        Command ParseLine(string line)
        {
            if (line.StartsWith("rect "))
            {
                var groups = line.Groups("(\\d+)x(\\d+)");
                return new Command(Operation.RECT, int.Parse(groups[0]), int.Parse(groups[1]));
            }
            else if (line.StartsWith("rotate row y="))
            {
                var groups = line.Groups("(\\d+) by (\\d+)");
                return new Command(Operation.ROTATE_ROW, int.Parse(groups[0]), int.Parse(groups[1]));
            }
            else if (line.StartsWith("rotate column x="))
            {
                var groups = line.Groups("(\\d+) by (\\d+)");
                return new Command(Operation.ROTATE_COLUMN, int.Parse(groups[0]), int.Parse(groups[1]));
            }
            throw new Exception(line);
        }
        
        [Theory]
        [InlineData("Data/Day08_Test.txt", 7, 3, 6)]
        [InlineData("Data/Day08.txt", 50, 6, 123)]
        public void Part1(string filename, int width, int height, int expectedAnswer)
        {
            Display display = new(width, height);

            foreach (string line in FileIterator.Lines(filename))
            {
                var command = ParseLine(line);
                display.Apply(command);
            }

            display.Render(output.WriteLine);
            display.GetNumActivePixels().Should().Be(expectedAnswer);
        }
    }
}
