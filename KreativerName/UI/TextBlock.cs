using System.Drawing;
using System.Linq;
using KreativerName.Rendering;
using KreativerName.UI.Constraints;
using OpenTK;

namespace KreativerName.UI
{
    public class TextBlock : UIElement
    {
        public TextBlock()
        {
        }
        public TextBlock(string text, float size = 2)
        {
            Text = text;
            Size = size;
        }
        public TextBlock(string text, float size, int x, int y)
        {
            Text = text;
            Size = size;
            constraints = new UIConstraints(
                new PixelConstraint(x),
                new PixelConstraint(y),
                new PixelConstraint((int)TextWidth),
                new PixelConstraint((int)TextHeight));
        }

        public string Text { get; set; }//.ToUpper(); }
        public float Size { get; set; }
        public Color Color { get; set; } = Color.Black;
        public float TextWidth => Text.Sum(x => !char.IsUpper(x) ? Size * 6 : Size * 7);
        public float TextHeight => Size * 6 + Text.Count(x => x == '\n') * Size * 8;

        public override void Update(Vector2 windowSize)
        {

        }

        public override void Render(Vector2 windowSize)
        {
            Vector2 pos = new Vector2(GetX(windowSize), GetY(windowSize));

            TextRenderer.RenderString(Text, pos, Color, Size);
        }

    }
}
