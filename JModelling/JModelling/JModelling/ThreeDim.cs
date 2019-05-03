using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// An entity that has a mesh. It is able to be represented in
    /// 3D space. 
    /// </summary>
    public abstract class ThreeDim : Entity
    {
        /// <summary>
        /// The model used to represent this in 3D space. 
        /// </summary>
        public Mesh Mesh; 

        public ThreeDim(Vec4 Loc, Mesh Mesh)
            : base(Loc)
        {
            this.Mesh = Mesh; 
        }
    }
}
