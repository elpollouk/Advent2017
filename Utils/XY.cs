using System;

namespace Utils
{
    public class XY(int x, int y)
    {
        public int x = x;
        public int y = y;

        public XY() : this(0, 0) { }

        public static implicit operator XY((int x, int y) value)
        {
            return new(value.x, value.y);
        }

        public static bool operator ==(XY a, (int x, int y) b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(XY a, (int x, int y) b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public XY Set(XY other)
        {
            x = other.x;
            y = other.y;
            return this;
        }

        public XY Set((int x, int y) other)
        {
            x = other.x;
            y = other.y;
            return this;
        }

        public XY Set(int x, int y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public XY Clone()
        {
            return new(x, y);
        }

        public XY Add(int x, int y)
        {
            this.x += x;
            this.y += y;
            return this;
        }

        public XY Add(XY other)
        {
            x += other.x;
            y += other.y;
            return this;
        }

        public XY Sub(XY other)
        {
            x -= other.x;
            y -= other.y;
            return this;
        }

        public int ManhattanDistanceTo(XY other)
        {
            var dx = other.x - x;
            var dy = other.y - y;
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public XY RotateRight()
        {
            (x, y) = (-y, x);
            return this;
        }
        public XY RotateLeft()
        {
            (x, y) = (y, -x);
            return this;
        }

        public (int x, int y) ToTuple() {
            return (x, y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return x == ((XY)obj).x && y == ((XY)obj).y;
        }

        public override int GetHashCode()
        {
            return (x, y).GetHashCode();
        }
    }
}
