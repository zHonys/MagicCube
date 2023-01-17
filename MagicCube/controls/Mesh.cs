using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace MagicCube.controls
{
    public class Mesh : IDisposable
    {
        public string Name;
        public Matrix4 modelMatrix = Matrix4.Identity;

        int _VAO, _VBO, _EBO;

        List<Vertex> _vertices;
        uint[] _indices;
        List<Texture> _textures;

        public Mesh(string name, List<Vertex> vertices, uint[] indices, List<Texture> textures)
        {
            Name = name;

            _vertices = vertices;
            _indices  = indices;
            _textures = textures;

            SetupMesh();
        }
        public void Draw(Shader shader)
        {
            GL.BindVertexArray(_VAO);

            shader.SetUniform("Model", ref modelMatrix);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textures[0].Handle);
            shader.SetUniform("diffuse", 0);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
        }
        private unsafe void SetupMesh()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);

            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(Vertex) * _vertices.Count, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            //int aPosLocation = 0;
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), 0);

            //int aNormalLocation = 1;
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("Normal"));

            //int aTexCoordLocation = 2;
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("UV"));

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Length, _indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        #region Disposable

        ~Mesh()
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
            GL.DeleteBuffer(_VBO | _EBO);
            GL.DeleteVertexArray(_VAO);

            if (disposing)
            {
                _vertices.Clear();
                _textures.Clear();
            }
        }

        #endregion
    }

    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;
    }
    public struct Texture
    {
        public int Handle;
        public string Type;
        public string Path;
    }
}
