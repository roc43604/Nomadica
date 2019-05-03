using JModelling.Chunk;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling.Chunk
{
    class Chunk
    {
        //private Biome biome;
        private int indexX;
        private int indexZ;
        private ListUtil<Mesh> chunkMeshes;
        private ListUtil<ListNode<Mesh>> loadedMeshes;

        private JManager manager;

        public Chunk(JManager manager, int indexX, int indexZ)
        {
            this.indexX = indexX;
            this.indexZ = indexZ;

            this.manager = manager;
        }

        public void Load()
        {
            ListNode<Mesh> list = chunkMeshes.list;

            while (list.next != null)
            {
                loadedMeshes.Add(manager.AddMesh(list.dat));
                list = list.next;
            }
        }

        public void UnLoad()
        {
            ListNode<Mesh> list = chunkMeshes.list;

            while (list.next != null)
            {
                manager.AddMesh(list.dat);
                list = list.next;
            }
        }
    }

    class ChunkGenerator
    {
        private const int CHUNK_DEFAULT_X = 200;
        private const int CHUNK_DEFAULT_Z = 200;

        private int chunkSizeX;
        private int chunkSizeZ;
        private int chunkSeed;
        private PerlinNoise chunkNoise;
        private PerlinNoise colorNoise;

        public int triSizeX = 40;
        public int triSizeZ = 40;

        public int viewDistX;
        public int viewDistZ;

        private int viewDist;
        private Mesh[,] chunkMesh;
        private Mesh cow;
        private JManager manager;

        private bool generated;
        
        public ChunkGenerator(int seed, int chunkSizeX, int chunkSizeZ, int viewDist, JManager manager, Mesh cow)
        {
            this.chunkSeed = seed;
            this.chunkSizeX = chunkSizeX;
            this.chunkSizeZ = chunkSizeZ;
            this.generated = false;

            this.cow = cow;
            this.viewDist = viewDist;
            // chunkMesh = new Mesh(new Triangle[] { });

            chunkMesh = new Mesh[viewDist*2, viewDist*2];
            for (int x=0; x<viewDist*2; x++)
                for (int z = 0; z < viewDist * 2; z++)
                    chunkMesh[x, z] = new Mesh(null);

            this.manager = manager;

            this.chunkNoise = new PerlinNoise(seed);
            this.colorNoise = new PerlinNoise(seed/2 * 345 - 1000 - 1233);
        }


        public float GetHeightAt(float posX, float posZ)
        {
            //  float posXn = posX/zoom / (triSizeX);

            //  return (float)chunkNoise.Noise(
            //      posX / zoom / (triSizeX),
            //      posZ / zoom / (triSizeZ),
            //      modi
            //  ) * amp;
            return 0;
        }

        public Biome BiomeAt(Vec4 pos)
        {
            return BiomeRegistry.GetBiomeFor(0.5);
        }

        /// <summary>
        /// <see cref=""/>
        /// </summary>
        /// <param name="indexX"></param>
        /// <param name="indexZ"></param>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public void GenerateChunks(int indexX, int indexZ, int modifier)
        {
            for (int cx=0; cx<viewDist * 2; cx++)
            {
                for (int cz = 0; cz<viewDist * 2; cz++)
                {
                    int increment = 1;
                    //int distFromMid = Math.Abs(viewDist - cx);
                    //if (distFromMid > 2)
                   // {
                    //    increment = 2;
                    //}

                    Triangle[] tris = new Triangle[chunkSizeX * chunkSizeZ * 2];

                    //Create tris
                    int idx = 0;
                    for (int x = 0; x < chunkSizeX; x+=increment)
                    {
                        for (int z = 0; z < chunkSizeZ; z+=increment)
                        {
                            int xF = (x + (indexX - viewDist + cx) * chunkSizeX);
                            int zF = (z + (indexZ - viewDist + cz) * chunkSizeZ);

                            Biome chunkBiome = BiomeRegistry.GetBiomeFor(
                                colorNoise.Noise(
                                    xF,//(indexX + cx),
                                    zF,//(indexZ + cz),
                                    0.5
                                )
                            );


                            float amp = chunkBiome.amp;
                            float zoom = chunkBiome.zoom;
                            float modi = chunkBiome.thatMagicNumber;

                            // double amp = 700;
                            // double zoom = 15;

                            double tL = chunkNoise.CreateNoiseHeight(
                                xF / zoom,
                                zF / zoom,
                                modi
                            ) * amp;

                            double tR = chunkNoise.CreateNoiseHeight(
                                (xF + 1) / zoom,
                                zF / zoom,
                                modi
                            ) * amp;
                            double bL = chunkNoise.CreateNoiseHeight(
                                xF / zoom,
                                (zF + 1) / zoom,
                                modi
                            ) * amp;
                            double bR = chunkNoise.CreateNoiseHeight(
                                (xF + 1) / zoom,
                                (zF + 1) / zoom,
                                modi
                            ) * amp;

                            int basePX = x * triSizeX + (indexX - viewDist + cx) * chunkSizeX * triSizeX;// - (chunkSizeX * triSizeX)/2;
                            int basePZ = z * triSizeZ + (indexZ - viewDist + cz) * chunkSizeZ * triSizeZ;// - (chunkSizeZ * triSizeZ)/2;

                            //Top Right, Bottom Left, Top Left
                            Triangle nA = new Triangle(new Vec4[] {
                                new Vec4(
                                    basePX + triSizeX,
                                    (float)tR,
                                    basePZ
                                ),
                                new Vec4(
                                    basePX,
                                    (float)bL,
                                    basePZ + triSizeZ
                                ),
                                new Vec4(
                                    basePX,
                                    (float)tL,
                                    basePZ
                                )
                            });
                            nA.Normal = Vec4.CrossProduct(
                                nA.Points[1] - nA.Points[0],
                                nA.Points[2] - nA.Points[0]
                            );
                            nA.Normal.Normalize();
                            nA.Normal *= -1;


                            //Bottom Left, Bottom Right, Top Right
                            Triangle nB = new Triangle(new Vec4[] {
                                new Vec4(
                                    basePX + triSizeX,
                                    (float)bR,
                                    basePZ + triSizeZ
                                ),
                                new Vec4(
                                    basePX + triSizeX,
                                    (float)tR,
                                    basePZ
                                ),
                                new Vec4(
                                    basePX,
                                    (float)bL,
                                    basePZ + triSizeZ
                                )
                            });
                            nB.Normal = Vec4.CrossProduct(
                                nB.Points[1] - nB.Points[0],
                                nB.Points[2] - nB.Points[0]
                            );
                            nB.Normal.Normalize();

                            tris[idx] = nA;
                            idx++;
                            tris[idx] = nB;
                            idx++;



                            //Colors
                            double clrNoise = colorNoise.Noise(
                                (xF / zoom) * 5,
                                (zF / zoom) * 5,
                                0.5
                            );
                            double shadeModifier = 20;

                            Color biomeColor = chunkBiome.GetEstimatedColorY(clrNoise);
                            double txtrVari = clrNoise * 2 - 1;

                            Color curColor = new Color(
                                (int)(biomeColor.R + shadeModifier * txtrVari),
                                (int)(biomeColor.G + shadeModifier * txtrVari),
                                (int)(biomeColor.B + shadeModifier * txtrVari)
                            );


                            nB.Color = curColor;
                            nA.Color = curColor;
                            
                            if (MathHelper.ToDegrees((float)Math.Sin(nA.Normal.Y)) < 40)
                            {
                                nA.Color = Color.Gray;
                            }
                            if (MathHelper.ToDegrees((float)Math.Sin(nB.Normal.Y)) < 40)
                            {
                                nB.Color = Color.Gray;
                            }


                            //Object Placement

                            /*
                              Ned sum code hear
                            */

                        }
                    }

                    chunkMesh[cx, cz].Triangles = tris;
                    
                }
            }

            if (generated == false)
            {
                for (int x=0; x<viewDist*2; x++)
                {
                    for (int z = 0; z < viewDist * 2; z++)
                    {
                        manager.AddMesh(chunkMesh[x, z]);
                    }
                }
                generated = true;
            }


        }
        
        public int GetIndexX(int posX)
        {
            return (posX) / (chunkSizeX * triSizeX);
        }


        public int GetIndexZ(int posZ)
        {
            return (posZ) / (chunkSizeZ * triSizeZ);
        }

    }
}
