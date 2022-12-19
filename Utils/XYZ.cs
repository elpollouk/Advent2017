using System.Collections.Generic;

namespace Utils
{
    public class XYZ
    {
        public int x;
        public int y;
        public int z;

        public XYZ() : this(0, 0, 0) { }

        public XYZ(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator XYZ((int x, int y, int z) value)
        {
            return new(value.x, value.y, value.z);
        }

        public XYZ Clone()
        {
            return new(x, y, z);
        }

        public IEnumerable<XYZ> GetAdjacent()
        {
            yield return (x - 1, y, z);
            yield return (x + 1, y, z);
            yield return (x, y - 1, z);
            yield return (x, y + 1, z);
            yield return (x, y, z - 1);
            yield return (x, y, z + 1);
        }

        public (int x, int y, int z) ToTuple()
        {
            return (x, y, z);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}
