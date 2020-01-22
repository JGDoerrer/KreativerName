using System;
using OpenTK;

namespace KreativerName.Grid
{
    /// <summary>
    /// Stores information of rendering a HexGrid.
    /// </summary>
    public struct HexLayout
    {
        /// <summary>
        /// Creates a new HexLayout.
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="inverse"></param>
        /// <param name="origin">Origin of the grid.</param>
        /// <param name="size">Size of a hex.</param>
        /// <param name="startAngle">Rotation of a hex.</param>
        public HexLayout(Matrix2 forward, Matrix2 inverse, Vector2 origin, float size, float startAngle)
        {
            f = forward;
            b = inverse;
            this.origin = origin;
            this.size = size;
            this.startAngle = startAngle;
            spacing = 1;
        }

        internal Matrix2 f;
        internal Matrix2 b;
        internal Vector2 origin;
        internal float size;
        internal float startAngle;
        internal float spacing;

        const float sqrt3 = 1.732050807568877293527446341505872366942805253810380628055f;

        /// <summary>
        /// Converts a hex position to a pixel position.
        /// </summary>
        /// <param name="h">The hex position</param>
        /// <returns>Returns the pixel position</returns>
        public Vector2 HexToPixel(Vector2 h)
        {
            float x = (f.M11 * h.X + f.M12 * h.Y) * size;
            float y = (f.M21 * h.X + f.M22 * h.Y) * size;
            return new Vector2(x + origin.X, y + origin.Y);
        }

        /// <summary>
        /// Converts a pixel position to a hex position.
        /// </summary>
        /// <param name="v">The pixel position</param>
        /// <returns>Returns the hex position</returns>
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

        /// <summary>
        /// Returns the pixel position of a corner of a hex.
        /// </summary>
        /// <param name="h">The position of the hex</param>
        /// <param name="i">The index of the corner</param>
        /// <returns>Returns the pixel position of a corner of a hex.</returns>
        public Vector2 HexCorner(Vector2 h, int i)
        {
            double angle = 2 * Math.PI * (startAngle + i) / 6;
            float cos;
            float sin;

            switch ((startAngle + i) % 6)
            {
                case 0f:
                    sin = 0;
                    cos = 1;
                    break;
                case 0.5f:
                    sin = 0.5f;
                    cos = sqrt3 / 2f;
                    break;
                case 1f:
                    sin = sqrt3 / 2f;
                    cos = 0.5f;
                    break;
                case 1.5f:
                    sin = 1;
                    cos = 0;
                    break;
                case 2f:
                    sin = sqrt3 / 2f;
                    cos = -0.5f;
                    break;
                case 2.5f:
                    sin = 0.5f;
                    cos = -sqrt3 / 2f;
                    break;
                case 3f:
                    sin = 0;
                    cos = -1;
                    break;
                case 3.5f:
                    sin = -0.5f;
                    cos = -sqrt3 / 2f;
                    break;
                case 4f:
                    sin = -sqrt3 / 2f;
                    cos = -0.5f;
                    break;
                case 4.5f:
                    sin = -1;
                    cos = 0;
                    break;
                case 5f:
                    sin = -sqrt3 / 2f;
                    cos = 0.5f;
                    break;
                case 5.5f:
                    sin = -0.5f;
                    cos = sqrt3 / 2f;
                    break;
                default:
                    cos = (float)Math.Cos(angle);
                    sin = (float)Math.Sin(angle);
                    break;
            }

            return HexToPixel(h) + new Vector2(cos * size, sin * size);
        }
    }
}
