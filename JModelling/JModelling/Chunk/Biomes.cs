using JModelling.JModelling;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    public class Biome
    {

        public string Name;
        public Color[] clrYRange;
        public Color[] clrVariation;
        public Dictionary<Mesh, float> adornments;
        public float amp;
        public float zoom;
        public float thatMagicNumber;

        public Biome(
            string Name, 
            Color[] colorHeights, Color[] colorVariation,
            float amp, float zoom, float thatMagicNumber
           // Dictionary<Mesh, float> adornments
        )
        {
            this.Name = Name;
            this.clrYRange = colorHeights;
            this.clrVariation = colorVariation;
            this.amp = amp;
            this.zoom = zoom;
            this.thatMagicNumber = thatMagicNumber;
         //   this.adornments = adornments;
        }

        /// <summary>
        /// Gets the estimated color for the given height (0-1)
        /// </summary>
        /// <param name="y">The height 0-1</param>
        /// <returns>The biome color based off of the given height</returns>
        public Color GetEstimatedColorY(double y)
        {
            if (y > 1)
                y = 1;
            else if (y < 0)
                y = 0;
            int level = (int)(Math.Floor((y*100) * (clrYRange.Length-1)))/100;      

            return clrYRange[level];
        }


        public bool biomeEquals(Biome other)
        {
            if (other.Name.Equals(this.Name))
                return true;
            return false;
        }

        public string ToString()
        {
            return this.Name;
        }

    }
}
