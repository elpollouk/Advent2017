using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Utils
{
    public class Renderer
    {
        public static void RenderGrid<T>(string outputFile, T[,] grid, Func<T, Color> itemToPixelColour)
        {
            var bitmap = new Bitmap(grid.GetLength(0), grid.GetLength(1), PixelFormat.Format24bppRgb);
            foreach (var (x, y) in grid.Rectangle())
                bitmap.SetPixel(x, y, itemToPixelColour(grid[x, y]));

            bitmap.Save(outputFile, ImageFormat.Png);
        }
    }
}
