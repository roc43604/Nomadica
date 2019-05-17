using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A grouping of 3 points. This could also contain a group
    /// of 3 texture coords, if applicable. 
    /// </summary>
    public class Triangle : IComparable
    {
        /// <summary>
        /// The three points this Triangle is made of. 
        /// </summary>
        public Vec4[] Points;

        /// <summary>
        /// The three texture points this Triangle is made of. 
        /// </summary>
        public Vec3[] Texels;

        /// <summary>
        /// The image that is drawn on this triangle. 
        /// </summary>
        public Color[,] Image;

        /// <summary>
        /// This triangle's surface normal. 
        /// </summary>
        public Vec4 Normal;

        /// <summary>
        /// The Length of this triangle's surface normal. 
        /// </summary>
        public float NormalLength;

        /// <summary>
        /// The angle this triangle makes. 
        /// </summary>
        public float Angle;

        /// <summary>
        /// How far away this triangle is from the camera. 
        /// </summary>
        public float ClosestDist;

        /// <summary>
        /// What color this Triangle is.
        /// </summary>
        public Color Color;

        /// <summary>
        /// How illuminated the triangle's color is. 
        /// </summary>
        public float Alpha; 
        
        /// <summary>
        /// Creates a default triangle with no assigned points.
        /// </summary>
        public Triangle()
        {
            Points = new Vec4[3];
            Texels = new Vec3[3]; 
            Color = Color.Red; 
        } 

        /// <summary>
        /// Creates a triangle given you know all three points. 
        /// Assigns a default texture coord set. 
        /// </summary>
        public Triangle(Vec4 a, Vec4 b, Vec4 c)
            : this(new Vec4[] { a, b, c })
        { }

        /// <summary>
        /// Creates a trianle given you know all three points and
        /// texel locations. 
        /// </summary>
        public Triangle(Vec4 a, Vec4 b, Vec4 c, Vec3 d, Vec3 e, Vec3 f)
            : this(a, b, c, d, e, f, Color.Red, null)
        { }

        /// <summary>
        /// Creates a triangle given you know all three points, 
        /// all three texel locations, and the color. 
        /// </summary>
        public Triangle(Vec4 a, Vec4 b, Vec4 c, Vec3 d, Vec3 e, Vec3 f, Color color, Color[,] image)
            : this(new Vec4[] { a, b, c }, new Vec3[] { d, e, f }, color, image)
        { }

        /// <summary>
        /// Creates a triangle given you know all three points. 
        /// Assigns a default texture coord set. 
        /// </summary>
        public Triangle(Vec4[] points)
            : this(points, new Vec3[] { new Vec3(), new Vec3(), new Vec3() }, Color.Red, null)
        { }

        /// <summary>
        /// Creates a triangle given you know all three points, 
        /// as well as all three texture coords. 
        /// </summary>
        public Triangle(Vec4[] points, Vec3[] texels, Color color, Color[,] image)
            : this(points, texels, color, image, Vec4.Zero, 0)
        { }

        public Triangle(Vec4[] points, Vec3[] texels, Color color, Color[,] image, Vec4 normal, float normalLength)
        {
            Points = points;
            Texels = texels;
            Color = color;
            Image = image;
            Normal = normal;
            NormalLength = normalLength; 
        }

        /// <summary>
        /// Creates a copy of this triangle.
        /// </summary>
        public Triangle Clone()
        {
            return new Triangle(
                new Vec4[] 
                {
                    Points[0].Clone(),
                    Points[1].Clone(),
                    Points[2].Clone()
                },
                new Vec3[]
                {
                    Texels[0].Clone(),
                    Texels[1].Clone(),
                    Texels[2].Clone()
                },                
                Color,
                Image, 
                Normal, 
                NormalLength);
        }

        public override bool Equals(object obj)
        {
            return this == (Triangle)obj; 
        }

        public override int GetHashCode()
        {
            return (int)(Points[0].X + Points[1].Y + Points[2].Z); 
        }

        public static bool operator ==(Triangle left, Triangle right)
        {
            return (
                left.Points[0] == right.Points[0] &&
                left.Points[1] == right.Points[1] &&
                left.Points[2] == right.Points[2]);
        }

        public static bool operator !=(Triangle left, Triangle right)
        {
            return !(left == right); 
        }

        /// <summary>
        /// Multiplies the points of this triangle against the
        /// matrix. 
        /// </summary>
        public void TimesEquals(Matrix mat)
        {
            Points[0].TimesEquals(mat);
            Points[1].TimesEquals(mat);
            Points[2].TimesEquals(mat);
        }

        /// <summary>
        /// Divides the texel index by the specified value. 
        /// </summary>
        public void DivideTexel(int index, float val)
        {
            Texels[index].U /= val;
            Texels[index].V /= val;
            Texels[index].W /= val;
        }

        /// <summary>
        /// Returns a printable version of this triangle. It is 
        /// represented by each of the Vec4's that make it up. 
        /// </summary>
        public override string ToString()
        {
            string str = "[";
            foreach(Vec4 point in Points)
            {
                str += point + ", ";
            }
            return str + "]"; 
        }

        /// <returns>Returns a value dictating how similar two triangles are.
        /// Will organize triangles in the order closest to furthest away. Will 
        /// throw an exception if you try to compare this and a non-triangle. 
        /// </returns>
        public int CompareTo(object obj)
        {
            Triangle other = (Triangle)obj;

            float z1 = (Points[0].Z + Points[1].Z + Points[2].Z) / 3f;
            float z2 = (other.Points[0].Z + other.Points[1].Z + other.Points[2].Z) / 3f;

            return -z1.CompareTo(z2); 
        }
    }
}
