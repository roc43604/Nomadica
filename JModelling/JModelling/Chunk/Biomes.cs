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
        public float amp;
        public float zoom;
        public float thatMagicNumber;

        public Biome(
            string Name, 
            Color[] colorHeights, Color[] colorVariation,
            float amp, float zoom, float thatMagicNumber
        )
        {
            this.Name = Name;
            this.clrYRange = colorHeights;
            this.clrVariation = colorVariation;
            this.amp = amp;
            this.zoom = zoom;
            this.thatMagicNumber = thatMagicNumber;
        }

        public Color GetEstimatedColorY(double y)
        {
            if (y > 1)
                y = 1;
            else if (y < 0)
                y = 0;
            int level = (int)(Math.Floor((y*100) * (clrYRange.Length-1)))/100;      

            return clrYRange[level];
        }


        public string ToString()
        {
            return this.Name;
        }

    }
}
