using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Grid
{
    public struct HexLayout
    {
        public HexLayout(Matrix2 forward, Matrix2 inverse, Vector2 origin, float size, float startAngle)
        {
            f = forward;
            b = inverse;
            this.origin = origin;
            this.size = size;
            this.startAngle = startAngle;
        }

        internal Matrix2 f;
        internal Matrix2 b;
        internal Vector2 origin;
        internal float size;
        internal float startAngle;

        public Vector2 HexToPixel(Hex h)
        {
            float x = (f.M11 * h.X + f.M12 * h.Y) * size;
            float y = (f.M21 * h.X + f.M22 * h.Y) * size;
            return new Vector2(x + origin.X, y + origin.Y);
        }

        public HexPoint PixelToHex(Vector2 v)
        {
            v -= origin;
            v /= size;


            float x = b.M11 * v.X + b.M12 * v.Y;
            float y = b.M21 * v.X + b.M22 * v.Y;
            float z = -x - y;

            float rx = (float)Math.Round(x);
            float ry = (float)Math.Round(y);
            float rz = (float)Math.Round(z);

            float dx = Math.Abs(rx - x);
            float dy = Math.Abs(ry - y);
            float dz = Math.Abs(rz - z);

            if (dx > dy && dx > dz)
                rx = -ry - rz;
            else if (dy > dz)
                ry = -rx - rz;
            else
                rz = -rx - ry;

            return new HexPoint((int)rx, (int)ry);
        }

        public Vector2 HexCorner(Hex h, int i)
        {
            double angle = 2 * Math.PI * (startAngle + i) / 6;
            return HexToPixel(h) + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle) * size);
        }


    }
}
