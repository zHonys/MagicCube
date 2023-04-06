using Silk.NET.OpenGL;

namespace MagicCube.Controls
{
    public class Texture2D
    {
        GL _GL;
        uint _handle;
        string _uniformName;
        public Texture2D(GL gl, uint handle, string uniformName)
        {
            _GL = gl;
            _handle = handle;
            _uniformName = uniformName;
        }
        public void BindTexture(Shader shader, TextureUnit textureUnit=TextureUnit.Texture0)
        {
            _GL.ActiveTexture(textureUnit);
            _GL.BindTexture(GLEnum.Texture2D, _handle);
            shader.SetUniform(_uniformName, (uint)(textureUnit - 33984));
        }
        public static void BindTextures(Shader shader, IEnumerable<Texture2D> textures)
        {
            foreach((Texture2D texture, int index) item in textures.Select((texture, index) => (texture, index)))
            {
                item.texture.BindTexture(shader, (TextureUnit)(item.index + 33984));
            }
        }
        public unsafe static Texture2D CreateTexture(GL gl, string uniformName, byte[,] data, uint width, uint height, PixelFormat pixelFormat)
        {
            uint handle = gl.GenTexture();
            gl.BindTexture(GLEnum.Texture2D, handle);

            fixed (void* pointer = data)
            {
                gl.TexImage2D(
                    target: GLEnum.Texture2D,
                    level: 0,
                    internalformat: (InternalFormat)pixelFormat,
                    width: width,
                    height: height,
                    border: 0,
                    format: pixelFormat,
                    type: PixelType.UnsignedByte,
                    pixels: pointer);
            }
            gl.GenerateMipmap(GLEnum.Texture2D);

            gl.BindTexture(GLEnum.Texture2D, 0);
            return new Texture2D(gl, handle, uniformName);
        }
    }
}
