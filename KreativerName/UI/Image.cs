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

        Texture2D texture;
        RectangleF? sourceRect;

        public override void Update(Vector2 windowSize)
        {
        }

        public override void Render(Vector2 windowSize)
        {
            Vector2 pos = new Vector2(GetX(windowSize), GetY(windowSize));
            float width = sourceRect.HasValue ? sourceRect.Value.Width : texture.Width;
            float height = sourceRect.HasValue ? sourceRect.Value.Height : texture.Height;
            Vector2 scale = new Vector2(GetWidth(windowSize) / width, GetHeight(windowSize) / height);

            TextureRenderer.Draw(texture, pos, scale, Color.White, sourceRect);
        }
    }
}
