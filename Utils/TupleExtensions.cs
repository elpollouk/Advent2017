namespace Utils
{
    public static class TupleExtensions
    {
        public static bool WithinRect(this (int x, int y) coord, int lx, int ly)
        {
            return (0 <= coord.x) && (0 <= coord.y) && (coord.x < lx) && (coord.y < ly);
        }

        public static bool WithinRect(this (int x, int y) coord, (int x, int y) shape)
        {
            return WithinRect(coord, shape.x, shape.y);
        }
    }
}
