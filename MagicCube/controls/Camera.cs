using MagicCube.Interfaces;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MagicCube.controls
{
    public class Camera : IUpdatable
    {
        //1d Vars
        public float Speed;

        float _fov;
        float _depthNear;
        float _depthFar;
        int _sensitivity;

        float radius = 20;
        float yaw = 0;
        float pitch = 0;

        //Vectors
        Vector3 position = new();
        readonly Vector3 worldUp = new(0, 1, 0);

        Vector2 _screenSize;
        public Vector2 ScreenSize
        {
            get { return _screenSize; }
            set
            {
                _screenSize = value;
                Matrix4.CreatePerspectiveFieldOfView(_fov, _screenSize.X / _screenSize.Y, _depthNear, _depthFar, out Projection);
            }
        }

        //Matrix
        public Matrix4 View;
        public Matrix4 Projection;

        // Input
        readonly MouseState _mouse;

        public Camera(MouseState mouse, float speed, float fov, int sensitivity, Vector2 screenSize, float depthNear, float depthFar)
        {
            _mouse = mouse;

            Speed = speed;
            _fov = fov;
            _sensitivity = sensitivity;
            _depthNear = depthNear;
            _depthFar = depthFar;
            ScreenSize = screenSize;
        }

        public void Update(float elapsedTime)
        {;
            Move(elapsedTime);

            View = Matrix4.LookAt(position * radius, Vector3.Zero, worldUp);
        }

        private void Move(float elapsed)
        {
            Vector2 offSet = _mouse.Delta;

            offSet *= (MathHelper.Pi / 180) * (_sensitivity / 100f);

            yaw += offSet.X;
            pitch -= offSet.Y;

            float maxPitch = MathHelper.DegreesToRadians(89);

            if (pitch > maxPitch) pitch = maxPitch;
            if (pitch < -maxPitch) pitch = -maxPitch;

            position.X = MathF.Sin(yaw);
            position.Y = MathF.Sin(pitch);
            position.Z = MathF.Cos(yaw);
            position.Normalize();
        }
    }
}
