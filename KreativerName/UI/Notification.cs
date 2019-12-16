using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KreativerName.UI
{
    public static class Notification
    {
        static Queue<Note> notifications  = new Queue<Note>();
        static Note? current;

        public static void Update()
        {
        }

        public static void Render(Vector2 windowSize)
        {

        }

        public static void Show(string text, Image image)
        {
            notifications.Enqueue(new Note()
            {
                Text = text,
                Image = image
            });
        }

        struct Note
        {
            public string Text;
            public Image Image;
        }
    }
}
