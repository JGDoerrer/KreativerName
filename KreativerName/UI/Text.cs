using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KreativerName.Rendering;
using OpenTK;

namespace KreativerName.UI
{
    public class Text : UIElement
    {
        public Text()
        { }

        public Text(string text, float size = 2)
        {
            String = text;
            Size = size;
        }

        string s;

        public string String { get => s; set => s = value; }//.ToUpper(); }
        public float Size { get; set; }
        public Color Color { get; set; } = Color.Black;

        public override void Update(Vector2 windowSize)
        {

        }

        public override void Render(Vector2 windowSize)
        {
            Texture2D tex = Textures.Get("Font");
            Vector2 pos = new Vector2(GetX(windowSize), GetY(windowSize));

            foreach (char c in String)
            {
                if (c <= 126 && c >= 32 && pos.X < GetX(windowSize) + GetWidth(windowSize))
                {
                    RectangleF sourceRect = new RectangleF(((c - 32) % 16) * 6, ((c - 32) / 16) * 6, 6, 6);
                    TextureRenderer.Draw(tex, pos, Vector2.One * Size, Color, sourceRect);
                    pos.X += 6 * Size;
                }
            }
        }
    }
}
