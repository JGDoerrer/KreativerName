using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace KreativerName.Rendering
{
    public class MeshBuilder
    {
        public MeshBuilder()
        {
            mesh.vertices = new List<float>();
            mesh.textureCoords = new List<float>();
            mesh.indices = new List<uint>();
        }

        Mesh mesh;
        uint vertices = 0;

        public Mesh Mesh => mesh;

        public void AddIndex(uint index)
        {
            mesh.indices.Add(index);
        }
        
        public void AddIndices(uint[] indices)
        {
            mesh.indices.AddRange(indices);
        }

        public void AddVertex(Vector2 vertex, Vector2 texCorrd)
        {
            mesh.vertices.Add(vertex.X);
            mesh.vertices.Add(vertex.Y);
            mesh.vertices.Add(0);

            mesh.textureCoords.Add(texCorrd.X);
            mesh.textureCoords.Add(texCorrd.Y);

            vertices++;
        }

        public void AddVertex(Vector3 vertex, Vector2 texCorrd)
        {
            mesh.vertices.Add(vertex.X);
            mesh.vertices.Add(vertex.Y);
            mesh.vertices.Add(vertex.Z);

            mesh.textureCoords.Add(texCorrd.X);
            mesh.textureCoords.Add(texCorrd.Y);

            vertices++;
        }

        public void AddTriangle(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 t1, Vector2 t2, Vector2 t3)
        {
            AddIndex(vertices);
            AddVertex(v1, t1);
            AddIndex(vertices);
            AddVertex(v2, t2);
            AddIndex(vertices);
            AddVertex(v3, t3);
        }

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector2 t1, Vector2 t2, Vector2 t3)
        {
            AddIndex(vertices);
            AddVertex(v1, t1);
            AddIndex(vertices);
            AddVertex(v2, t2);
            AddIndex(vertices);
            AddVertex(v3, t3);
        }

        public void AddRectangle(RectangleF rect, RectangleF tex)
        {
            AddIndex(vertices);
            AddIndex(vertices+1);
            AddIndex(vertices+3);

            AddIndex(vertices+1);
            AddIndex(vertices+2);
            AddIndex(vertices+3);

            AddVertex(new Vector2(rect.X, rect.Y), new Vector2(tex.X, tex.Y));
            AddVertex(new Vector2(rect.X + rect.Width, rect.Y), new Vector2(tex.X + tex.Width, tex.Y));
            AddVertex(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), new Vector2(tex.X + tex.Width, tex.Y + tex.Height));
            AddVertex(new Vector2(rect.X , rect.Y + rect.Height), new Vector2(tex.X, tex.Y + tex.Height));            
        }
    }
}
