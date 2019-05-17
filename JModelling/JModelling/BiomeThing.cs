using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling
{
    class Chunker
    {
        private const int CHUNK_WIDTH = 10;
        private const int CHUNK_LENGTH = 10;


        private PerlinNoise noiseGen;


        public Chunker(int seed)
        {
            this.noiseGen = new PerlinNoise(seed);
        }


        public int getChunkIndexX(int x)
        {
            return x / (CHUNK_WIDTH * 8);
        }
        public int getChunkIndexZ(int z)
        {
            return z / (CHUNK_LENGTH * 8);
        }


        public double[,] GetHeights(int chunkX, int chunkY)
        {
            double[,] heights = new double[CHUNK_WIDTH, CHUNK_LENGTH];

            for (int x=0; x<heights.GetLength(0); x++)
            {
                for (int z=0; z<heights.GetLength(1); z++)
                {
                    heights[x, z] = 
                        noiseGen.Noise((chunkX+x)*50, (chunkY+z)*50, -0.5) + 
                        noiseGen.Noise((chunkX*3+x)*50, (chunkY * 3+z)*50, 0) + 
                        noiseGen.Noise((chunkX * 8+x)*50, (chunkY *8+z)*50, 0.5);

                }
            }

            return heights;
        }
    }
}
