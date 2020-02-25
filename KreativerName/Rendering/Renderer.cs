﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    public static class Renderer
    {
        public static void Render(RenderInfo info)
        {
            GL.BindVertexArray(info.VAO);

            info.Texture.Use();
            info.Shader.Use();

            info.Shader.SetMatrix4("transform", Matrix4.Identity);

            GL.DrawElements(PrimitiveType.Triangles, info.IndicesCount, DrawElementsType.UnsignedInt, 0);
        }
    }
}
