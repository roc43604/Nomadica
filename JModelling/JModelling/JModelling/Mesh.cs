using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A collection of Vectors and Triangles connecting these
    /// vectors to form a model. 
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// A list of all of the points in this mesh. 
        /// </summary>
        private Vec4[] Points;

        /// <summary>
        /// Position of this mesh
        /// </summary>
        private Vec4 Position;

        /// <summary>
        /// Size of this mesh
        /// </summary>
        public Vec4 Size;

        /// <summary>
        /// A box defined around this model that denotes the
        /// area the player can hit. 
        /// </summary>
        public BoundingBox bounds; 

        /// <summary>
        /// A list of all of the triangles that connect the 
        /// points together. 
        /// </summary>
        public Triangle[] Triangles;


        /// <summary>
        /// Creates a mesh given the triangles that make it up.
        /// </summary>
        public Mesh(Triangle[] triangles)
        {
            if (triangles != null)
            {
                Triangles = triangles;

                HashSet<Vec4> chked = new HashSet<Vec4>();
                Vec4[] points = new Vec4[triangles.Length * 3];
                int pointI = 0;

                float maxX = float.NegativeInfinity, maxY = float.NegativeInfinity, maxZ = float.NegativeInfinity;
                float minX = float.PositiveInfinity, minY = float.PositiveInfinity, minZ = float.PositiveInfinity;
                for (int i = 0; i < triangles.Length; i++)
                {
                    for (int j = 0; j < triangles[i].Points.Length; j++)
                    {
                        Vec4 point = triangles[i].Points[j];
                        if (chked.Contains(point) == false)
                        {
                            if (minX > point.X)
                                minX = point.X;
                            if (minY > point.Y)
                                minY = point.Y;
                            if (minZ > point.Z)
                                minZ = point.Z;

                            if (maxX < point.X)
                                maxX = point.X;
                            if (maxY < point.Y)
                                maxY = point.Y;
                            if (maxZ < point.Z)
                                maxZ = point.Z;

                            points[pointI] = point;
                            pointI++;
                            chked.Add(point);
                        }
                    }
                }

                Points = new Vec4[pointI];
                for (int i = 0; i < pointI; i++)
                {
                    Points[i] = points[i];
                }

                bounds = new BoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));

                maxX = Math.Abs(maxX);
                maxY = Math.Abs(maxY);
                maxZ = Math.Abs(maxZ);

                minX = Math.Abs(minX);
                minY = Math.Abs(minY);
                minZ = Math.Abs(minZ);

                Size = new Vec4((maxX + minX), (maxY + minY), (maxZ + minZ));
                Position = new Vec4((maxX + minX) / 2, (maxY + minY) / 2, (maxZ + minZ) / 2);

                MoveTo(0, 0, 0);
            }
        }

        /// <summary>
        /// Clones this <see cref="Mesh"/> 
        /// </summary>
        /// <returns> Copy of this <see cref="Mesh"/> </returns>
        public Mesh Clone()
        {
            Triangle[] tris = new Triangle[this.Triangles.Length];
            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = this.Triangles[i].Clone();
            }

            return new Mesh(tris);
        }

        /// <summary>
        /// Scales a mesh 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Scale(float x, float y, float z)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Vec4 point = Points[i];
                point.X *= x;
                point.Y *= y;
                point.Z *= z;
            }
            Size.X *= x;
            Size.Y *= y;
            Size.Z *= z;
        }

        /// <summary>
        /// Places the mesh in the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void MoveTo(float x, float y, float z)
        {
            Vec4 oldPos = Position;
            float distX = oldPos.X - x;
            float distY = oldPos.Y - y;
            float distZ = oldPos.Z - z;

            for (int i = 0; i < Points.Length; i++)
            {
                Vec4 point = Points[i];
                point.X -= distX;
                point.Y -= distY;
                point.Z -= distZ;
            }
            Position.X = x;
            Position.Y = y;
            Position.Z = z;
        }

        public Vec4 GetPosition()
        {
            return Position;
        }

        /// <summary>
        /// Translates or moves this mesh to the specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(float x, float y, float z)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Vec4 point = Points[i];
                point.X -= x;
                point.Y -= y;
                point.Z -= z;
            }
        }

        /// <summary>
        /// Rotates an object
        /// NOT SET!
        /// </summary>
        /// <param name="up">Up vector of the rotation</param>
        /// <param name="fw">Forward vector of the rotation</param>
        /// <param name="rt">Right vector of the rotation</param>
        public void Rotate(Vec4 up, Vec4 fw, Vec4 rt)
        {

        }

        public void SetColor(Color color)
        {
            for (int i = 0; i < Triangles.Length; i++)
            {
                Triangles[i].Color = color;
            }
        }

        /// <returns>A printable statement of what this mesh is
        /// represented by (which is each of its triangles's 
        /// ToString() method in one giant string). </returns>
        public override string ToString()
        {
            string str = "";
            foreach (Triangle tri in Triangles)
            {
                str += tri + "\n";
            }
            return str;
        }
    }
}