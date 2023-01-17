using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicCube.controls
{
    public class Camera
    {
        //1d Vars
        public float Speed;

        float _fov;
        float _depthNear;
        float _depthFar;
        int _sensitivity;

        float yaw   = MathHelper.Pi;
        float pitch = 0;//-MathHelper.PiOver2;

        //Vectors
        Vector3 position;

        Vector3 cameraX;
        readonly Vector3 worldUp = new(0, 1, 0);
        Vector3 cameraZ;

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
        readonly KeyboardState _keyboardState;
        readonly MouseState _mouse;

        public Camera(KeyboardState keyboardState, MouseState mouse, float speed, float fov, int sensitivity, Vector2 screenSize, float depthNear, float depthFar)
        {
            _keyboardState = keyboardState;
            _mouse = mouse;

            Speed = speed;
            _fov = fov;
            _sensitivity = sensitivity;
            _depthNear = depthNear;
            _depthFar = depthFar;
            ScreenSize = screenSize;
        }

        public void Update(float elapsedTime)
        {
            Look(elapsedTime);
            Move(elapsedTime);

            View = Matrix4.LookAt(position, position + cameraZ, worldUp);
            //Console.WriteLine($"{position}");
        }

        private void Move(float elapsed)
        {
            if (!_keyboardState.IsAnyKeyDown) return;

            Vector2 movement = Vector2.Zero;
            if (_keyboardState.IsKeyDown(Keys.W)) movement += new Vector2(0, 1);
            if (_keyboardState.IsKeyDown(Keys.S)) movement += new Vector2(0, -1);
            if (_keyboardState.IsKeyDown(Keys.A)) movement += new Vector2(-1, 0);
            if (_keyboardState.IsKeyDown(Keys.D)) movement += new Vector2(1, 0);
            if (movement == Vector2.Zero) return;
            movement.Normalize();
            //Console.WriteLine(movement);
            movement *= elapsed * Speed;
            position += (cameraX * movement.X) + (cameraZ * movement.Y);
        }
        private void Look(float elapsed)
        {
            Vector2 offSet = _mouse.Delta;

            offSet *= (MathHelper.Pi/180) * (_sensitivity/100f);

            yaw += offSet.X;
            pitch -= offSet.Y;

            float maxPitch = MathHelper.DegreesToRadians(89);

            if (pitch >  maxPitch) pitch =  maxPitch;
            if (pitch < -maxPitch) pitch = -maxPitch;

            cameraZ = new(MathF.Cos(yaw) * MathF.Cos(pitch),
                          MathF.Sin(pitch),
                          MathF.Sin(yaw) * MathF.Cos(pitch));

            //cameraX.Normalize();
            cameraX = Vector3.Cross(cameraZ, worldUp);
            //cameraZ.Normalize();
            Console.WriteLine($"{yaw}    {pitch}");
        }
    }

}
