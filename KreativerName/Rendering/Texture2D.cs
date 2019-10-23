using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    public struct Texture2D
    {
        public Texture2D(int id, int width, int height)
        {
            ID = id;
            Width = width;
            Height = height;
        }

        public int ID { get; }

        public int Width { get; }

        public int Height { get; }

    }
}
