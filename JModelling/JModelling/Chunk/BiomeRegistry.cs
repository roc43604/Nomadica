using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    class BiomeRegistry
    {

        private readonly static Biome[] BIOMES = new Biome[]{
            new Biome("Desert", new Color[] {
                new Color(),
                new Color(),
            }),
        };


        private readonly static float biomeAllocationHeight;


        static BiomeRegistry()
        {
            biomeAllocationHeight = 1/BIOMES.Length;
        }


       //public GetBiomeFor(float height)
     //  {
           // (int)(height/1)
     //  }


       // public GetColorFor(float y)
       // {

       // }

        
    }
}
