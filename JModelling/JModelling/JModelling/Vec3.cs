using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// An object that contains an U, V, and W component. Although the 
    /// Microsoft.Xna library already contains a Vector3 object, we don't
    /// use it in order to create our own operator extension methods
    /// (which C# doesn't allow for in an extension class). 
    /// 
    /// NOTE: Not many methods will actually involve the W component; this
    /// value is only used in extremely-specific scenarios. Most of the time, 
    /// this value will be kept blank with a value of 1. 
    /// </summary>
    public class Vec3
    {
        /// <summary>
        /// One of the three numbers used to create a Vec3. 
        /// </summary>
        public float U, V, W;

        /// <summary>
        /// Creates a default Vec3 object with 0 in all boxes. 
        /// </summary>
        public Vec3()
            : this(0, 0) { }

        /// <summary>
        /// Creates a Vec3 object given the coords of the U and
        /// V components. The W value is set to 1. 
        /// </summary>
        public Vec3(float u, float v)
            : this(u, v, 1) { }

        /// <summary>
        /// Creates a Vec3 object given the coords of each value. 
        /// </summary>
        public Vec3(float u, float v, float w)
        {
            U = u;
            V = v;
            W = w; 
        }

        /// <summary>
        /// Creates a copy of this Vec3
        /// </summary>
        public Vec3 Clone()
        {
            return new Vec3(U, V, W);
        }

        /// <summary>
        /// Divides the U, V, and W components of this vector 
        /// by a float value. 
        /// </summary>
        public void DivideEquals(float val)
        {
            U /= val;
            V /= val;
            W /= val; 
        }

        /// <summary>
        /// Returns a printable version of this Vec3, which is
        /// just "(U, V, W)"
        /// </summary>
        public override string ToString()
        {
            return "(" + U + ", " + V + ", " + W + ")"; 
        }
    }
}
