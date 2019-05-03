using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// Some math-related extension methods
    /// </summary>
    public class MathExtensions
    {
        /// <summary>
        /// Returns the distance between two vectors. 
        /// </summary>
        public static float Dist(Vec4 a, Vec4 b)
        {
            return (float)Math.Sqrt(
                Math.Pow(a.X - b.X, 2) +
                Math.Pow(a.Y - b.Y, 2) +
                Math.Pow(a.Z - b.Z, 2));
        }

        public static void CalcTriNormal(Triangle triangle)
        {
            Vec4 normal =
                Vec4.CrossProduct(
                    triangle.Points[1] - triangle.Points[0],
                    triangle.Points[2] - triangle.Points[0]);
            normal.Normalize();
            triangle.Normal = normal;
        }
    }
}
