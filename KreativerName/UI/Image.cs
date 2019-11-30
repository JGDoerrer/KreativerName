using System.Drawing;
using KreativerName.Rendering;
using OpenTK;

namespace KreativerName.UI
{
    public class Image : UIElement
    {
        public Image(Texture2D texture)
        {
            this.texture = texture;
        }
        public Image(Texture2D texture, RectangleF? sourceRect)
        {
            this.texture = texture;
            this.sourceRect = sourceRect;
        }
        public Image(Texture2D texture, RectangleF? sourceRect, Color color)
        {
            this.texture = texture;
            this.sourceRect = sourceRect;
            Color = color;
        }

        Texture2D texture;
        RectangleF? sourceRect;
        public Color Color { get; set; } = Color.White;

        public override void Update(Vector2 windowSize)
        {
        }

        public override void Render(Vector2 windowSize)
        {
            Vector2 pos = new Vector2(GetX(windowSize), GetY(windowSize));
            float width = sourceRect.HasValue ? sourceRect.Value.Width : texture.Width;
            float height = sourceRect.HasValue ? sourceRect.Value.Height : texture.Height;
            Vector2 scale = new Vector2(GetWidth(windowSize) / width, GetHeight(windowSize) / height);

            TextureRenderer.Draw(texture, pos, scale, Color, sourceRect);
        }
    }
}
