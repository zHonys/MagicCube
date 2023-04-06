using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using MagicCube.Controls;
using StbImageSharp;

namespace MagicCube
{
    #pragma warning disable CS8618
    public class Game : IDisposable
    {
        GL _GL;
        readonly IWindow _window;
        IInputContext _input;

        Camera _camera;
        public Game(string title, string iconPath, int width, int heigth, int fps)
        {
            WindowOptions opts = WindowOptions.Default;
            opts.Title = title;
            opts.Size = new(width, heigth);
            opts.UpdatesPerSecond = fps;
            opts.FramesPerSecond  = fps;
            opts.ShouldSwapAutomatically = false;

            _window = Window.Create(opts);
            setIcon(iconPath);

            _window.Load   += onLoad;
            _window.Update += onUpdate;
            _window.Render += onRender;
            _window.Resize += onResize;
        }
        private void onLoad()
        {
            _GL    = _window.CreateOpenGL();
            _input = _window.CreateInput();

            _camera = new(_input, _window.Size, 8, Scalar.DegreesToRadians(45f), 0.01f, 100f);
        }
        private void onUpdate(double elapsedTime)
        {
            _camera.Update(elapsedTime);
        }
        private void onRender(double elapsedTime)
        {
            _GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Code Area



            // End Area

            _window.SwapBuffers();
        }
        #region Util
        private void onResize(Vector2D<int> newSIze)
        {
            _GL.Viewport(newSIze);
        }
        public void Run()
        {
            _window.Run();
        }
        private void setIcon(string relativeFilePath)
        {
            string path = Path.Join(Directory.GetCurrentDirectory(), relativeFilePath);
            using(Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                var rawImage = new RawImage(image.Width, image.Height, image.Data);
                _window.SetWindowIcon(ref rawImage);
            }
        }
        #endregion
        #region Dispose
        readonly private List<IDisposable> disposables = new();
        private bool disposed = false;
        ~Game()
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
            if (disposing)
            {
                disposables.ForEach(item => item.Dispose());
            }

            disposed = true;
        }
        #endregion
    }
    /*public class Game : GameWindow
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

            camera = new(MouseState, 14, MathHelper.DegreesToRadians(60), 10, Size, 0.1f, 100);
            addToLists(camera);
            cube = new(@"assets\Cube\Cube.obj", KeyboardState, 0.5f, RotationFunctionsType.Sin);
            addToLists(cube);
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.1f, 0.1f);
        }

        private void matrixUpdate()
        {
            shader.SetUniform("View", camera.View);
            shader.SetUniform("Projection", camera.Projection);

            Matrix4 projection = Matrix4.CreateOrthographic(Size.X, Size.Y, 0.1f, 20);
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
    }*/
}
