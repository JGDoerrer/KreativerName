using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KreativerName.Rendering
{
    public struct Mesh
    {
        public Mesh(float[] vert, float[] texCoords, uint[] ind)
        {
            vertices = vert.ToList();
            indices = ind.ToList();
            textureCoords = texCoords.ToList();
        }

        public List<float> vertices;
        public List<float> textureCoords;
        public List<uint> indices;
    }
}
