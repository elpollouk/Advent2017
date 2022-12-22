using System;

namespace Utils
{
    public class XY
    {
        public int x;
        public int y;

        public XY() : this(0, 0) { }

        public XY(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator XY((int x, int y) value)
        {
            return new(value.x, value.y);
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

        public int ManhattenDistanceTo(XY other)
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
    }
}
