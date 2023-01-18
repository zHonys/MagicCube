using MagicCube.controls;
using OpenTK.Mathematics;

namespace MagicCube.shapes
{
    public class ThreeByThree : IDisposable
    {
        Model model;
        Mesh[,,] sides = new Mesh[3,3,3];

        public Matrix4 modelMatrix
        {
            get
            {
                return model.modelMatrix;
            }
            set
            {
                model.modelMatrix = value;
            }
        }
        public ThreeByThree(string modelPath)
        {
            model = new(modelPath);
            setUpSdtes();
        }
        private void setUpSdtes()
        {
            foreach(Mesh mesh in model.Meshes)
            {
                int[] indice = Array.ConvertAll(mesh.Name.Substring(mesh.Name.IndexOf("_")+1).Split(','), s=> int.Parse(s));
                sides.SetValue(mesh, indice);
            }
        }
        public void Draw(Shader shader)
        {
            model.Draw(shader);
        }
        public void U(bool reverse = false)
        {
            int angle = reverse ? 90 : -90;
            int rot = reverse ? 2 : 0;
            Mesh[,,] copy = new Mesh[3,3,3];
            Array.Copy(sides, copy, 27);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    sides[i, j, 0].modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle));
                    sides[i, j, 0] = copy[MathHelper.Abs(rot - j), MathHelper.Abs(2 - rot - i), 0];
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
                    sides[2, j , k].modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle));
                    sides[2, j , k] = copy[2, MathHelper.Abs((2 - rot) - k), MathHelper.Abs(rot - j)];
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
                    sides[i, j, 2].modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(angle));
                    sides[i, j, 2] = copy[MathHelper.Abs(rot - j), MathHelper.Abs(2 - rot - i), 2];
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
                    sides[0, j, k].modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(angle));
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
                    sides[i, 2, k].modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle));
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
                    sides[i, 0, k].modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angle));
                    sides[i, 0, k] = copy[MathHelper.Abs((2 - rot) - k), 0, MathHelper.Abs(rot - i)];
                }
            }
        }

        public void rotate()
        {
            model.modelMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * model.modelMatrix;
        }
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
}
