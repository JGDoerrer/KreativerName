using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    class TextureRenderer
    {
        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, Color color, RectangleF? sourceRect)
            => Draw(texture, position, scale, color, sourceRect, new Vector2(0, 0));

        public static void Draw(Texture2D texture, Vector2 position, Vector2 scale, Color color, RectangleF? sourceRect, Vector2 origin)
        {
            Vector2[] vertecies = new Vector2[4]
            {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1),
            };

            GL.BindTexture(TextureTarget.Texture2D, texture.ID);

            GL.Begin(PrimitiveType.Quads);
            GL.Color4(color);

            for (int i = 0; i < 4; i++)
            {
                if (!sourceRect.HasValue)
                    GL.TexCoord2(vertecies[i]);
                else
                    GL.TexCoord2((sourceRect.Value.Left + vertecies[i].X * sourceRect.Value.Width) / texture.Width,
                       (sourceRect.Value.Top + vertecies[i].Y * sourceRect.Value.Height) / texture.Height);

                vertecies[i].X *= !sourceRect.HasValue ? texture.Width : sourceRect.Value.Width;
                vertecies[i].Y *= !sourceRect.HasValue ? texture.Height : sourceRect.Value.Height;

                vertecies[i] -= origin;
                vertecies[i] *= scale;
                vertecies[i] += position;
            
                vertecies[i].X = (float)Math.Floor(vertecies[i].X);
                vertecies[i].Y = (float)Math.Floor(vertecies[i].Y);

                GL.Vertex2(vertecies[i]);
            }

            GL.End();
        }


        static Vector2 RotatedAround(Vector2 pointToRotate, Vector2 centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                X = (int)(cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (int)(sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
