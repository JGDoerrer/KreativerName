using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Grid
{
    public struct HexPoint
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

        public static bool operator ==(HexPoint left, HexPoint right)
        {
            return left.x == right.x && left.y == right.y;
        }

        public static bool operator !=(HexPoint left, HexPoint right)
        {
            return !(left == right);
        }

        #endregion
    }
}
