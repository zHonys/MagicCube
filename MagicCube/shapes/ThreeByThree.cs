using MagicCube.controls;
using OpenTK.Mathematics;
using OpenTK.Platform.Windows;
using System.Collections;
using System.Runtime.InteropServices;

namespace MagicCube.shapes
{
    public class ThreeByThree : IDisposable
    {
        Model model;
        Mesh[,,] sides = new Mesh[3, 3, 3];

        private float _rotationTime;

        public Matrix4 modelMatrix
        {
            get => model.modelMatrix;
            set => model.modelMatrix = value;
        }
        public ThreeByThree(string modelPath, float rotationTime=0.5f, RotationFunctionsType rotationType=RotationFunctionsType.Sin)
        {
            model = new(modelPath);
            model.Meshes.ForEach(mesh => {
                int[] indice = Array.ConvertAll(mesh.Name.Substring(mesh.Name.IndexOf("_") + 1).Split(','), s => int.Parse(s));
                sides.SetValue(mesh, indice);
            });

            _rotationTime = rotationTime;
            SetAnimationFunction(rotationType);
        }
        public void Update(float elapsed)
        {
            runAnimations(elapsed);
        }
        public void Draw(Shader shader)
        {
            model.Draw(shader);
        }

        #region Movements

        public void U(bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    querryAnimation(sides[i, j, 0], Vector3.UnitY, angle);
                    sides[i, j, 0] = copy[MathHelper.Abs(rot - j), MathHelper.Abs(2 - rot - i), 0];
                }
            }
        }
        public void D(bool reverse = false)
        {
            int angle = reverse ? -90 : 90;
            int rot = reverse ? 0 : 2;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    querryAnimation(sides[i, j, 2], Vector3.UnitY, angle);
                    sides[i, j, 2] = copy[MathHelper.Abs(rot - j), MathHelper.Abs(2 - rot - i), 2];
                }
            }
        }
        public void R(bool reverse = false)
        {
            int angle = reverse ? -90 : 90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[2, j, k], Vector3.UnitX, angle);
                    sides[2, j , k] = copy[2, MathHelper.Abs((2 - rot) - k), MathHelper.Abs(rot - j)];
                }
            }
        }
        public void L(bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 0 : 2;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[0, j, k], Vector3.UnitX, angle);
                    sides[0, j, k] = copy[0, MathHelper.Abs((2 - rot) - k), MathHelper.Abs(rot - j)];
                }
            }
        }
        public void F(bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[i, 2, k], Vector3.UnitZ, angle);
                    sides[i, 2, k] = copy[MathHelper.Abs((2 - rot) - k), 2, MathHelper.Abs(rot - i)];
                }
            }
        }
        public void B(bool reverse = false)
        {
            int angle = reverse ? -90 : 90;
            int rot = reverse ? 0 : 2;
            Mesh[,,] copy = new Mesh[3, 3, 3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    querryAnimation(sides[i, 0, k], Vector3.UnitZ, angle);
                    sides[i, 0, k] = copy[MathHelper.Abs((2 - rot) - k), 0, MathHelper.Abs(rot - i)];
                }
            }
        }

        public void Spin()
        {
            model.modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * model.modelMatrix;
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
