using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Utils
{
    public class Renderer
    {
        public static Bgr24 Colour(byte r, byte g, byte b) => new Bgr24(r, g, b);

        public static void RenderGrid<T>(string outputFile, T[,] grid, Func<T, Bgr24> itemToPixelColour)
        {
            var bitmap = new Image<Bgr24>(grid.GetLength(0), grid.GetLength(1));
            foreach (var (x, y) in grid.Rectangle())
                bitmap[x, y] = itemToPixelColour(grid[x, y]);

            bitmap.Save(outputFile, new PngEncoder());
        }
    }
}
