using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    public class Model
    {
        public Model(Mesh mesh)
        {
            Info.Shader = mesh.shader;
            Info.Texture = mesh.texture;

            vbos = new List<int>();

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            Info.VAO = vao;

            AddVBO(3, mesh.vertices.ToArray());
            AddVBO(2, mesh.textureCoords.ToArray());

            AddEBO(mesh.indices.ToArray());
        }

        private void AddVBO(int dimensions, float[] data)
        {
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(vbos.Count, dimensions, VertexAttribPointerType.Float, false, dimensions * sizeof(float), 0);
            GL.EnableVertexAttribArray(vbos.Count);

            vbos.Add(vbo);
        }

        private void AddEBO(uint[] indices)
        {
            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            Info.IndicesCount = indices.Length;
        }

        int vao;

        List<int> vbos;

        public RenderInfo Info;

        public void Delete()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            foreach (int vbo in vbos)
            {
                GL.DeleteBuffer(vbo);
            }

            GL.DeleteVertexArray(vao);
        }
    }
}
