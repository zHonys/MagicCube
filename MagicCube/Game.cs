using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

using StbImageSharp;

namespace MagicCube
{
    public class Game : GameWindow
    {
        public Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            GL.Enable(EnableCap.DepthTest);
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // code


            //

            SwapBuffers();
        }
        protected override void OnUnload()
        {
            base.OnUnload();
        }
        public void Run(int width, int height, float Hertz, string relativePath)
        {
            Size = new Vector2i(width, height);
            GL.Viewport(0, 0, width, height);

            UpdateFrequency = Hertz;
            RenderFrequency = Hertz;

            ImageResult image = ImageResult.FromStream(File.OpenRead($@"{Directory.GetCurrentDirectory()}\{relativePath}"));
            Icon = new WindowIcon(new Image(image.Width, image.Height, image.Data));

            Run();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
