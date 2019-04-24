using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// An array of Vec4's that generate a terrain. 
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The default size of a map. 
        /// </summary>
        public static readonly Vector2 DefaultSize = new Vector2(10, 10);

        /// <summary>
        /// The default difference between any two points on the map. 
        /// </summary>
        public const int DefaultIntensity = 5;

        /// <summary>
        /// The points that make up the terrain. 
        /// </summary>
        public Vec4[,] Vecs;

        /// <summary>
        /// The triangles that connect the various terrain points.
        /// </summary>
        public Triangle[] Tris;

        /// <summary>
        /// Generates a default map. 
        /// </summary>
        public Map()
            : this(DefaultIntensity)
        { }

        /// <summary>
        /// Generates a map of differing intensity between the points. 
        /// </summary>
        /// <param name="intensity"></param>
        public Map(int intensity)
            : this(intensity, (int)DefaultSize.X, (int)DefaultSize.Y)
        { }

        public Map(int intensity, int width, int height)
            : this(intensity, width, height, 0)
        { }

        /// <summary>
        /// Generates a map with a set intensity, size, and starting altitude.
        /// </summary>
        public Map(int intensity, int width, int height, int baseLine)
        {
            Vecs = new Vec4[width, height];

            Random random = new Random(); 
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vecs[x, z] = new Vec4(x * 10, random.Next(-intensity, intensity) * 10, z * 10);
                }
            }


            for(int x = 0; x < width - 1; x++)
            {
                for (int z = 0; z < height - 1; z++)
                {
                    
                }
            }
        }
    }
}
