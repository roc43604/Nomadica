using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A light source. 
    /// </summary>
    public class Light
    {
        /// <summary>
        /// How much light this gives off. A bonfire has a high
        /// intensity, while a match has a low intensity. 
        /// </summary>
        public float DistAway;

        /// <summary>
        /// Where in world-space this light is. 
        /// </summary>
        public Vec4 Loc; 

        public Light(float DistAway, Vec4 Loc)
        {
            this.DistAway = DistAway; 
            this.Loc = Loc; 
        }

        public float CalcAlphaFromDist(float distAway)
        {
            return -(1f / DistAway) * distAway + 1f;
        }
    }
}
