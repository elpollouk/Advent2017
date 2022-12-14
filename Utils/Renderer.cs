using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Utils
{
    public static class Renderer
    {
        public static readonly Bgr24 Black = Colour(0, 0, 0);
        public static readonly Bgr24 White = Colour(255, 255, 255);
        public static readonly Bgr24 Yellow = Colour(255, 217, 25);

        public static Bgr24 Colour(byte r, byte g, byte b) => new(r, g, b);

        public static void RenderGrid<T>(string outputFile, T[,] grid, Func<T, Bgr24> itemToPixelColour)
        {
            var bitmap = new Image<Bgr24>(grid.GetLength(0), grid.GetLength(1));
            foreach (var (x, y) in grid.Rectangle())
                bitmap[x, y] = itemToPixelColour(grid[x, y]);

            bitmap.Save(outputFile, new PngEncoder());
        }

        public static void DrawLine<T>(this T[,] grid, XY from, XY to, T colour)
        {
            if (from.x == to.x)
            {
                if (to.y < from.y) (from, to) = (to, from);
                for (int y = from.y; y <= to.y; y++)
                    grid[from.x, y] = colour;
            }
            else if (from.y == to.y)
            {
                if (to.x < from.x) (from, to) = (to, from);
                for (int x = from.x; x <= to.x; x++)
                    grid[x, from.y] = colour;
            }
            else
            {
                throw new Exception("Unsupported line orientation");
            }
        }

        public static void DrawPath<T>(this T[,] grid, XY[] path, T colour)
        {
            for (int i = 1; i < path.Length; i++)
                DrawLine(grid, path[i - 1], path[i], colour);
        }

        public static void Plot<T>(this T[,] grid, XY pos, T colour)
        {
            grid[pos.x, pos.y] = colour;
        }
    }
}
