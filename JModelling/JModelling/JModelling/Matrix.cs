using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A matrix of variable size that is used in calculations. 
    /// </summary>
    public class Matrix
    {
        /// <summary>
        /// How wide this Matrix is. Represented by M.GetLength(1). 
        /// </summary>
        public int Width;

        /// <summary>
        /// How tall this Matrix is. Represented by M.GetLength(0). 
        /// </summary>
        public int Height;

        /// <summary>
        /// The matrix that's holding the data. 
        /// </summary>
        public float[,] M; 

        /// <summary>
        /// Creates a matrix with the default length and width of 4. 
        /// </summary>
        public Matrix()
            : this(4)
        { }

        /// <summary>
        /// Creates a matrix with the dimensions of [size, size].
        /// </summary>
        public Matrix(int size)
        {
            Width = size;
            Height = size; 
            M = new float[Height, Width]; 
        }
        
        /// <summary>
        /// Returns the result of two matricies being multiplied together. 
        /// </summary>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix matrix = new Matrix(); 
            for(int r = 0; r < matrix.Height; r++)
            {
                for(int c = 0; c < matrix.Width; c++)
                {
                    matrix.M[r, c] =
                        m1.M[r, 0] * m2.M[0, c] +
                        m1.M[r, 1] * m2.M[1, c] +
                        m1.M[r, 2] * m2.M[2, c] +
                        m1.M[r, 3] * m2.M[3, c];
                }
            }
            return matrix; 
        }

        /// <summary>
        /// Creates a matrix with [0,0] [1,1] [2,2] and [3,3] equal to 1. 
        /// </summary>
        public static Matrix MakeIdentity()
        {
            Matrix mat = new Matrix();
            mat.M[0, 0] = 1;
            mat.M[1, 1] = 1;
            mat.M[2, 2] = 1;
            mat.M[3, 3] = 1;
            return mat; 
        }

        public static Vec4 operator *(Matrix mat, Vec4 vec)
        {
            return new Vec4(
                vec.X * mat.M[0, 0] + vec.Y * mat.M[1, 0] + vec.Z * mat.M[2, 0] + vec.W * mat.M[3, 0],
                vec.X * mat.M[0, 1] + vec.Y * mat.M[1, 1] + vec.Z * mat.M[2, 1] + vec.W * mat.M[3, 1],
                vec.X * mat.M[0, 2] + vec.Y * mat.M[1, 2] + vec.Z * mat.M[2, 2] + vec.W * mat.M[3, 2],
                vec.X * mat.M[0, 3] + vec.Y * mat.M[1, 3] + vec.Z * mat.M[2, 3] + vec.W * mat.M[3, 3]); 
        }

        public static Vec4 operator *(Vec4 vec, Matrix mat)
        {
            //Console.WriteLine("Matrix: {0} * {1} + {2} * {3} + {4} * {5} + {6} * {7}", vec.X, mat.M[0, 0], vec.Y, mat.M[1, 0], vec.Z, mat.M[2, 0], vec.W, mat.M[3, 0]);

            return new Vec4(
                vec.X * mat.M[0, 0] + vec.Y * mat.M[1, 0] + vec.Z * mat.M[2, 0] + vec.W * mat.M[3, 0],
                vec.X * mat.M[0, 1] + vec.Y * mat.M[1, 1] + vec.Z * mat.M[2, 1] + vec.W * mat.M[3, 1],
                vec.X * mat.M[0, 2] + vec.Y * mat.M[1, 2] + vec.Z * mat.M[2, 2] + vec.W * mat.M[3, 2],
                vec.X * mat.M[0, 3] + vec.Y * mat.M[1, 3] + vec.Z * mat.M[2, 3] + vec.W * mat.M[3, 3]);
        }

        /// <summary>
        /// Given the radian of the x-rotation, create a matrix that
        /// represents it. 
        /// </summary>
        public static Matrix MakeRotationX(float rads)
        {
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = 1;
            matrix.M[1, 1] = (float)Math.Cos(rads);
            matrix.M[1, 2] = (float)Math.Sin(rads);
            matrix.M[2, 1] = (float)-Math.Sin(rads);
            matrix.M[2, 2] = (float)Math.Cos(rads);
            matrix.M[3, 3] = 1;
            return matrix; 
        }

        /// <summary>
        /// Given the radian of the y-rotation, create a matrix that
        /// represents it. 
        /// </summary>
        public static Matrix MakeRotationY(float rads)
        {
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = (float)Math.Cos(rads);
            matrix.M[0, 2] = (float)Math.Sin(rads);
            matrix.M[2, 0] = (float)-Math.Sin(rads);
            matrix.M[1, 1] = 1;
            matrix.M[2, 2] = (float)Math.Cos(rads);
            matrix.M[3, 3] = 1;
            return matrix; 
        }

        /// <summary>
        /// Given the radian of the z-rotation, create a matrix that 
        /// represents it. 
        /// </summary>
        public static Matrix MakeRotationZ(float rads)
        {
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = (float)Math.Cos(rads);
            matrix.M[0, 1] = (float)Math.Sin(rads);
            matrix.M[1, 0] = (float)-Math.Sin(rads);
            matrix.M[1, 1] = (float)Math.Cos(rads);
            matrix.M[2, 2] = 1;
            matrix.M[3, 3] = 1;
            return matrix; 
        }

        /// <summary>
        /// Makes a matrix that represents a complete rotation, given
        /// the yaw, pitch, and roll. 
        /// </summary>
        public static Matrix MakeRotationMatrix(float yaw, float pitch, float roll)
        {
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = (float)(Math.Cos(yaw) * Math.Cos(pitch));
            matrix.M[0, 1] = (float)(-Math.Cos(roll) * Math.Sin(pitch) + Math.Sin(roll) * Math.Sin(yaw) * Math.Cos(pitch));
            matrix.M[0, 2] = (float)(Math.Sin(roll) * Math.Sin(pitch) + Math.Cos(roll) * Math.Sin(yaw) * Math.Cos(pitch));
            matrix.M[1, 0] = (float)(Math.Cos(yaw) * Math.Sin(pitch));
            matrix.M[1, 1] = (float)(Math.Cos(roll) * Math.Cos(pitch) + Math.Sin(roll) * Math.Sin(yaw) * Math.Sin(pitch));
            matrix.M[1, 2] = (float)(-Math.Sin(roll) * Math.Cos(pitch) + Math.Cos(roll) * Math.Sin(yaw) * Math.Sin(pitch));
            matrix.M[2, 0] = (float)(-Math.Sin(yaw));
            matrix.M[2, 1] = (float)(Math.Sin(roll) * Math.Cos(yaw));
            matrix.M[2, 2] = (float)(Math.Cos(roll) * Math.Cos(yaw));
            matrix.M[3, 3] = 1;
       
            return matrix;
        }

        /// <summary>
        /// Makes a matrix that represents a translation from the origin. 
        /// </summary>
        public static Matrix MakeTranslation(float x, float y, float z)
        {
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = 1;
            matrix.M[1, 1] = 1;
            matrix.M[2, 2] = 1;
            matrix.M[3, 3] = 1;
            matrix.M[3, 0] = x;
            matrix.M[3, 1] = y;
            matrix.M[3, 2] = z;
            return matrix; 
        }

        public static Matrix MakeProjection(float fov, float aspectRatio, float near, float far)
        {
            float fovRad = 1f / (float)Math.Tan(fov * 0.5f);
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = aspectRatio * fovRad;
            matrix.M[1, 1] = fovRad;
            matrix.M[2, 2] = far / (far - near);
            matrix.M[3, 2] = (-far * near) / (far - near);
            matrix.M[2, 3] = 1;
            return matrix; 
        }

        public static Matrix PointAt(Vec4 pos, Vec4 target, Vec4 up)
        {
            // Calculate new forward direction.
            Vec4 newForward = target - pos;
            newForward.Normalize();

            // Calculate new Up direction. 
            Vec4 a = newForward * Vec4.DotProduct(up, newForward);
            Vec4 newUp = up - a;
            newUp.Normalize();

            // New right direction is cross product. 
            Vec4 newRight = Vec4.CrossProduct(newUp, newForward);

            // Construct dimensioning and translation matrix. 
            Matrix matrix = new Matrix();
            matrix.M[0, 0] = newRight.X;    matrix.M[0, 1] = newRight.Y;    matrix.M[0, 2] = newRight.Z;        matrix.M[0, 3] = 0;
            matrix.M[1, 0] = newUp.X;       matrix.M[1, 1] = newUp.Y;       matrix.M[1, 2] = newUp.Z;           matrix.M[1, 3] = 0;
            matrix.M[2, 0] = newForward.X;  matrix.M[2, 1] = newForward.Y;  matrix.M[2, 2] = newForward.Z;      matrix.M[2, 3] = 0;
            matrix.M[3, 0] = pos.X;         matrix.M[3, 1] = pos.Y;         matrix.M[3, 2] = pos.Z;             matrix.M[3, 3] = 1;
            return matrix;
        }

        /// <summary>
        /// Inverses the Rotation/Translation matricies. This will not
        /// work for other types of matricies. 
        /// </summary>
        public void QuickInverse()
        {
            float[,] matrix = new float[M.GetLength(0), M.GetLength(1)];
            matrix[0, 0] = M[0, 0]; matrix[0, 1] = M[1, 0]; matrix[0, 2] = M[2, 0]; matrix[0, 3] = 0;
            matrix[1, 0] = M[0, 1]; matrix[1, 1] = M[1, 1]; matrix[1, 2] = M[2, 1]; matrix[1, 3] = 0;
            matrix[2, 0] = M[0, 2]; matrix[2, 1] = M[1, 2]; matrix[2, 2] = M[2, 2]; matrix[2, 3] = 0;
            matrix[3, 0] = -(M[3, 0] * matrix[0, 0] + M[3, 1] * matrix[1, 0] + M[3, 2] * matrix[2, 0]);
            matrix[3, 1] = -(M[3, 0] * matrix[0, 1] + M[3, 1] * matrix[1, 1] + M[3, 2] * matrix[2, 1]);
            matrix[3, 2] = -(M[3, 0] * matrix[0, 2] + M[3, 1] * matrix[1, 2] + M[3, 2] * matrix[2, 2]);
            matrix[3, 3] = 1;

            M = matrix; 
        }
    }
}
