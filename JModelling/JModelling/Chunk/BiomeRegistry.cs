using JModelling.JModelling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Chunk
{
    public class BiomeRegistry
    {

        private readonly Biome[,] BIOMES = null;


        private static Biome[] biomeRegUpper;
        private static Biome[] biomeRegLower;


        public BiomeRegistry()
        {
            BIOMES = new Biome[,]{
                {
                    new Biome(
                        "Orange",
                        new Color[] {Color.Orange},
                        new Color[] { },
                        1300, 100, 0.05f,
                        null
                    ),
                    new Biome(
                        "Forest",
                        new Color[] {Color.DarkSeaGreen},
                        new Color[] { },
                        1100, 100, 0.05f,
                        new Dictionary<AdorneeMesh, float>(){
                            {
                                new AdorneeMesh(
                                    Load.Mesh(@"Content/Models/cube.obj"),
                                    "Tree"
                                ),
                                0.25f
                            },
                        }
                    ),
                },
                {
                    new Biome(
                        "Mountain",
                        new Color[] {Color.LawnGreen},
                        new Color[] { },
                        1500, 75, 0.5f,
                        new Dictionary<AdorneeMesh, float>(){
                            {
                                new AdorneeMesh(
                                    Load.Mesh(@"Content/Models/cube.obj"),
                                    "Tree"
                                ),
                                0.75f
                            },
                            {
                                new AdorneeMesh(
                                    Load.Mesh(@"Content/Models/cube.obj").Scale(0.25f, 0.5f, 0.25f),
                                    "Flower"
                                ),
                                0.75f
                            },
                        }
                    ),
                    new Biome(
                        "Desert",
                        new Color[] {Color.Tan},
                        new Color[] { },
                        2000, 55, 0.5f,
                        new Dictionary<AdorneeMesh, float>(){
                            {
                                new AdorneeMesh(
                                    Load.Mesh(@"Content/Models/cube.obj"),
                                    "Tree"
                                ),
                                0.5f
                            },
                        }
                    ),
                }
            };

        }

        public Biome GetBiomeFor(double x, double z)
        {
            return BIOMES[
                (int)(x * BIOMES.GetLength(0)),
                (int)(z * BIOMES.GetLength(1))
            ];
        }
        
    }
}
