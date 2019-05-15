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
        private PerlinNoise biomeX;
        private PerlinNoise biomeY;

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

            this.biomeX = new PerlinNoise(seed * 100 + 934 * 4657);
            this.biomeY = new PerlinNoise(seed * 833 - 934 / 3455);
        }


        public float GetHeightAt(float posX, float posZ)
        {
            Biome biome = BiomeRegistry.GetBiomeFor(
                colorNoise.Noise(
                    GetIndexX((int)posX),
                    GetIndexZ((int)posZ),
                    0.5
                ),
                0.5
            );

            return (float)chunkNoise.CreateNoiseHeight(
                posX / biome.zoom / (triSizeX),
                posZ / biome.zoom / (triSizeZ),
                biome.thatMagicNumber
            ) * biome.amp;
        }

        public string BiomeAt(Vec4 pos)
        {
            return "String";
        }

        public float[,] lerpHeights(float p10, float p11, float p20, float p21, int sl)
        {
            /*
             * [10][20]
             * [11][21]
             */

            float[,] points = new float[sl, sl];

            float dist1020 = p20 - p10;
            float f1020 = dist1020 / (sl-1);

            float dist1121 = p11 - p21;
            float f1121 = dist1121 / (sl-1);

            float dist1011 = p10 - p11;
            float f1011 = dist1011 / (sl-1);

            float dist2021 = p20 - p21;
            float f2021 = dist2021 / (sl-1);

            //Top row
            for (int x = 0; x < sl; x++)
                points[x, 0] = p10 + f1020 * x;
            //Bottom
            for (int x = 0; x < sl; x++)
                points[x, sl-1] = p11 - f1121 * x;

            //Side
            // for (int y = 0; y < sl; y++)
            //   points[0, y] = p10 - f1020 * y;
            //Bottom
            //for (int y = 0; y < sl; y++)
            //   points[sl - 1, y] = p20 + f1121 * y;

            //Calculate points Top to Bottom
            for (int x = 0; x < sl; x++)
            {
                float top = points[x, 0];
                float bot = points[x, sl - 1];

                float dist = bot - top;
                float scle = dist / (sl - 1);

                //1 and -1 because top and bottom already calculated
                for (int y = 1; y < sl-1; y++) 
                {
                    points[x, y] = top + scle * y;
                }
            }

            //StringBuilder builder2 = new StringBuilder();
            //for (int x = 0; x < sl - 1; x++)
            //{
            //    for (int y = 0; y < sl - 1; y++)
            //    {
            //        builder2.Append((int)points[x, y]).Append(", ");
            //    }
            //    builder2.Append("\n");
            //}
            //Console.WriteLine(builder2.ToString());
            //builder2.ToString();
            return points;
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
                    int distFromMidX = Math.Abs(viewDist - cx);
                   // int distFromMidZ = Math.Abs(viewDist = cz);
                    if (distFromMidX > 2)
                    {
                      // increment = 2;
                    }

                    Triangle[] tris = new Triangle[(chunkSizeX * chunkSizeZ * 2)/increment];

                    //Create tris
                    int idx = 0;

                    int stepInc = 4;
                    for (int x = 0; x < chunkSizeX/stepInc; x++)
                    {
                        for (int z = 0; z < chunkSizeZ/stepInc; z++)
                        {
                            //Find the biomes for amplitude, zoom, and magic data
                            float biomeZoom = 200f;

                            int x10 = (x * increment + (indexX - viewDist + cx) * chunkSizeX);
                            int z10 = (z * increment + (indexZ - viewDist + cz) * chunkSizeZ);

                            int x11 = ((x+stepInc) * increment + (indexX - viewDist + cx) * chunkSizeX);
                            int z11 = ((z) * increment + (indexZ - viewDist + cz) * chunkSizeZ);

                            Biome b10 = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    x10 / biomeZoom,
                                    z10 / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    x10 / biomeZoom,
                                    z10 / biomeZoom,
                                    0.5f
                                )
                            );
                            Biome b11 = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    x11 / biomeZoom,
                                    z11 / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    x11 / biomeZoom,
                                    z11 / biomeZoom,
                                    0.5f
                                )
                            );


                            int x20 = ((x) * increment + (indexX - viewDist + cx) * chunkSizeX);
                            int z20 = ((z + stepInc) * increment + (indexZ - viewDist + cz) * chunkSizeZ);

                            int x21 = ((x + stepInc) * increment + (indexX - viewDist + cx) * chunkSizeX);
                            int z21 = ((z + stepInc) * increment + (indexZ - viewDist + cz) * chunkSizeZ);

                            Biome b20 = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    x20 / biomeZoom,
                                    z20 / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    x20 / biomeZoom,
                                    z20 / biomeZoom,
                                    0.5f
                                )
                            );
                            Biome b21 = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    x21 / biomeZoom,
                                    z21 / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    x21 / biomeZoom,
                                    z21 / biomeZoom,
                                    0.5f
                                )
                            );

                            
                            //Generate the heights
                            float h10 = chunkNoise.CreateNoiseHeight(
                                x10 / b10.zoom,
                                z10 / b10.zoom,
                                b10.thatMagicNumber
                            ) * b10.amp;
                            float h11 = chunkNoise.CreateNoiseHeight(
                                x11 / b11.zoom,
                                z11 / b11.zoom,
                                b11.thatMagicNumber
                            ) * b11.amp; ;
                            float h20 = chunkNoise.CreateNoiseHeight(
                                x20 / b20.zoom,
                                z20 / b20.zoom,
                                b20.thatMagicNumber
                            ) * b20.amp; ;
                            float h21 = chunkNoise.CreateNoiseHeight(
                                x21 / b21.zoom,
                                z21 / b21.zoom,
                                b21.thatMagicNumber
                            ) * b21.amp; ;

                            //Now time to generate the triangles from the interpolation
                            float[,] generatedPoints = lerpHeights(
                                h10, h20,
                                h11, h21,
                                stepInc+1
                            );

                            float zoom = b10.zoom;

                            for (int tx = 0; tx < stepInc; tx++)
                            {
                                for (int tz = 0; tz < stepInc; tz++)
                                {
                                    int iX = ((x + tx) * increment + (indexX - viewDist + cx) * chunkSizeX);
                                    int iZ = ((z + tz) * increment + (indexX - viewDist + cz) * chunkSizeZ);

                                    float pX = (x + tx) * triSizeX * increment + (indexX - viewDist + cx) * chunkSizeX * triSizeX;
                                    float pZ = (z + tz) * triSizeZ * increment + (indexZ - viewDist + cz) * chunkSizeZ * triSizeZ;

                                    float tL= generatedPoints[tx+1, tz+1];
                                    float tR = generatedPoints[tx, tz+1];
                                    float bL = generatedPoints[tx+1, tz];
                                    float bR = generatedPoints[tx, tz];


                                    //Top Right, Bottom Left, Top Left
                                    Triangle nA = new Triangle(new Vec4[] {
                                        new Vec4(
                                            pX,
                                            (float)tL,
                                            pZ
                                        ),
                                        new Vec4(
                                            pX - triSizeX*increment,
                                            (float)tR,
                                            pZ
                                        ),
                                        new Vec4(
                                            pX,
                                            (float)bL,
                                            pZ - triSizeZ*increment
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
                                            pX - triSizeX*increment,
                                            (float)bR,
                                            pZ - triSizeZ*increment
                                        ),
                                        new Vec4(
                                            pX - triSizeX*increment,
                                            (float)tR,
                                            pZ
                                        ),
                                        new Vec4(
                                            pX,
                                            (float)bL,
                                            pZ - triSizeZ*increment
                                        )
                                    });
                                    nB.Normal = Vec4.CrossProduct(
                                        nB.Points[1] - nB.Points[0],
                                        nB.Points[2] - nB.Points[0]
                                    );
                                    nB.Normal.Normalize();




                                    double clrNoise = colorNoise.Noise(
                                        (pX / zoom) * 5,
                                        (pZ / zoom) * 5,
                                        0.5
                                    );
                                    double shadeModifier = 20;

                                    Color biomeColor = b10.GetEstimatedColorY(clrNoise);
                                    double txtrVari = clrNoise * 2 - 1;

                                    Color curColor = new Color(
                                        (int)(biomeColor.R + shadeModifier * txtrVari),
                                        (int)(biomeColor.G + shadeModifier * txtrVari),
                                        (int)(biomeColor.B + shadeModifier * txtrVari)
                                    );


                                    nB.Color = curColor;
                                    nA.Color = curColor;

                                    int limit = 35;
                                    if (MathHelper.ToDegrees((float)Math.Sin(nA.Normal.Y)) < limit || MathHelper.ToDegrees((float)Math.Sin(nB.Normal.Y)) < 35)
                                    {
                                        nA.Color = Color.Gray;
                                        nB.Color = Color.Gray;
                                    }


                                    //Add triangles to the triangle array
                                    tris[idx] = nA;
                                    idx++;
                                    tris[idx] = nB;
                                    idx++;


                                }
                            }

                        }
                    }

                    chunkMesh[cx, cz].Triangles = tris;



                    /*

                    for (int x = 0; x < chunkSizeX; x++)
                    {
                        if (idx >= tris.Length)
                        {
                            continue;
                        }
                        for (int z = 0; z < chunkSizeZ; z++)
                        {
                            if (idx >= tris.Length)
                            {
                                continue;
                            }

                            float xF = (x*increment + (indexX - viewDist + cx) * chunkSizeX);
                            float zF = (z*increment + (indexZ - viewDist + cz) * chunkSizeZ);
                            


                            
                            float biomeZoom = 200f;
                            float biomeScale = 1f;
                            Biome chunkBiome = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    (xF + 1)* biomeScale / biomeZoom,
                                    zF * biomeScale / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    xF * biomeScale / biomeZoom,
                                    (zF - 1) * biomeScale / biomeZoom,
                                    0.5f
                                )
                            );
                            
                            


                            float amp = chunkBiome.amp;
                            float zoom = chunkBiome.zoom;
                            float modi = chunkBiome.thatMagicNumber;
                            

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

                            //Top Right, Bottom Left, Top Left
                            Triangle nA = new Triangle(new Vec4[] {
                                new Vec4(
                                    basePX,
                                    (float)tL,
                                    basePZ
                                ),
                                new Vec4(
                                    basePX + triSizeX*increment,
                                    (float)tR,
                                    basePZ
                                ),
                                new Vec4(
                                    basePX,
                                    (float)bL,
                                    basePZ + triSizeZ*increment
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
                                    basePX + triSizeX*increment,
                                    (float)bR,
                                    basePZ + triSizeZ*increment
                                ),
                                new Vec4(
                                    basePX + triSizeX*increment,
                                    (float)tR,
                                    basePZ
                                ),
                                new Vec4(
                                    basePX,
                                    (float)bL,
                                    basePZ + triSizeZ*increment
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

                            int limit = 35;
                            if (MathHelper.ToDegrees((float)Math.Sin(nA.Normal.Y)) < limit || MathHelper.ToDegrees((float)Math.Sin(nB.Normal.Y)) < 35)
                            {
                                nA.Color = Color.Gray;
                                nB.Color = Color.Gray;
                            }
                            
                            //Object Placement

                            /*
                              Ned sum code hear
                            

                        }
                    }
            
                    chunkMesh[cx, cz].Triangles = tris;
                    */

                    //Console.WriteLine(builder.ToString());
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
            return (posX) / (chunkSizeX * triSizeX) ;
        }


        public int GetIndexZ(int posZ)
        {
            return (posZ) / (chunkSizeZ * triSizeZ);
        }

    }
}
