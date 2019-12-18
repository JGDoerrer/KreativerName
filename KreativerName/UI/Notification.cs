using System.Collections.Generic;
using System.Drawing;
using KreativerName.Rendering;
using OpenTK;

namespace KreativerName.UI
{
    public static class Notification
    {
        static Queue<Note> notifications = new Queue<Note>();
        static Note? current;
        static int animationIn;
        static int animationStay;
        static int animationOut;
        const int maxAnimation = 120;

        public static void Update()
        {
            if (animationIn >= 0)
                animationIn--;
            if (animationOut >= 0)
                animationOut--;
            if (animationStay >= 0)
                animationStay--;

            if (animationIn == 0)
                animationStay = maxAnimation;
            if (animationStay == 0)
                animationOut = maxAnimation;
            if (animationOut < 0 && animationStay < 0 && animationIn < 0)
            {
                if (notifications.Count > 0)
                {
                    current = notifications.Dequeue();
                    animationIn = maxAnimation;
                }
                else
                    current = null;
            }
        }

        public static void Render(Vector2 windowSize)
        {
            if (current.HasValue)
            {
                const int a = 8;
                const float scale = 2;

                float animIn = QuadraticInOut((float)animationIn / maxAnimation);
                float animOut = QuadraticInOut((float)animationOut / maxAnimation);

                if (animationIn > 0 || animationStay > 0)
                    animOut = 1;

                float w = TextRenderer.GetWidth(current?.Text) + 16;
                float h = TextRenderer.GetHeight(current?.Text) + 16;
                float x = windowSize.X - w * animOut;
                float y = -h * animIn + 1;
                
                Color color = Color.White;
                Texture2D tex = Textures.Get("TextBox");

                // corner top left
                TextureRenderer.Draw(tex, new Vector2(x, y), Vector2.One * scale, color, new RectangleF(0, 0, a, a));
                // corner top right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y), Vector2.One * scale, color, new RectangleF(a * 2, 0, a, a));
                // corner bottom left
                TextureRenderer.Draw(tex, new Vector2(x, y + h - a * scale), Vector2.One * scale, color, new RectangleF(0, a * 2, a, a));
                // corner bottom right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + h - a * scale), Vector2.One * scale, color, new RectangleF(a * 2, a * 2, a, a));
                // left
                TextureRenderer.Draw(tex, new Vector2(x, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(0, a, a, a));
                // top
                TextureRenderer.Draw(tex, new Vector2(x + a * scale, y), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(a, 0, a, a));
                // right
                TextureRenderer.Draw(tex, new Vector2(x + w - a * scale, y + a * scale), new Vector2(1, h / (a * scale) - 2) * scale, color, new RectangleF(a * 2, a, a, a));
                // bottom
                TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + h - a * scale), new Vector2(w / (a * scale) - 2, 1) * scale, color, new RectangleF(a, a * 2, a, a));
                // center
                TextureRenderer.Draw(tex, new Vector2(x + a * scale, y + a * scale), new Vector2(w / (a * scale) - 2, h / (a * scale) - 2) * scale, color, new RectangleF(a, a, a, a));

                TextRenderer.RenderString(current?.Text, new Vector2(x + 8, y + 8), Color.Black, current.Value.TextSize);
            }
        }

        public static void Show(string text, float textSize = 2)
        {
            notifications.Enqueue(new Note()
            {
                Text = text,
                TextSize = textSize,
            });
        }

        private static float QuadraticInOut(float t)
           => t * t / (2 * t * t - 2 * t + 1);

        struct Note
        {
            public string Text;
            public float TextSize;
        }
    }
}
