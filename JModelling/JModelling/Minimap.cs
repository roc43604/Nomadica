using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling
{
    /// <summary>
    /// A menu on the side of the screen that shows the player's 
    /// discovered surroundings from a top-down perspective. 
    /// </summary>
    public class Minimap
    {
        /// <summary>
        /// Used for creating our own Texture2Ds. 
        /// </summary>
        internal static GraphicsDevice gd;

        private List<ChunkMap> maps; 

        /// <summary>
        /// Creates a new Minimap. Only one will be made per game, 
        /// and it will be displayed in the corner of the screen. 
        /// </summary>
        /// <param name="gd">Used to create Texture2Ds</param>
        public Minimap(ChunkGenerator gen, GraphicsDevice gd)
        {
            maps = new List<ChunkMap>(); 
            maps.Add(new ChunkMap(gd, gen.chunkMesh[4, 4])); 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(maps[0].tex, new Rectangle(0, 0, 100, 100), Color.White); 
        }
    }

    /// <summary>
    /// A texture of a single chunk. Each "unit" in the mesh
    /// represents one pixel. 
    /// </summary>
    internal class ChunkMap
    {
        internal Texture2D tex; 

        internal ChunkMap(GraphicsDevice gd, Mesh mesh)
        {
            BoundingBox box = mesh.bounds;

            // Creates a color array that we can draw to, and later
            // create a Texture2D from. Accessed via [x,z]. 
            Color[,] map = new Color[(int)(box.Max.X - box.Min.X), (int)(box.Max.Z - box.Min.Z)];
            Console.WriteLine((box.Max.X - box.Min.X) + " " + (box.Max.Z - box.Min.Z)); 

            foreach (Triangle tri in mesh.Triangles)
            {
                Color color = tri.Color;

                MinimapTriangle miniTri = new MinimapTriangle(tri); 
                if (miniTri.TwoPointsOnTop)
                {
                    if (miniTri.TwoPointsOnLeft)
                    {
                        int numSubtract = 0; 
                        for (int z = (int)miniTri.MinZ; z < miniTri.MaxZ; z++)
                        {
                            for (int x = (int)miniTri.MinX; x < miniTri.MaxX - numSubtract; x++)
                            {
                                Console.WriteLine(x + " " + z + " " + miniTri.MinX + " " + miniTri.MinZ); 
                                map[x-(int)miniTri.MinX, z-(int)miniTri.MinZ] = tri.Color; 
                            }
                            numSubtract++; 
                        }
                    }
                }
                else
                {

                }
            }

            tex = new Texture2D(gd, (int)(mesh.bounds.Max.X - mesh.bounds.Min.X), (int)(mesh.bounds.Max.Z - mesh.bounds.Min.Z)); 
        }
    }

    /// <summary>
    /// A container for helpful fields/methods involving triangles used
    /// specifically for this minimap. 
    /// </summary>
    internal class MinimapTriangle
    {
        /// <summary>
        /// The triangle's points in the range of [0...triangle width]. 
        /// </summary>
        internal Vec4[] OffsetPoints; 

        /// <summary>
        /// The bounds of the triangle. 
        /// </summary>
        internal float MinX, MaxX, MinZ, MaxZ;

        /// <summary>
        /// The index of the min z/x value, use for determining if two
        /// points are on the top/left of the triangle. 
        /// </summary>
        private float minZIndex, minXIndex; 

        /// <summary>
        /// Whether or not two points of this triangle are on the top/left. 
        /// The alternative would be two points on the bottom/right. 
        /// </summary>
        internal bool TwoPointsOnTop, TwoPointsOnLeft; 

        public MinimapTriangle(Triangle triangle)
        {
            OffsetPoints = OffsetTrianglePoints(triangle); 

            GetTriangleBounds(triangle);

            // Determine if two points are on the top by seeing if 
            // the minimum z has a duplicate. If there is only one 
            // minimum z value, then there are two points on the top. 
            int index = 0; 
            while (true)
            {
                if (index == 3)
                {
                    TwoPointsOnTop = true;
                    break;
                } 
                if (index != minZIndex)
                {
                    if (triangle.Points[index].Z == MinZ)
                    {
                        TwoPointsOnTop = false;
                        break; 
                    }
                }
                index++; 
            }

            // Same as above, although checks for points on the left. 
            index = 0; 
            while (true)
            {
                if (index == 3)
                {
                    TwoPointsOnLeft = true;
                    break;
                }
                if (index != minZIndex)
                {
                    if (index != minXIndex)
                    {
                        if (triangle.Points[index].X == MinX)
                        {
                            TwoPointsOnLeft = false;
                            break; 
                        }
                    }
                }
                index++;
            }
        }

        /// <summary>
        /// Makes a triangle's points in the range  of [0...triangle width] 
        /// </summary>
        /// <param name="tri">The triangle we're trying to offset</param>
        /// <returns>A new array of points that are offset from the original</returns>
        private Vec4[] OffsetTrianglePoints(Triangle tri)
        {
            Vec4 a = tri.Points[0],
                 b = tri.Points[1],
                 c = tri.Points[2];
            BoundingBox bounds = GetTriangleBounds(tri);

            return new Vec4[]
            {
                new Vec4(a.X - bounds.Min.X, a.Y, a.Z - bounds.Min.Z),
                new Vec4(b.X - bounds.Min.X, b.Y, b.Z - bounds.Min.Z),
                new Vec4(c.X - bounds.Min.X, c.Y, c.Z - bounds.Min.Z)
            };
        }

        /// <summary>
        /// Returns a bounding box of a triangle on the (x,z) level. 
        /// </summary>
        /// <param name="tri">The triangle we're getting a bounding box from</param>
        /// <returns>The bounding box of the given triangle.</returns>
        private BoundingBox GetTriangleBounds(Triangle tri)
        {
            MinX = float.MaxValue;
            MaxX = float.MinValue;
            MinZ = float.MaxValue;
            MaxZ = float.MinValue;

            // Finds sMallest/biggest values in each of the points
            for (int index = 0; index < 3; index++)
            {
                Vec4 point = tri.Points[index];
                if (point.X < MinX)
                {
                    MinX = point.X;
                    minXIndex = index; 
                }
                if (point.X > MaxX)
                {
                    MaxX = point.X;
                }
                if (point.Z < MinZ)
                {
                    MinZ = point.Z;
                    minZIndex = index; 
                }
                if (point.Z > MaxZ)
                {
                    MaxZ = point.Z;
                }
            }

            return new BoundingBox(new Vector3(MinX, 0, MinZ), new Vector3(MaxX, 0, MaxZ));
        }
    }
}
