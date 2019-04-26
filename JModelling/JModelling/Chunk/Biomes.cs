using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    class Biome
    {

        public string Name;
        public Color[] colorRange;

        public Biome(string Name, Color[] colorRange)
        {
            this.Name = Name;
            this.colorRange = colorRange;
        }

        public Color GetEstimatedColor(float y)
        {
            if (y > 1)
                y = 1;
            else if (y < 0)
                y = 0;

           // int closeIndex = (int)(y * (colorRange.Length-1));
            return colorRange[(int)(y * (colorRange.Length - 1))];
        }

    }
}
