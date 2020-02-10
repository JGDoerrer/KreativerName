using System.Drawing;
using KreativerName.Rendering;
using OpenTK;

namespace KreativerName.UI
{
    public class TextBlock : UIElement
    {
        public TextBlock()
        { }

        public TextBlock(string text, float size = 2)
        {
            Text = text;
            Size = size;
        }
        public TextBlock(string text, float size, int x, int y) : base(x, y, (int)TextRenderer.GetWidth(text, size), (int)TextRenderer.GetHeight(text, size))
        {
            Text = text;
            Size = size;
        }

        public string Text { get; set; }//.ToUpper(); }
        public float Size { get; set; }
        public Color Color { get; set; } = Color.Black;
        public float TextWidth => TextRenderer.GetWidth(Text, Size);
        public float TextHeight => TextRenderer.GetHeight(Text, Size);

        public override void Update(Vector2 windowSize)
        {

        }

        public override void Render(Vector2 windowSize)
        {
            TextRenderer.RenderString(Text, ActualPosition, Color, Size);
        }

    }
}
