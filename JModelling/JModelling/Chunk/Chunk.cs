using JModelling.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling.Chunk
{
    public class Chunk
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

    public class ChunkGenerator
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

        private float biomeZoom = 100f;

        public int viewDistX;
        public int viewDistZ;

        private int viewDist;
        private Mesh[,] chunkMesh;
        private Mesh cow;
        public JManager manager;

        private bool generated;
        private SpriteBatch spriteBatch;

        private ListUtil<Mesh> placedList = new ListUtil<Mesh>();

        public ChunkGenerator(int seed, int chunkSizeX, int chunkSizeZ, int viewDist, JManager manager, SpriteBatch spriteBatch, Mesh cow)
        {
            this.chunkSeed = seed;
            this.chunkSizeX = chunkSizeX;
            this.chunkSizeZ = chunkSizeZ;
            this.generated = false;
            this.spriteBatch = spriteBatch;

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


        class Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
        Dictionary<Point, bool> things = new Dictionary<Point, bool>();

        public float GetHeightAt(float posX, float posZ)
        {
            int indexX = GetIndexX((int)posX) * chunkSizeX;
            int indexZ = GetIndexZ((int)posZ) * chunkSizeZ;


            Biome biome = BiomeRegistry.GetBiomeFor(
                biomeX.Noise(
                    (indexX)/ biomeZoom,
                    (indexZ)/ biomeZoom,
                    0.5f
                ),
                biomeY.Noise(
                    (indexX)/ biomeZoom,
                    (indexZ)/ biomeZoom,
                    0.5f
                )
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

        public float[,] lerpHeightsWithSides(float[] top, float[] bot, float[] lft, float[] rht, int sl)
        {
            float[,] points = new float[sl, sl];

            //Top fill
            for (int i=0; i<sl; i++)
            {
                points[i, 0] = top[i];
            }
            //Bottom fill
            for (int i = 0; i <sl; i++)
            {
                points[i, sl-1] = bot[i];
            }

            //Left fill
            for (int i = 0; i < sl; i++)
            {
                points[0, i] = lft[i];
            }
            //Right fill
            for (int i = 0; i < sl; i++)
            {
                points[sl-1, i] = rht[i];
            }


            //Calculate points Top to Bottom
            for (int x = 1; x < sl - 1; x++)//1, -1 because sides pre-calculated
            {
                float ptop = points[x, 0];
                float pbot = points[x, sl - 1];

                float dist = pbot - ptop;
                float scle = dist / (sl - 1);

                for (int y = 1; y < sl - 1; y++)//1, -1 because sides pre-calculated
                {
                    points[x, y] = ptop + scle * y;
                    /*
                    StringBuilder builder = new StringBuilder();
                    for (int k=0; k<sl; k++)
                    {
                        for (int b = 0; b < sl; b++)
                        {
                            builder.Append((int)points[k, b]).Append(", ");
                        }
                        builder.Append("\n");
                    }
                    Console.WriteLine(builder.ToString());
                    builder.ToString();
                    */
                }
            }

            //Calculate points Left to Right
            for (int y = 1; y < sl-1; y++)//1, -1 because sides pre-calculated
            {
                float pleft = points[0, y];
                float pright = points[sl - 1, y];

                float dist = pright - pleft;
                float scle = dist / (sl - 1);

                for (int x = 1; x < sl - 1; x++)//1, -1 because sides pre-calculated
                {
                    points[x, y] += pleft + scle * x;
                    points[x, y] /= 2; // divide by 2 for average
                }
            }

            return points;
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

                    Triangle[] tris = new Triangle[(chunkSizeX * chunkSizeZ * 2)];

                    //Create tris
                    int idx = 0;
                    int stepInc = 8;
                    int limit = 35; //Gray limit

                    bool doLerp = false;

                    //Determine if we should lerp or not
                    for (int x = 0; x < chunkSizeX+4; x++)
                    {
                        for (int z = 0; z < chunkSizeZ+4; z++)
                        {
                            int tLX = ((x - 2) + (indexX - viewDist + cx) * chunkSizeX);
                            int tLZ = ((z - 2) + (indexZ - viewDist + cz) * chunkSizeZ);

                            int bRX = (((x-2) + stepInc) + (indexX - viewDist + cx) * chunkSizeX);
                            int bRZ = (((z-2) + stepInc) + (indexZ - viewDist + cz) * chunkSizeZ);

                            Biome bTL = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    (tLX) / biomeZoom,
                                    (tLZ) / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    (tLX) / biomeZoom,
                                    (tLZ) / biomeZoom,
                                    0.5f
                                )
                            );

                            Biome bBR = BiomeRegistry.GetBiomeFor(
                                biomeX.Noise(
                                    (bRX) / biomeZoom,
                                    (bRZ) / biomeZoom,
                                    0.5f
                                ),
                                biomeY.Noise(
                                    (bRX) / biomeZoom,
                                    (bRZ) / biomeZoom,
                                    0.5f
                                )
                            );

                            if (!bBR.Equals(bTL))
                            {
                                doLerp = true;
                            }
                        }
                    }
                            

                    //Looks like they are not the same biome, lets lerp
                    if (doLerp)
                    {
                        for (int x = 0; x < chunkSizeX / stepInc; x++)
                        {
                            for (int z = 0; z < chunkSizeZ / stepInc; z++)
                            {
                                //Find the biomes for amplitude, zoom, and magic data

                                int x10 = (x + (indexX - viewDist + cx) * chunkSizeX);
                                int z10 = (z + (indexZ - viewDist + cz) * chunkSizeZ);

                                int x11 = ((x + stepInc) + (indexX - viewDist + cx) * chunkSizeX);
                                int z11 = ((z) + (indexZ - viewDist + cz) * chunkSizeZ);

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


                                int x20 = ((x) + (indexX - viewDist + cx) * chunkSizeX);
                                int z20 = ((z + stepInc) + (indexZ - viewDist + cz) * chunkSizeZ);

                                int x21 = ((x + stepInc) + (indexX - viewDist + cx) * chunkSizeX);
                                int z21 = ((z + stepInc) + (indexZ - viewDist + cz) * chunkSizeZ);

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

                                float[,] generatedPoints = lerpHeights(
                                    h10, h20,
                                    h11, h21,
                                    stepInc + 1
                                );

                                //Sample sides
                                float[] leftSideSample = new float[stepInc +1];
                                float[] rightSideSample = new float[stepInc +1];
                                float[] topSideSample = new float[stepInc +1];
                                float[] botSideSample = new float[stepInc +1];

                                //Left
                                for (int i=0; i<leftSideSample.Length; i++)
                                {
                                    int posX = ((x + i) + (indexX - viewDist + cx) * chunkSizeX);
                                    int posZ = ((z) + (indexZ - viewDist + cz) * chunkSizeZ);

                                    Biome biome = BiomeRegistry.GetBiomeFor(
                                        biomeX.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        ),
                                        biomeY.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        )
                                    );

                                    leftSideSample[i] = chunkNoise.CreateNoiseHeight(
                                        posX / biome.zoom,
                                        posZ / biome.zoom,
                                        biome.thatMagicNumber
                                    ) * biome.amp; ;
                                }

                                //Right
                                for (int i = 0; i < rightSideSample.Length; i++)
                                {
                                    int posX = ((x + i) + (indexX - viewDist + cx) * chunkSizeX);
                                    int posZ = ((z + stepInc) + (indexZ - viewDist + cz) * chunkSizeZ);

                                    Biome biome = BiomeRegistry.GetBiomeFor(
                                        biomeX.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        ),
                                        biomeY.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        )
                                    );

                                    rightSideSample[i] = chunkNoise.CreateNoiseHeight(
                                        posX / biome.zoom,
                                        posZ / biome.zoom,
                                        biome.thatMagicNumber
                                    ) * biome.amp; ;
                                }

                                //Top
                                for (int i = 0; i < topSideSample.Length; i++)
                                {
                                    int posX = ((x) + (indexX - viewDist + cx) * chunkSizeX);
                                    int posZ = ((z + i) + (indexZ - viewDist + cz) * chunkSizeZ);

                                    Biome biome = BiomeRegistry.GetBiomeFor(
                                        biomeX.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        ),
                                        biomeY.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        )
                                    );

                                    topSideSample[i] = chunkNoise.CreateNoiseHeight(
                                        posX / biome.zoom,
                                        posZ / biome.zoom,
                                        biome.thatMagicNumber
                                    ) * biome.amp; ;
                                }

                                //Bottom
                                for (int i = 0; i < botSideSample.Length; i++)
                                {
                                    int posX = ((x + stepInc) + (indexX - viewDist + cx) * chunkSizeX);
                                    int posZ = ((z + i) + (indexZ - viewDist + cz) * chunkSizeZ);

                                    Biome biome = BiomeRegistry.GetBiomeFor(
                                        biomeX.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        ),
                                        biomeY.Noise(
                                            posX / biomeZoom,
                                            posZ / biomeZoom,
                                            0.5f
                                        )
                                    );

                                    botSideSample[i] = chunkNoise.CreateNoiseHeight(
                                        posX / biome.zoom,
                                        posZ / biome.zoom,
                                        biome.thatMagicNumber
                                    ) * biome.amp; ;
                                }

                                float[,] generatedPoints2 = lerpHeightsWithSides(
                                    leftSideSample, rightSideSample,
                                   topSideSample, botSideSample,
                                   
                                   stepInc + 1
                                );


                                //Now time to generate the triangles from the interpolation
                                for (int tx = 0; tx < stepInc; tx++)
                                {
                                    for (int tz = 0; tz < stepInc; tz++)
                                    {

                                        int iX = ((x + tx) + (indexX - viewDist + cx) * chunkSizeX);
                                        int iZ = ((z + tz) + (indexZ - viewDist + cz) * chunkSizeZ);
                                        
                                        float pX = (x + tx+1) * triSizeX + (indexX - viewDist + cx) * chunkSizeX * triSizeX;
                                        float pZ = (z + tz+1) * triSizeZ + (indexZ - viewDist + cz) * chunkSizeZ * triSizeZ;

                                        float tL = generatedPoints2[tx + 1, tz + 1];
                                        float tR = generatedPoints2[tx, tz + 1];
                                        float bL = generatedPoints2[tx + 1, tz];
                                        float bR = generatedPoints2[tx, tz];


                                        

                                        //Top Right, Bottom Left, Top Left
                                        Triangle nA = new Triangle(new Vec4[] {
                                            new Vec4(
                                                pX,
                                                (float)tL,
                                                pZ
                                            ),
                                            new Vec4(
                                                pX - triSizeX,
                                                (float)tR,
                                                pZ
                                            ),
                                            new Vec4(
                                                pX,
                                                (float)bL,
                                                pZ - triSizeZ
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
                                                pX - triSizeX,
                                                (float)bR,
                                                pZ - triSizeZ
                                            ),
                                            new Vec4(
                                                pX - triSizeX,
                                                (float)tR,
                                                pZ
                                            ),
                                            new Vec4(
                                                pX,
                                                (float)bL,
                                                pZ - triSizeZ
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


                                        Biome chunkBiome = BiomeRegistry.GetBiomeFor(
                                            biomeX.Noise(
                                                iX / biomeZoom,
                                                iZ / biomeZoom,
                                                0.5f
                                            ),
                                            biomeY.Noise(
                                                iX / biomeZoom,
                                                iZ / biomeZoom,
                                                0.5f
                                            )
                                        );

                                        //Colors
                                        double clrNoise = colorNoise.Noise(
                                            (pX / 50) * 5,
                                            (pZ / 50) * 5,
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

                                        
                                        if (MathHelper.ToDegrees((float)Math.Sin(nA.Normal.Y)) < limit-5 || MathHelper.ToDegrees((float)Math.Sin(nB.Normal.Y)) < 35)
                                        {
                                            Color gray = new Color(
                                                (int)(127 + txtrVari * 48),
                                                (int)(127 + txtrVari * 48),
                                                (int)(127 + txtrVari * 48)
                                            );
                                            nA.Color = gray;
                                            nB.Color = gray;
                                        }



                                        //Object Placement
                                        Mesh tree = chunkBiome.Tree;

                                        if (tree != null) {
                                            double noise = colorNoise.Noise(
                                                (pX / 50) * 5,
                                                (pZ / 50) * 5,
                                                0.5
                                            );

                                            if (noise < 0.5)
                                            {
                                                manager.AddMesh(tree);
                                                placedList.Add(new ListNode<Mesh>(tree));
                                            }
                                            
                                        }

                                        ListNode<Mesh> placed = placedList.list;
                                        while (placed != null)
                                        {
                                            Mesh mesh = placed.dat;
                                            

                                            if ((mesh.GetPosition() - manager.camera.loc).Length() > 5000)
                                            {
                                                placed.Remove();
                                            }
                                            placed = placed.next;
                                        }
                                    }
                                }

                            }
                        }

                        chunkMesh[cx, cz].Triangles = tris;



                    //Lools like they are the same biome
                    }
                    else
                    {
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

                                float xF = (x + (indexX - viewDist + cx) * chunkSizeX);
                                float zF = (z + (indexZ - viewDist + cz) * chunkSizeZ);

                                float basePX = x * triSizeX + (indexX - viewDist + cx) * chunkSizeX * triSizeX;
                                float basePZ = z * triSizeZ + (indexZ - viewDist + cz) * chunkSizeZ * triSizeZ;


                                Biome chunkBiome = BiomeRegistry.GetBiomeFor(
                                    biomeX.Noise(
                                        xF / biomeZoom,
                                        zF / biomeZoom,
                                        0.5f
                                    ),
                                    biomeY.Noise(
                                        xF / biomeZoom,
                                        zF / biomeZoom,
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



                                //Colors :)
                                double clrNoise = colorNoise.Noise(
                                    (basePX / 50) * 5,
                                    (basePZ / 50) * 5,
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

                                if (MathHelper.ToDegrees((float)Math.Sin(nA.Normal.Y)) < limit || MathHelper.ToDegrees((float)Math.Sin(nB.Normal.Y)) < 35)
                                {
                                    Color gray = new Color(
                                        (int)(127 + txtrVari * 48),
                                        (int)(127 + txtrVari * 48),
                                        (int)(127 + txtrVari * 48)
                                    );
                                    nA.Color = gray;
                                    nB.Color = gray;
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
