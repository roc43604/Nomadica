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
        {
            Points = points;
            Texels = texels;
            Color = color; 
            Image = image;  
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
