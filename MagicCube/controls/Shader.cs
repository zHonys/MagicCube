using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MagicCube.controls
{
    public class Shader : IDisposable
    {
        public int handle;
        private bool handleDisposed = false;
        public Shader(string vertShaderPath, string fragShaderPath)
        {
            handle = GL.CreateProgram();

            string currentPath = Directory.GetCurrentDirectory();

            string vertShaderSource = File.ReadAllText(@$"{currentPath}\{vertShaderPath}");
            string fragShaderSource = File.ReadAllText(@$"{currentPath}\{fragShaderPath}");

            int vertShader = GL.CreateShader(ShaderType.VertexShader);
            int fragShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertShader, vertShaderSource);
            GL.ShaderSource(fragShader, fragShaderSource);

            GL.CompileShader(vertShader);
            GL.CompileShader(fragShader);


            GL.AttachShader(handle, vertShader);
            GL.AttachShader(handle, fragShader);

            GL.LinkProgram(handle);

            GL.GetShader(vertShader, ShaderParameter.CompileStatus, out int vertStatus);
            GL.GetShader(fragShader, ShaderParameter.CompileStatus, out int fragStatus);
            GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (vertStatus + fragStatus + linkStatus < 3)
            {
                string vertInfoLog = GL.GetShaderInfoLog(vertShader);
                string fragInfoLog = GL.GetShaderInfoLog(fragShader);
                string linkInfoLog = GL.GetProgramInfoLog(handle);

                Console.WriteLine($"Vertice Shader Log:\n" +
                                  $"{vertInfoLog}\n\n" +
                                  $"Fragment Shader Log:\n" +
                                  $"{fragInfoLog}\n\n" +
                                  $"Program Link Log:\n" +
                                  $"{linkInfoLog}\n\n");
            }

            // Clean UP

            GL.DetachShader(handle, vertShader);
            GL.DetachShader(handle, fragShader);

            GL.DeleteShader(vertShader);
            GL.DeleteShader(fragShader);
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }

        public int GetAttributeLocation(string attributesName)
        {
            return GL.GetAttribLocation(handle, attributesName);
        }
        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(handle, uniformName);
        }
        public static void SetUniform(int location, ref Matrix4 data, bool transpose = false)
        {
            GL.UniformMatrix4(location, transpose, ref data);
        }
        public void SetUniform(string uniformName, ref Matrix4 data, bool transpose = false)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(handle, uniformName), transpose, ref data);
        }
        public void SetUniform(string uniformName, int data)
        {
            GL.Uniform1(GL.GetUniformLocation(handle, uniformName), data);
        }

        #region IDisposable
        ~Shader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!handleDisposed)
            {
                GL.DeleteProgram(handle);
                handleDisposed = true;
            }
            if (disposing)
            {
                handle = 0;
            }
        }
        #endregion
    }
}