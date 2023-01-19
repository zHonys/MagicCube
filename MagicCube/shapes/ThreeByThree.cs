using MagicCube.controls;
using MagicCube.Interfaces;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace MagicCube.shapes
{
    public class ThreeByThree : IDisposable, IUpdatable, IDrawable
    {
        private Model model;
        private Mesh[,,] sides = new Mesh[3, 3, 3];

        private KeyboardState _keyboardState;
        private float _rotationTime;

        public Matrix4 modelMatrix
        {
            get => model.modelMatrix;
            set => model.modelMatrix = value;
        }
        public ThreeByThree(string modelPath, KeyboardState keyboardState, float rotationTime=0.5f, RotationFunctionsType rotationType=RotationFunctionsType.Sin)
        {
            model = new(modelPath);
            model.Meshes.ForEach(mesh => {
                int[] indice = Array.ConvertAll(mesh.Name.Substring(mesh.Name.IndexOf("_") + 1).Split(','), s => int.Parse(s));
                sides.SetValue(mesh, indice);
            });
            _keyboardState = keyboardState;

            _rotationTime = rotationTime;
            SetAnimationFunction(rotationType);
        }
        public void Update(float elapsed)
        {
            ProcessInput();
            runAnimations(elapsed);
        }
        public void Draw(Shader shader)
        {
            model.Draw(shader);
        }

        #region Input manager

        private void ProcessInput()
        {
            bool reverse = false;
            if (_keyboardState.IsKeyDown(Keys.LeftShift)) reverse = true;

            if (_keyboardState.IsKeyPressed(Keys.R)) TurnAxisX(0, reverse);
            if (_keyboardState.IsKeyPressed(Keys.L)) TurnAxisX(2, !reverse);

            if (_keyboardState.IsKeyPressed(Keys.U)) TurnAxisY(0,  reverse);
            if (_keyboardState.IsKeyPressed(Keys.C)) TurnAxisY(2, !reverse);

            if (_keyboardState.IsKeyPressed(Keys.F)) TurnAxisZ(0, reverse);
            if (_keyboardState.IsKeyPressed(Keys.B)) TurnAxisZ(2, !reverse);

            if (_keyboardState.IsKeyPressed(Keys.X)) Spin(Vector3.UnitX, reverse);
            if (_keyboardState.IsKeyPressed(Keys.Z)) Spin(Vector3.UnitZ, reverse);
            if (_keyboardState.IsKeyPressed(Keys.Y)) Spin(Vector3.UnitY, reverse);
        }

        #endregion

        #region Movements

        public void TurnAxisY(int layer, bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    querryAnimation(sides[i, j, layer], Vector3.UnitY, angle);
                    sides[i, j, layer] = copy[MathHelper.Abs(rot - j), MathHelper.Abs(2 - rot - i), layer];
                }
            }
        }
        public void TurnAxisX(int column, bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 0 : 2;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[column, j, k], Vector3.UnitX, angle);
                    sides[column, j, k] = copy[column, MathHelper.Abs((2 - rot) - k), MathHelper.Abs(rot - j)];
                }
            }
        }
        public void TurnAxisZ(int row, bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[i, row, k], Vector3.UnitZ, angle);
                    sides[i, row, k] = copy[MathHelper.Abs((2 - rot) - k), row, MathHelper.Abs(rot - i)];
                }
            }
        }
        public void Spin(Vector3 axis, bool reverse)
        {
            Action<int, bool> turn = (foo, bar) => { };
            if (axis == Vector3.UnitX) turn = TurnAxisX;
            if (axis == Vector3.UnitY) turn = TurnAxisY;
            if (axis == Vector3.UnitZ) turn = TurnAxisZ;
            for (int i = 0; i < 3; i++)
            {
                turn(i, reverse);
            }
        }

        #endregion

        #region animation

        private delegate Quaternion animationFunction(RotationInfo data, float totalTime);
        private animationFunction _animationFuncton;
        private class RotationInfo
        {
            public Mesh mesh;
            public Quaternion invLastRotation;
            public Quaternion finalRotation;
            public float time;
        }
        private List<RotationInfo> rotations = new();

        public void SetAnimationFunction(RotationFunctionsType func)
        {
            switch (func){
                case RotationFunctionsType.Sin:
                    _animationFuncton = (RotationInfo data, float totalTime) =>
                    {
                        return Quaternion.Slerp(Quaternion.Identity, data.finalRotation, MathF.Sin(MathF.PI / (2 * _rotationTime) * totalTime));
                    };
                    break;
                case RotationFunctionsType.Exponential:
                    _animationFuncton = (RotationInfo data, float totalTime) =>
                    {
                        return Quaternion.Slerp(Quaternion.Identity, data.finalRotation, MathF.Pow(totalTime / _rotationTime, 2));
                    };
                    break;
                case RotationFunctionsType.Linear:
                    _animationFuncton = (RotationInfo data, float totalTime) =>
                    {
                        return Quaternion.Slerp(Quaternion.Identity, data.finalRotation, totalTime/_rotationTime);
                    };
                    break;
            }
        }
        private void querryAnimation(Mesh mesh, Vector3 axis, float angle)
        {
            if(rotations.Any(x => x.mesh == mesh))
            {
                RotationInfo data = rotations.Find(x => x.mesh == mesh);
                data.mesh.modelMatrix *= Matrix4.CreateFromQuaternion(data.invLastRotation);
                data.mesh.modelMatrix *= Matrix4.CreateFromQuaternion(data.finalRotation);
                rotations.Remove(data);
            }

            rotations.Add(new RotationInfo
            {
                mesh = mesh,
                invLastRotation = Quaternion.Identity.Inverted(),
                finalRotation = Quaternion.FromAxisAngle(axis, MathHelper.DegreesToRadians(angle)),
                time = 0
            });
        }
        private void animate(RotationInfo data, float elapsed)
        {
            float totalTime = data.time + elapsed >= _rotationTime ? _rotationTime : data.time + elapsed;
            Quaternion newRotation = _animationFuncton(data, totalTime);

            data.mesh.modelMatrix *= Matrix4.CreateFromQuaternion(data.invLastRotation * newRotation);
            data.invLastRotation = newRotation.Inverted();
            data.time = totalTime;
        }
        private void runAnimations(float elapsed)
        {
            rotations.ForEach(item => animate(item, elapsed));
            rotations = rotations.Where(item => item.time < _rotationTime).ToList();
        }

        #endregion

        #region Dispose

        ~ThreeByThree()
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
            model.Dispose();
            if (disposing)
            {
                Array.Clear(sides);
            }
        }

        #endregion
    }
    public enum RotationFunctionsType
    {
        Sin = 0,
        Exponential = 1,
        Linear = 2
    }
}
