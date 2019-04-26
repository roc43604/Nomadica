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

        public int triSizeX = 80;
        public int triSizeZ = 80;

        public int viewDistX;
        public int viewDistZ;

        private int viewDist;
        private Mesh[,] chunkMesh;
        private Mesh cow;
        private JManager manager;

        private bool generated;

        private int amp = 700;
        private int zoom = 15;
        private double modi = 0.5;
        
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
            float posXn = posX/zoom / (triSizeX);

            return (float)chunkNoise.Noise(
                posX / zoom / (triSizeX),
                posZ / zoom / (triSizeZ),
                modi
            ) * amp;
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
                    Triangle[] tris = new Triangle[chunkSizeX * chunkSizeZ * 2];

                    Color c = new Color(cx*255, 0, cz*255);
                    int idx = 0;

                    double colorA = colorNoise.Noise(
                        (indexX+cx)/10,
                        (indexZ+cz)/10,
                        0.5
                    );
                    if (colorA > 0.1)
                        c = Color.LawnGreen;
                    else if (colorA <= 0.1 && colorA > -0.1)
                        c = Color.Tan;
                    else if (colorA <= -0.1)
                        c = Color.Brown;


                    for (int x = 0; x < chunkSizeX; x++)
                    {
                        for (int z = 0; z < chunkSizeZ; z++)
                        {
                            int xF = (x + (indexX - viewDist + cx) * chunkSizeX);
                            int zF = (z + (indexZ - viewDist + cz) * chunkSizeZ);

                            double amp = 700;
                            double zoom = 15;

                            double tL = chunkNoise.Noise(
                                xF / zoom,
                                zF / zoom,
                                modi
                            ) * amp;

                            double tR = chunkNoise.Noise(
                                (xF + 1) / zoom,
                                zF / zoom,
                                modi
                            ) * amp;
                            double bL = chunkNoise.Noise(
                                xF / zoom,
                                (zF + 1) / zoom,
                                modi
                            ) * amp;
                            double bR = chunkNoise.Noise(
                                (xF + 1) / zoom,
                                (zF + 1) / zoom,
                                modi
                            ) * amp;

                            int basePX = x * triSizeX + (indexX - viewDist + cx) * chunkSizeX * triSizeX;// - (chunkSizeX * triSizeX)/2;
                            int basePZ = z * triSizeZ + (indexZ - viewDist + cz) * chunkSizeZ * triSizeZ;// - (chunkSizeZ * triSizeZ)/2;
                            

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
                            //Top Right, Bottom Left, Top Left
                            nA.Normal = Vec4.CrossProduct(
                                nA.Points[1] - nA.Points[0],
                                nA.Points[2] - nA.Points[0]
                            );
                            nA.Normal.Normalize();
                            nA.Color = c;
                            nA.Color = new Color((int)(chunkNoise.Noise(xF, zF, modi))*255, 240, 240);
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
                            nB.Color = new Color((int)(chunkNoise.Noise(xF + 1, zF + 1, modi) * 255), 240, 240);

                            //nA.Color = new Color(1f, 0, 0);
                            //nB.Color = new Color(0, 1f, 0);

                            nB.Color = Color.LawnGreen;
                            nA.Color = Color.LawnGreen;

                            //  nB.Color = Color.LightBlue;
                            //  nA.Color = Color.LightBlue;

                            // else
                            // {

                            // }
                            
                            nA.Color = c;
                            nB.Color = c;


                            tris[idx] = nA;
                            idx++;
                            tris[idx] = nB;
                            idx++;
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
                        Console.WriteLine("ADDED");
                    }
                }
                generated = true;

               // chunkMesh[34543, 345345] = null;
            }


        }


        public void translate(Mesh mes, float x, float y, float z)
        {
            Triangle[] points = mes.Triangles;

            for (int i=0; i<points.Length; i++)
            {
                Vec4[] tri = points[i].Points;
                for (int j=0; j<tri.Length; j++)
                {
                    tri[j].X += x;
                    tri[j].Y += y;
                    tri[j].Z += z;
                }
                
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
