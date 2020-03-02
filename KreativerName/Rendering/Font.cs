using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KreativerName.Rendering
{
    public class Font
    {
        public Font(int charWidth, int charHeight, int charsPerRow, char startChar, Texture2D texture)
        {
            this.charWidth = charWidth;
            this.charHeight = charHeight;
            this.charsPerRow = charsPerRow;
            this.startChar = startChar;
            this.texture = texture;
        }

        int charWidth, charHeight;
        int charsPerRow;
        char startChar;
        Texture2D texture;

        public RectangleF GetTexRect(char c)
        {
            int index = c - startChar + 1;
            return new RectangleF(index % charsPerRow * charWidth / (float)texture.Width, index / charsPerRow * charHeight / (float)texture.Height, charWidth / (float)texture.Width, charHeight / (float)texture.Height);
        }

        public Mesh GetMesh(string s, Vector2 startPos, Vector2 scale)
        {
            MeshBuilder builder = new MeshBuilder();

            foreach (char c in s)
            {
                builder.AddRectangle(new RectangleF(startPos.X, startPos.Y, charWidth * scale.X, charHeight * scale.Y), GetTexRect(c));

                startPos += new Vector2(charWidth * scale.X, 0);
            }

            return builder.Mesh;
        }
    }
}
