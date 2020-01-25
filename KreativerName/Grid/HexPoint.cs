using System;
using OpenTK;

namespace KreativerName.Grid
{
    public struct HexPoint : IBytes
    {
        public HexPoint(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public HexPoint(Vector2 v)
        {
            x = (int)v.X;
            y = (int)v.Y;
        }

        int x, y;

        public int X
        {
            get => x;
            set => x = value;
        }
        public int Y
        {
            get => y;
            set => y = value;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public float DistanceTo(HexPoint b)
           => DistanceTo(b.x, b.y);

        public float DistanceTo(int x, int y)
        {
            return (Math.Abs(X - x) + Math.Abs(X + Y - x - y) + Math.Abs(Y - y)) / 2;
        }

        #region Operators

        public static HexPoint operator +(HexPoint left, HexPoint right)
        {
            return new HexPoint(
                left.x + right.x,
                left.y + right.y
                );
        }

        public static HexPoint operator -(HexPoint left, HexPoint right)
        {
            return new HexPoint(
                left.x - right.x,
                left.y - right.y
                );
        }

        public static HexPoint operator *(HexPoint left, int right)
        {
            return new HexPoint(
                left.x * right,
                left.y * right
                );
        }

        public static HexPoint operator /(HexPoint left, int right)
        {
            return new HexPoint(
                left.x / right,
                left.y / right
                );
        }

        public static HexPoint operator *(int left, HexPoint right)
        {
            return new HexPoint(
                right.x * left,
                right.y * left
                );
        }

        public static HexPoint operator /(int left, HexPoint right)
        {
            return new HexPoint(
                right.x / left,
                right.y / left
                );
        }

        public static bool operator ==(HexPoint left, HexPoint right)
        {
            return left.x == right.x && left.y == right.y;
        }

        public static bool operator !=(HexPoint left, HexPoint right)
        {
            return !(left == right);
        }

        public static implicit operator Vector2(HexPoint hexPoint)
        {
            return new Vector2(hexPoint.x, hexPoint.y);
        }

        #endregion

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[8];
            BitConverter.GetBytes(X).CopyTo(bytes, 0);
            BitConverter.GetBytes(Y).CopyTo(bytes, 4);
            return bytes;
        }

        public int FromBytes(byte[] bytes, int startIndex)
        {
            X = BitConverter.ToInt32(bytes, startIndex);
            Y = BitConverter.ToInt32(bytes, startIndex + 4);
            return 8;
        }
    }
}
