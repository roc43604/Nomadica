using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// Something that has a location in the world-space
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Where this entity is located in the world
        /// </summary>
        public Vec4 Loc; 

        public Entity(Vec4 Loc)
        {
            this.Loc = Loc; 
        }
    }
}
