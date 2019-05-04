using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A loading class, used to load in things JModelling needs, like 
    /// .obj files and height-maps. 
    /// </summary>
    class Load
    {
        private static ContentManager content; 

        private const float PIOverTwo = (float)(Math.PI / 2);

        private static Vec4 Up;

        private const float NormalRange = 0.78539f;
        //private static readonly Color StoneColor = new Color(181, 181, 181), 
        //                              GrassColor = new Color(112, 149, 43);
        
        /// <summary>
        /// Initializes the Load class, allowing it to perform its methods. 
        /// </summary>
        public static void Init(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
            Up = new Vec4(0, -1, 0);
            Up.Normalize();
        }

        /// <summary>
        /// Load a mesh given the name of the .obj file. The mesh will
        /// spawn at the location of (0, 0, 0). 
        /// </summary>
        public static Mesh Mesh(string fileName)
        {
            return Mesh(fileName, 1, 0, 0, 0); 
        }

        /// <summary>
        /// Loads a mesh given the name of the .obj file, how big it will
        /// be (2 = twice the size in the .obj file, 0.5 is half the size), 
        /// and its (x, y, z) location.
        /// </summary>
        public static Mesh Mesh(string fileName, float scale, float locX, float locY, float locZ)
        {
            // Loads the images the mesh will use.
            //Color[,] grass = TwoDimImage(@"Images/grass");
            //Color[,] stone = TwoDimImage(@"Images/stone"); 

            // Creates the system that will be reading the file. 
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(fileName); 
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Data);
                System.Environment.Exit(0); 
            }

            List<Vec4> points = new List<Vec4>();
            List<int> faces = new List<int>(); 
            while(!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(' '); 
                if(line.Length == 4)
                {
                    if(line[0] == "v")
                    {
                        float x = float.Parse(line[1]) + locX;
                        float distX = (locX - x) * scale;

                        float y = float.Parse(line[2]) + locY;
                        float distY = (locY - y) * scale;

                        float z = float.Parse(line[3]) + locZ;
                        float distZ = (locZ - z) * scale;

                        points.Add(new JModelling.Vec4(x - distX, y - distY, z - distZ)); 
                    }
                    else if(line[0] == "f")
                    {
                        faces.Add(int.Parse(line[1]));
                        faces.Add(int.Parse(line[2]));
                        faces.Add(int.Parse(line[3]));
                    }
                }
            }

            Triangle[] triangles = new Triangle[faces.Count / 3]; 
            for(int k = 0; k < faces.Count; k += 3)
            {
                Triangle triangle = new Triangle(points[faces[k] - 1], points[faces[k + 1] - 1], points[faces[k + 2] - 1], 
                    new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));

                Vec4 normal =
                    Vec4.CrossProduct(
                        triangle.Points[1] - triangle.Points[0],
                        triangle.Points[2] - triangle.Points[0]);
                normal.Normalize();
                triangle.Normal = normal;

                //float angle = GetAngle(normal, Up) - PIOverTwo;
                //if (angle < 0) angle += 180;
                //triangle.Angle = angle;

                //if (angle > NormalRange)
                //{
                //    triangle.Color = GrassColor;
                //    triangle.Image = grass; 
                //}
                //else
                //{
                //    triangle.Color = StoneColor;
                //    triangle.Image = stone; 
                //}

                triangles[k / 3] = triangle;                
            }

            return new Mesh(triangles);
        }

        private static float GetAngle(Vec4 one, Vec4 two)
        {
            return (float)Math.Acos(Vec4.DotProduct(one, two)); 
        }

        /// <summary>
        /// Loads an image into a one-dimensional array of colors. 
        /// </summary>
        public static Color[] OneDimImage(string fileName)
        {
            Texture2D tex = content.Load<Texture2D>(fileName);

            Color[] oneDimArray = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(oneDimArray);

            return oneDimArray; 
        }

        /// <summary>
        /// Loads an image into a two-dimensional array of colors.
        /// </summary>
        public static Color[,] TwoDimImage(string fileName)
        {
            Texture2D tex = content.Load<Texture2D>(fileName);

            Color[] oneDimArray = new Color[tex.Width * tex.Height];
            tex.GetData<Color>(oneDimArray);

            Color[,] twoDimArray = new Color[tex.Width, tex.Height]; 
            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    twoDimArray[x, y] = oneDimArray[y * tex.Width + x]; 
                }
            }
            
            return twoDimArray; 
        }
    }
}
