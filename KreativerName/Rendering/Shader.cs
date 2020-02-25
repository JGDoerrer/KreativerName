using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace KreativerName.Rendering
{
    public class Shader : IDisposable
    {
        readonly int handle;

        Dictionary<string, int> uniformLocations;

        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexSource = LoadSource(vertexPath);
            string fragmentSource = LoadSource(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            // Compile shaders
            GL.ShaderSource(vertexShader, vertexSource);
            GL.ShaderSource(fragmentShader, fragmentSource);
            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, vertexShader);
            GL.AttachShader(handle, fragmentShader);

            LinkProgram(handle);

            GL.DetachShader(handle, vertexShader);
            GL.DetachShader(handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            // Load uniforms
            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);

            uniformLocations = new Dictionary<string, int>();
            for (int i = 0; i < uniformCount; i++)
            {
                string key = GL.GetActiveUniform(handle, i, out _, out _);

                int location = GL.GetUniformLocation(handle, key);

                uniformLocations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(uniformLocations[name], true, ref data);
        }

        public void SetColor(string name, Color data)
        {
            GL.UseProgram(handle);
            GL.Uniform4(uniformLocations[name], data.R / 255f, data.G / 255f, data.B / 255f, data.A / 255f);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        private static string LoadSource(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        #region Dispose

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(handle);

                disposedValue = true;
            }
        }

        ~Shader()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
