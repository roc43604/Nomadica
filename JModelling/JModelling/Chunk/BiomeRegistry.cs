﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    public class BiomeRegistry
    {

        private readonly static Biome[,] BIOMES = new Biome[,]{
            {
                new Biome(
                    "Orange",
                    new Color[] {Color.Orange},
                    new Color[] { },
                    1300, 100, 0.05f,
                    null
                ),
                new Biome(
                    "Red",
                    new Color[] {Color.Red},
                    new Color[] { },
                    1100, 100, 0.05f,
                     null
                ),
                new Biome(
                    "Pink",
                    new Color[] {Color.Pink},
                    new Color[] { },
                    1000, 75, 0.5f,
                     null
                ),
            },
            {
                new Biome(
                    "Mountain",
                    new Color[] {Color.LawnGreen},
                    new Color[] { },
                    1500, 75, 0.5f,
                     null
                ),
                new Biome(
                    "Desert",
                    new Color[] {Color.Tan},
                    new Color[] { },
                    2000, 55, 0.5f,
                     null
                ),
                 new Biome(
                    "Gold",
                    new Color[] {Color.Gold},
                    new Color[] { },
                    1300, 100, 0.5f,
                     null
                ),
            }
        };


        private static Biome[] biomeRegUpper;
        private static Biome[] biomeRegLower;


        static BiomeRegistry()
        {
            
            /*
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
            for (int i=0; i<leftover; i++)
            {
                biomeReg[leftover + i - 1] = BIOMES[BIOMES.Length-1];
            }
            */
        }

        public static Biome GetBiomeFor(double x, double z)
        {
            //Console.WriteLine(x + " : " + (int)(x * BIOMES.GetLength(0)));
            return BIOMES[
                (int)(x * BIOMES.GetLength(0)),
                (int)(z * BIOMES.GetLength(1))
            ];
            /*
            point *= biomeReg.Length;
            if (point >= biomeReg.Length)
                point = biomeReg.Length - 1;
            else if (point < 0)
                point = 0;

            return biomeReg[(int)(point)];
            */
        }
        
    }
}
