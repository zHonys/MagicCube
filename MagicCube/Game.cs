using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

using StbImageSharp;
using MagicCube.controls;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using MagicCube.shapes;

namespace MagicCube
{
    public class Game : GameWindow
    {
        List<IDisposable> disposables= new List<IDisposable>();
        Shader shader;

        ThreeByThree test;

        Camera camera;

        bool shouldUpdate = true;

        public Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            GL.Enable(EnableCap.DepthTest);

            setCursorInputMode(CursorModeValue.CursorDisabled);
            camera = new(KeyboardState, MouseState, 14, MathHelper.DegreesToRadians(60), 10, Size, 0.1f, 100);
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);

            shader = new(@"shaders\model.vert", @"shaders\model.frag");
            disposables.Add(shader);
            //Cube = new(@"assets\Cube\Cube.obj");

            test = new(@"assets\Cube\Cube.obj");
            disposables.Add(test);

            test.Spin();
            test.Spin();
            test.modelMatrix *= Matrix4.CreateTranslation(new(0, -10, -15));
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!shouldUpdate) return;

            float elapsedTime = (float)args.Time;

            if (KeyboardState.IsKeyPressed(Keys.Escape)) Close();
            bool reverse = false;
            if (KeyboardState.IsKeyDown(Keys.LeftShift)) reverse = true;

            if (KeyboardState.IsKeyPressed(Keys.U)) test.U(reverse);
            if (KeyboardState.IsKeyPressed(Keys.R)) test.R(reverse);
            if (KeyboardState.IsKeyPressed(Keys.C)) test.D(reverse);
            if (KeyboardState.IsKeyPressed(Keys.L)) test.L(reverse);
            if (KeyboardState.IsKeyPressed(Keys.F)) test.F(reverse);
            if (KeyboardState.IsKeyPressed(Keys.B)) test.B(reverse);
            if (KeyboardState.IsKeyPressed(Keys.V)) test.Spin();

            camera.Update(elapsedTime);
            test.Update(elapsedTime);
            matrixUpdate();
        }
        private void matrixUpdate()
        {
            shader.SetUniform("View", camera.View);
            shader.SetUniform("Projection", camera.Projection);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (!shouldUpdate) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;
            model = Matrix4.CreateTranslation(0, 0, -20);

            // code

            shader.Use();

            test.Draw(shader);
            //

            SwapBuffers();
        }
        protected override void OnUnload()
        {
            disposables.ForEach(i => i.Dispose());
            base.OnUnload();
        }
        public void Run(int width, int height, float Hertz, string relativePath)
        {
            Size = new Vector2i(width, height);
            GL.Viewport(0, 0, width, height);

            UpdateFrequency = Hertz;
            RenderFrequency = Hertz;

            ImageResult image = ImageResult.FromStream(File.OpenRead($@"{Directory.GetCurrentDirectory()}\{relativePath}"));
            Icon = new WindowIcon(new OpenTK.Windowing.Common.Input.Image(image.Width, image.Height, image.Data));

            Run();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            Size = new Vector2i(e.Width, e.Height);
            camera.ScreenSize = Size;
        }
        protected unsafe override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                setCursorInputMode(CursorModeValue.CursorDisabled);
                shouldUpdate = true;
             }
        }
        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            base.OnFocusedChanged(e);
            if (!e.IsFocused)
            {
                setCursorInputMode(CursorModeValue.CursorNormal);
                shouldUpdate = false;
            }
        }
        private unsafe void setCursorInputMode(CursorModeValue value)
        {
            GLFW.SetInputMode(WindowPtr, CursorStateAttribute.Cursor, value);
        }
    }
}
