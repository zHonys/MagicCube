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
using MagicCube.Interfaces;

namespace MagicCube
{
    public class Game : GameWindow
    {
        List<IDisposable> disposables = new List<IDisposable>();
        List<IUpdatable> updatables = new List<IUpdatable>();
        List<IDrawable> drawables = new List<IDrawable>();
        Shader shader;

        ThreeByThree cube;

        Camera camera;

        bool shouldUpdate = true;

        public Game() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            GL.Enable(EnableCap.DepthTest);

            setCursorInputMode(CursorModeValue.CursorDisabled);
            shader = new(@"shaders\model.vert", @"shaders\model.frag");

            camera = new(KeyboardState, MouseState, 14, MathHelper.DegreesToRadians(60), 10, Size, 0.1f, 100);
            addToLists(camera);
            cube = new(@"assets\Cube\Cube.obj", KeyboardState, 0.5f, RotationFunctionsType.Sin);
            addToLists(cube);
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);

            //cube.Spin(Vector3.UnitY, 180, false);
            cube.modelMatrix *= Matrix4.CreateTranslation(new(0, -10, -15));
        }

        private void matrixUpdate()
        {
            shader.SetUniform("View", camera.View);
            shader.SetUniform("Projection", camera.Projection);
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!shouldUpdate) return;

            float elapsedTime = (float)args.Time;

            if (KeyboardState.IsKeyPressed(Keys.Escape)) Close();

            updatables.ForEach(up => up.Update(elapsedTime));
            matrixUpdate();
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (!shouldUpdate) return;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;
            model = Matrix4.CreateTranslation(0, 0, -20);

            // code

            shader.Use();

            drawables.ForEach(obj => obj.Draw(shader));
            //

            SwapBuffers();
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
        protected override void OnUnload()
        {
            disposables.ForEach(i => i.Dispose());
            base.OnUnload();
        }
        private void addToLists(object item)
        {
            if (item is IDisposable) disposables.Add((IDisposable)item);
            if (item is IUpdatable) updatables.Add((IUpdatable)item);
            if (item is IDrawable) drawables.Add((IDrawable)item);
        }
    }
}
