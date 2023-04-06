using Silk.NET.OpenGL;
using Silk.NET.Maths;

namespace MagicCube.Controls
{
    public class Shader : IDisposable
    {
        private readonly GL Gl;
        public readonly uint Handle;
        public Shader(GL gl, string vertShaderRelativePath, string fragShaderRelativePath)
        {
            Gl = gl;

            Handle = Gl.CreateProgram();
            uint vert = Gl.CreateShader(ShaderType.VertexShader);
            uint frag = Gl.CreateShader(ShaderType.FragmentShader);

            string curPath = Directory.GetCurrentDirectory();
            var vertSource = File.ReadAllText(Path.Join(curPath, vertShaderRelativePath));
            var fragSource = File.ReadAllText(Path.Join(curPath, fragShaderRelativePath));
            Gl.ShaderSource(vert, vertSource);
            Gl.ShaderSource(frag, fragSource);

            Gl.CompileShader(vert);
            Gl.CompileShader(frag);

            Gl.AttachShader(Handle, vert);
            Gl.AttachShader(Handle, frag);

            Gl.LinkProgram(Handle);
            Gl.GetShader(vert, ShaderParameterName.CompileStatus, out int vertStatus);
            Gl.GetShader(frag, ShaderParameterName.CompileStatus, out int fragStatus);
            Gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int progStatus);

            if (vertStatus + fragStatus + progStatus < 3)
            {
                Console.WriteLine($"Vertex Shader Log:\n" +
                                  $"{Gl.GetShaderInfoLog(vert)}\n\n" +
                                  $"Fragment Shader Log:\n" +
                                  $"{Gl.GetShaderInfoLog(frag)}\n\n" +
                                  $"Program Link Status Log:\n" +
                                  $"{Gl.GetProgramInfoLog(Handle)}\n\n");
            }

            Gl.DetachShader(Handle, vert);
            Gl.DetachShader(Handle, frag);

            Gl.DeleteShader(vert);
            Gl.DeleteShader(frag);
        }
        public void Use() => Gl.UseProgram(Handle);
        public unsafe void SetUniform(string uniformName, uint value)
        {
            Gl.Uniform1(GetUniformLocation(uniformName), (int)value);
        }
        public unsafe void SetUniform(string uniformName, float value)
        {
            Gl.Uniform1(GetUniformLocation(uniformName), value);
        }
        public unsafe void SetUniform(string uniformName, Vector3D<float> value)
        {
            Gl.Uniform3(GetUniformLocation(uniformName), value.ToSystem());
        }
        public unsafe void SetUniform(string uniformName, Matrix4X4<float> matrix, bool transpose = false)
        {
            Gl.UniformMatrix4(GetUniformLocation(uniformName), 1, transpose, (float*)&matrix);
        }
        public int GetUniformLocation(string uniformName) => Gl.GetUniformLocation(Handle, uniformName);
        #region
        private bool disposed = false;
        ~Shader()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (!disposing)
            {
                // Good practice
            }
            Gl.DeleteProgram(Handle);
            disposed = true;
        }
        #endregion
    }
}