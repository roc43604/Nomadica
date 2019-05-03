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
        public Vec4[] Points;

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
            Triangles = triangles; 
        }

        /// <returns>A printable statement of what this mesh is
        /// represented by (which is each of its triangles's 
        /// ToString() method in one giant string). </returns>
        public override string ToString()
        {
            string str = "";
            foreach(Triangle tri in Triangles)
            {
                str += tri + "\n"; 
            }
            return str; 
        }
    }
}
