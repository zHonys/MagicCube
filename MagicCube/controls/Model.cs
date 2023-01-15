using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace MagicCube.controls
{
    public class Model : IDisposable
    {
        public string Name;

        private readonly string _directory;
        private List<Mesh> _meshes = new();
        private List<Texture> _textures = new();

        public Model(string modelPath)
        {
            _directory = @$"{Directory.GetCurrentDirectory()}\{modelPath.Substring(0, modelPath.LastIndexOf(@"\"))}";
            LoadModel(@$"{Directory.GetCurrentDirectory()}\{modelPath}");
        }
        public void Draw(Shader shader)
        {
            _meshes.ForEach(m =>
            {
                m.Draw(shader);
            });
        }

        private void LoadModel(string path)
        {
            AssimpContext importer = new();
            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate |
                                                    PostProcessSteps.FlipUVs |
                                                    PostProcessSteps.GenerateNormals);
            ProcessNode(scene.RootNode, scene);
            Name = scene.RootNode.Name;
        }
        private void ProcessNode(Node node, Scene scene)
        {
            if (node.MeshCount > 0)
            {
                node.MeshIndices.ForEach(i =>
                {
                    _meshes.Add(ProcessMesh(scene.Meshes[i], scene));
                });
            }
            if (node.ChildCount > 0)
            {
                foreach(Node inerNode in node.Children)
                {
                    ProcessNode(inerNode, scene);
                }
            }

        }
        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {;
            List<Vertex> vertices = new();
            uint[] indices = mesh.GetUnsignedIndices();
            List<Texture> textures = new();

            List<Vector3> positions = mesh.Vertices.Select(v => { return new Vector3(v.X, v.Y, v.Z); }).ToList();

            List<Vector3> normals = mesh.Normals.Select(v => { return new Vector3(v.X, v.Y, v.Z); }).ToList();

            List<Vector2> texCoords = mesh.TextureCoordinateChannels[0].Select(v => { return new Vector2(v.X, v.Y); }).ToList();

            foreach(var (First, Second, Third) in positions.Zip(normals, texCoords))
            {
                vertices.Add(new Vertex()
                {
                    Position = First,
                    Normal = Second,
                    UV = Third
                });
            }

            if (mesh.MaterialIndex >= 0)
            {
                textures = ProcessMaterial(scene.Materials[mesh.MaterialIndex]);
            }
            return new Mesh(mesh.Name, vertices, indices, textures);
        }
        private List<Texture> ProcessMaterial(Material material)
        {
            List<Texture> textures = new();
            foreach(TextureSlot textureSlot in material.GetAllMaterialTextures())
            {
                Texture texture = _textures.Find(tex => tex.Path == textureSlot.FilePath);
                if (texture.Equals(default(Texture)))
                {
                    texture = ProcessTexture(textureSlot);
                    _textures.Add(texture);
                }
                textures.Add(texture);
            }
            return textures;
        }
        private Texture ProcessTexture(TextureSlot texture)
        {
            string path = @$"{_directory}\{texture.FilePath}";
            ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);

            int handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);


            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                          image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GetTexParameter(texture);

            return new Texture()
            {
                Handle = handle,
                Type = texture.TextureType.ToString(),
                Path = texture.FilePath
            };
        }

        private void GetTexParameter(TextureSlot texture)
        {
            int mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat;
            if (texture.WrapModeU == Assimp.TextureWrapMode.Clamp)
            {
                mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge;
            }
            else if (texture.WrapModeU == Assimp.TextureWrapMode.Mirror)
            {
                mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat;
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, mode);

            mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat;
            if (texture.WrapModeV == Assimp.TextureWrapMode.Clamp)
            {
                mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.ClampToEdge;
            }
            else if (texture.WrapModeV == Assimp.TextureWrapMode.Mirror)
            {
                mode = (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.MirroredRepeat;
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, mode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMinFilter.Linear);
        }

        #region Disposa

        ~Model()
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
            _meshes.ForEach(m => m.Dispose(disposing));
            _textures.ForEach(tex =>
            {
                GL.DeleteTexture(tex.Handle);
            });
            if (disposing)
            {
                _meshes.Clear();
                _textures.Clear();
            }
        }

        #endregion
    }
}
