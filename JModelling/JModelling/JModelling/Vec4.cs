using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// An object that contains an X, Y, Z, and W component. Although
    /// the Microsoft.Xna library already contains a Vector4 object, we
    /// don't use it in order to create our own operator extension 
    /// methods (which C# doesn't allow for through an extension class). 
    /// 
    /// NOTE: Not many methods will actually involve the W component; this
    /// value is only used in extremely-specific scenarios. Most of the
    /// time, this value will be kept blank with a value of 1. 
    /// </summary>
    public class Vec4
    {
        /// <summary>
        /// A blank Vec4. 
        /// </summary>
        public static readonly Vec4 Zero = new Vec4(0, 0, 0, 1); 

        /// <summary>
        /// One of the four numbers used to create a Vec4.
        /// </summary>
        public float X, Y, Z, W; 

        /// <summary>
        /// Creates a default Vec4 object with 0 in all boxes. 
        /// </summary>
        public Vec4() 
            : this(0, 0, 0) { }

        /// <summary>
        /// Creates a Vec4 object given the coords of the X, Y,
        /// and Z values. The W value is set to 1. 
        /// </summary>
        public Vec4(float x, float y, float z) 
            : this(x, y, z, 1) { }

        /// <summary>
        /// Creates a Vec4 object given the coords of each value. 
        /// </summary>
        public Vec4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w; 
        }

        /// <summary>
        /// Creates a copy of this Vec4
        /// </summary>
        public Vec4 Clone()
        {
            return new Vec4(X, Y, Z, W);
        }
        
        /// <returns>Whether or not the X, Y, and Z coords are equivilant.</returns>
        public static bool operator ==(Vec4 left, Vec4 right)
        {
            return (left.X == right.X &&
                    left.Y == right.Y &&
                    left.Z == right.Z); 
        }

        /// <returns>Whether or not the X, Y, and Z coord don' equal eachother.</returns>
        public static bool operator !=(Vec4 left, Vec4 right)
        {
            return !(left == right); 
        }

        /// <returns>Whether or not the X, Y, and Z coors are equivilant. Will throw an 
        /// error if you check if a Vec4 and a non-Vec4 are equivilant. </returns> 
        public override bool Equals(Object other)
        {
            return this == (Vec4)other;
        }
        
        /// <returns>A unique integer representing this vec4. Use this method sparingly,
        /// because it will take up A LOT of time/resources to complete.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() / 10000; 
            //return ((short)X * (short)Y * (short)Z); 
        }

        /// <returns>A new Vec4 object, which is a combined value of adding the X, Y, and
        /// Z coords of each parameter. The W value is not affected. </returns>
        public static Vec4 operator +(Vec4 left, Vec4 right)
        {
            return new Vec4(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z); 
        }

        /// <summary>
        /// Is the same as doing one += two, in the sense that one.PlusEquals(two). This
        /// doesn't create a new Vec4 object like the + operator above does. 
        /// </summary>
        public void PlusEquals(Vec4 other)
        {
            X += other.X;
            Y += other.Y;
            Z += other.Z; 
        }

        /// <returns>A new Vec4 object, which is a combined value of subtracting the X, Y,
        /// and Z coords of each parameter. The W value is not affected. </returns>
        public static Vec4 operator -(Vec4 left, Vec4 right)
        {
            return new Vec4(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z); 
        }

        /// <summary>
        /// Is the same as doing one -= two, in the sense that one.MinusEquals(two). This
        /// doesn't create a new Vec4 object like the - operator above does. 
        /// </summary>
        public void MinusEquals(Vec4 other)
        {
            X -= other.X;
            Y -= other.Y;
            Z -= other.Z; 
        }

        /// <returns>A new Vec4 object, which is a combined value of multiplying the X, Y, 
        /// and Z coords of the Vec4 against the float val. The W value is not affected. </returns>
        public static Vec4 operator *(Vec4 vec, float val)
        {
            return new Vec4(
                vec.X * val,
                vec.Y * val,
                vec.Z * val); 
        }

        /// <summary>
        /// Is the same as doing one *= val, in the sense that one.TimesEquals(val). This
        /// doesn't create a new Vec4 object like the * operator above does. 
        /// </summary>
        public void TimesEquals(float val)
        {
            X *= val;
            Y *= val;
            Z *= val;
        }

        /// <summary>
        /// Multiplies this against the specified matrix. 
        /// </summary>
        public void TimesEquals(Matrix mat)
        {
            //Console.WriteLine("Vec4: {0} * {1} + {2} * {3} + {4} * {5} + {6} * {7}", X, mat.M[0, 0], Y, mat.M[1, 0], Z, mat.M[2, 0], W, mat.M[3, 0]); 



            float newX = X * mat.M[0, 0] + Y * mat.M[1, 0] + Z * mat.M[2, 0] + W * mat.M[3, 0];
            float newY = X * mat.M[0, 1] + Y * mat.M[1, 1] + Z * mat.M[2, 1] + W * mat.M[3, 1];
            float newZ = X * mat.M[0, 2] + Y * mat.M[1, 2] + Z * mat.M[2, 2] + W * mat.M[3, 2];
            float newW = X * mat.M[0, 3] + Y * mat.M[1, 3] + Z * mat.M[2, 3] + W * mat.M[3, 3];

            X = newX;
            Y = newY;
            Z = newZ;
            W = newW;
        }

        /// <returns>A new Vec4 object, which is a combined value of dividing the X, Y, 
        /// and Z coords of the Vec4 against the float val. The W value is not affected. </returns>
        public static Vec4 operator /(Vec4 vec, float val)
        {
            return new Vec4(
                vec.X / val,
                vec.Y / val,
                vec.Z / val); 
        }

        /// <summary>
        /// Is the same as doing one /= val, in the sense that one.DivideEquals(val). This
        /// doesn't create a new Vec4 object like the / operator above does. 
        /// </summary>
        public void DivideEquals(float val)
        {
            X /= val;
            Y /= val;
            Z /= val; 
        }

        /// <summary>
        /// Averages all of the items in the Vec4 list together into
        /// a new Vec4. 
        /// </summary>
        /// <param name="vecs">The list of Vec4's you want to average
        /// together. </param>
        /// <returns>An averaged list of Vec4's</returns>
        public static Vec4 Average(Vec4[] vecs)
        {
            float x = 0, y = 0, z = 0, w = 0; 
            foreach(Vec4 vec in vecs)
            {
                x += vec.X;
                y += vec.Y;
                z += vec.Z;
                w += vec.W; 
            }

            return new Vec4(x / vecs.Length, y / vecs.Length, z / vecs.Length, w / vecs.Length); 
        }

        /// <returns>The dot product of two Vec4's. </returns>
        public static float DotProduct(Vec4 v1, Vec4 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; 
        }

        /// <returns>The length of this vector </returns>
        public float Length()
        {
            return (float)Math.Sqrt(DotProduct(this, this)); 
        }

        /// <summary>
        /// Normalizes this vector, making all values range between 
        /// -1 and 1.  
        /// </summary>
        public void Normalize()
        {
            float l = Length();
            X /= l;
            Y /= l;
            Z /= l; 
        }
        
        /// <returns>The cross-product between the two vectors. </returns>
        public static Vec4 CrossProduct(Vec4 left, Vec4 right)
        {
            return new Vec4(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X); 
        }
        
        public static Triangle[] TriangleClipAgainstPlane(Vec4 planeP, Vec4 planeN, Triangle tri)
        {
            // Make sure plane normal is actually normal
            planeN.Normalize();

            Vec4[] insidePoints = new Vec4[3];  int nInsidePointCount = 0;
            Vec4[] outsidePoints = new Vec4[3]; int nOutsidePointCount = 0;
            Vec3[] insideTexels = new Vec3[3];  int nInsideTexelCount = 0;
            Vec3[] outsideTexels = new Vec3[3]; int nOutsideTexelCount = 0; 

            // Get signed distance of each point in triangle to plane. 
            float dotProduct = DotProduct(planeN, planeP);
            float d0 = Dist(planeN, tri.Points[0], dotProduct);
            float d1 = Dist(planeN, tri.Points[1], dotProduct);
            float d2 = Dist(planeN, tri.Points[2], dotProduct); 

            if(d0 >= 0)
            {
                insidePoints[nInsidePointCount++] = tri.Points[0];
                insideTexels[nInsideTexelCount++] = tri.Texels[0]; 
            }
            else
            {
                outsidePoints[nOutsidePointCount++] = tri.Points[0];
                outsideTexels[nOutsideTexelCount++] = tri.Texels[0]; 
            }
            if (d1 >= 0)
            {
                insidePoints[nInsidePointCount++] = tri.Points[1];
                insideTexels[nInsideTexelCount++] = tri.Texels[1];
            }
            else
            {
                outsidePoints[nOutsidePointCount++] = tri.Points[1];
                outsideTexels[nOutsideTexelCount++] = tri.Texels[1];
            }
            if (d2 >= 0)
            {
                insidePoints[nInsidePointCount++] = tri.Points[2];
                insideTexels[nInsideTexelCount++] = tri.Texels[2];
            }
            else
            {
                outsidePoints[nOutsidePointCount++] = tri.Points[2];
                outsideTexels[nOutsideTexelCount++] = tri.Texels[2];
            }

            if(nInsidePointCount == 0)
            {
                return new Triangle[] { };  // No triangles returned are valid. 
            }
            else if(nInsidePointCount == 3)
            {
                return new Triangle[] { tri };  // The entire returned triangle is valid. 
            }
            else if(nInsidePointCount == 1)
            {
                Triangle newTri = new Triangle();

                newTri.Alpha = tri.Alpha;
                newTri.ClosestDist = tri.ClosestDist;

                newTri.Image = tri.Image; 

                newTri.Color = tri.Color;                

                // The inside point is valid, so...
                newTri.Points[0] = insidePoints[0].Clone();
                newTri.Texels[0] = insideTexels[0].Clone(); 

                // but the two new points are at locations where the original sides
                // of the triangle (lines) intersect with the plane. 
                IntersectPlaneContainer c = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);
                newTri.Points[1] = c.V;

                newTri.Texels[1] = new Vec3(
                    c.T * (outsideTexels[0].U - insideTexels[0].U) + insideTexels[0].U,
                    c.T * (outsideTexels[0].V - insideTexels[0].V) + insideTexels[0].V,
                    c.T * (outsideTexels[0].W - insideTexels[0].W) + insideTexels[0].W); 

                c = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[1]);
                newTri.Points[2] = c.V;
                newTri.Texels[2] = new Vec3(
                    c.T * (outsideTexels[1].U - insideTexels[0].U) + insideTexels[0].U,
                    c.T * (outsideTexels[1].V - insideTexels[0].V) + insideTexels[0].V,
                    c.T * (outsideTexels[1].W - insideTexels[0].W) + insideTexels[0].W);

                return new Triangle[] { newTri }; 
            }
            else
            {
                Triangle tri1 = new Triangle();
                Triangle tri2 = new Triangle();

                tri1.ClosestDist = tri.ClosestDist;
                tri2.ClosestDist = tri.ClosestDist;

                tri1.Alpha = tri.Alpha;
                tri2.Alpha = tri.Alpha;

                tri1.Image = tri.Image;
                tri2.Image = tri.Image; 

                tri1.Color = tri.Color;
                tri2.Color = tri.Color;

                // The first triangle consists of the two inside points and a new point
                // determined by the location where one side of the triangle intersects
                // with the plane. 
                tri1.Points[0] = insidePoints[0].Clone();
                tri1.Points[1] = insidePoints[1].Clone();
                tri1.Texels[0] = insideTexels[0].Clone();
                tri1.Texels[1] = insideTexels[1].Clone(); 

                IntersectPlaneContainer c = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);
                tri1.Points[2] = c.V;
                tri1.Texels[2] = new Vec3(
                    c.T * (outsideTexels[0].U - insideTexels[0].U) + insideTexels[0].U,
                    c.T * (outsideTexels[0].V - insideTexels[0].V) + insideTexels[0].V,
                    c.T * (outsideTexels[0].W - insideTexels[0].W) + insideTexels[0].W);

                // The second triangle is composed of one of the inside points, a new point
                // determined by the intersection of the other side of the triangle and the
                // plane, and the newly created point above. 
                tri2.Points[0] = insidePoints[1].Clone();
                tri2.Points[1] = tri1.Points[2].Clone();
                tri2.Texels[0] = insideTexels[1];
                tri2.Texels[1] = tri1.Texels[2]; 

                c = IntersectPlane(planeP, planeN, insidePoints[1], outsidePoints[0]);
                tri2.Points[2] = c.V;
                tri2.Texels[2] = new Vec3(
                    c.T * (outsideTexels[0].U - insideTexels[1].U) + insideTexels[1].U,
                    c.T * (outsideTexels[0].V - insideTexels[1].V) + insideTexels[1].V,
                    c.T * (outsideTexels[0].W - insideTexels[1].W) + insideTexels[1].W);

                return new Triangle[] { tri1, tri2 };
            }
        }

        public static bool VecClipAgainstPlane(Vec4 planeP, Vec4 planeN, Vec4 vec)
        {
            // Make sure plane normal is actually normal
            planeN.Normalize();

            // Get signed distance of each point in triangle to plane. 
            float dotProduct = DotProduct(planeN, planeP);
            float d = Dist(planeN, vec, dotProduct);

            if (d >= 0)
            {
                return true; 
            }
            else
            {
                return false; 
            }
        }

        private static float Dist(Vec4 planeN, Vec4 p, float dotProduct)
        {
            return (planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - dotProduct); 
        }

        private static IntersectPlaneContainer IntersectPlane(Vec4 planeP, Vec4 planeN, Vec4 lineStart, Vec4 lineEnd)
        {
            planeN.Normalize();
            float planeD = -DotProduct(planeN, planeP);
            float ad = DotProduct(lineStart, planeN);
            float bd = DotProduct(lineEnd, planeN);
            float t = (-planeD - ad) / (bd - ad);
            Vec4 lineStartToEnd = lineEnd - lineStart;
            Vec4 lineToIntersect = lineStartToEnd * t;
            return new IntersectPlaneContainer(lineStart + lineToIntersect, t); 
        }

        /// <summary>
        /// A wrapper class for a vector and a value, that the intersectPlane
        /// method will return. 
        /// </summary>
        internal class IntersectPlaneContainer
        {
            internal Vec4 V;
            internal float T;
            
            internal IntersectPlaneContainer(Vec4 V, float T)
            {
                this.V = V;
                this.T = T; 
            }  
        }

        /// <returns>Returns a printable version of this Vec4, represented by 
        /// "(X, Y, Z, W)". </returns>
        public override string ToString()
        {
            return "(" + X + ", " + Y + ", " + Z + ", " + W + ")"; 
        }
    }
}
