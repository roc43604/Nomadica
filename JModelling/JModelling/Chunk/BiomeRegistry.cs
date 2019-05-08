using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    public class BiomeRegistry
    {

        private readonly static Biome[] BIOMES = new Biome[]{
            new Biome("Mountain", new Color[] {Color.LawnGreen}, new Color[] { }, 4000, 100, 100f),
            //new Biome("Desert", new Color[] {Color.Tan, Color.Red, Color.Black}, new Color[] { }, 2000, 100, 0f)
        };


        private static Biome[] biomeReg;

        static BiomeRegistry()
        {
            biomeReg = new Biome[50];

            int biomeInc = biomeReg.Length / BIOMES.Length;

            int num = biomeReg.Length;
            int count = 0;
            for (int b = 0; b < BIOMES.Length; b++)
            {
                for (int i = 1; i <= biomeInc; i++)
                {
                    biomeReg[count] = BIOMES[b];
                    count++;
                }
            }

            int biomeIndex = 0;
            int leftover = num - count;
            for (int i = 0; i < leftover; i++)
            {
                biomeReg[leftover + i - 1] = BIOMES[BIOMES.Length - 1];
            }

        }


        public static Biome GetBiomeFor(double point)
        {
            return biomeReg[(int)(point * biomeReg.Length)];
        }

    }
}
