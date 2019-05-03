using GraphicsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// The image set for the sky. 
    /// </summary>
    public class SkyBox : ThreeDim
    {
        /// <summary>
        /// How far away the skybox will be from the player. 
        /// </summary>
        public const int Dist = 1;

        /// <summary>
        /// How big one part of the box is. 
        /// </summary>
        public const int Size = Dist;

        public Dictionary<WeatherAndTime, Color[][,]> Textures;

        public SkyBox(WeatherAndTime[] times, string[] fileNames, int width, int height)
            : this(times, fileNames, GenerateSkyboxTextures(width, height))
        { }

        public SkyBox(WeatherAndTime[] times, string[] fileNames, Rectangle[] rectangles)
            : this(times, fileNames, rectangles, Vec4.Zero)
        { }

        /// <summary>
        /// Creates a sky box given the name of the file the image is in, and
        /// the Rectangle for each frame of the box. It will be loaded in the
        /// order of:
        /// 
        /// Front, Right, Left, Back, Up, Down. 
        /// </summary>
        public SkyBox(WeatherAndTime[] times, string[] fileNames, Rectangle[] rectangles, Vec4 Loc)
            : base(Loc, CreateSkybox(Loc))
        {
            // Load skybox textures. 
            Color[][,] source = new Color[fileNames.Length][,];
            Textures = new Dictionary<WeatherAndTime, Color[][,]>(fileNames.Length);
            for (int index = 0; index < fileNames.Length; index++)
            {
                source[index] = Load.TwoDimImage(fileNames[index]);

                Textures[times[index]] = new Color[][,] {
                    LoadPart(source[index], rectangles[0]),
                    LoadPart(source[index], rectangles[1]),
                    LoadPart(source[index], rectangles[2]),
                    LoadPart(source[index], rectangles[3]),
                    LoadPart(source[index], rectangles[4]),
                    LoadPart(source[index], rectangles[5]) };
            }
        }

        /// <summary>
        /// Creates a list of rectangles that say where the parts of the skybox
        /// are in a texture. 
        /// </summary>
        private static Rectangle[] GenerateSkyboxTextures(int partWidth, int partHeight)
        {
            int width = partWidth / 4, 
                height = partHeight / 3;
            return new Rectangle[] {
                new Rectangle(width * 2, height, width, height),
                new Rectangle(width * 3, height, width, height),
                new Rectangle(width, height, width, height),
                new Rectangle(0, height, width, height),
                new Rectangle(width, 0, width, height),
                new Rectangle(width, height * 2, width, height) };
        }

        /// <summary>
        /// Because we extend "ThreeDim", we need to return a Mesh in
        /// a single line. This method will help us do that. 
        /// </summary>
        private static Mesh CreateSkybox(Vec4 Loc)
        {
            // Create blank triangles array
            Triangle[] Triangles = new Triangle[12];
            for (int index = 0; index < Triangles.Length; index++)
            {
                Triangles[index] = new Triangle();
            }

            // Assign texels to each one
            Triangles[0].Texels = new Vec3[] { new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0) };
            Triangles[1].Texels = new Vec3[] { new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1) };
            Triangles[2].Texels = new Vec3[] { new Vec3(1, 0), new Vec3(1, 1), new Vec3(0, 1) };
            Triangles[3].Texels = new Vec3[] { new Vec3(0, 1), new Vec3(0, 0), new Vec3(1, 0) };
            Triangles[4].Texels = new Vec3[] { new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0) };
            Triangles[5].Texels = new Vec3[] { new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1) };
            Triangles[6].Texels = new Vec3[] { new Vec3(1, 0), new Vec3(1, 1), new Vec3(0, 1) };
            Triangles[7].Texels = new Vec3[] { new Vec3(0, 1), new Vec3(0, 0), new Vec3(1, 0) };
            Triangles[8].Texels = new Vec3[] { new Vec3(0, 1), new Vec3(0, 0), new Vec3(1, 0) };
            Triangles[9].Texels = new Vec3[] { new Vec3(1, 0), new Vec3(1, 1), new Vec3(0, 1) };
            Triangles[10].Texels = new Vec3[] { new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0) };
            Triangles[11].Texels = new Vec3[] { new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1) };

            // Calculate and set triangle points/normals
            Vec4[][] points = CreateTrianglePoints(Loc); 
            for (int index = 0; index < Triangles.Length; index++)
            {
                Triangles[index].Points = points[index]; 
                MathExtensions.CalcTriNormal(Triangles[index]);
            }

            return new Mesh(Triangles); 
        }

        /// <summary>
        /// Loads a single part of the skybox. 
        /// </summary>
        private Color[,] LoadPart(Color[,] source, Rectangle rec)
        {
            Color[,] destination = new Color[rec.Width, rec.Height];

            int endX = rec.X + rec.Width,
                endY = rec.Y + rec.Height;
            for (int x = rec.X; x < endX; x++)
            {
                for (int y = rec.Y; y < endY; y++)
                {
                    destination[x - rec.X, y - rec.Y] = source[x, y];
                }
            }

            return destination;
        }

        /// <summary>
        /// Used in the constructor. Needs to create a base set of
        /// points for the triangle locations.
        /// 
        /// This is exactly what Update does, except this method is
        /// static instead. 
        /// </summary>
        private static Vec4[][] CreateTrianglePoints(Vec4 Loc)
        {
            Vec4[][] points = new Vec4[12][];

            // Front
            // Temporarily move Loc over, to prevent the need to
            // repeatedly move it over for every point 
            Loc.Z += Dist;
            points[0] = new Vec4[]
            {
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z)
            };
            points[1] = new Vec4[]
            {
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z)
            };
            // Move Loc back
            Loc.Z -= Dist;

            // Right
            Loc.X += Dist;
            points[2] = new Vec4[]
            {
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size),
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z - Size),
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size)
            };
            points[3] = new Vec4[]
            {
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size),
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z + Size),
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size)
            };
            Loc.X -= Dist;

            // Left
            Loc.X -= Dist;
            points[4] = new Vec4[]
            {
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size),
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z - Size),
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size)
            };
            points[5] = new Vec4[]
            {
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size),
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z + Size),
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size)
            };
            Loc.X += Dist;

            // Back
            Loc.Z -= Dist;
            points[6] = new Vec4[]
            {
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z)
            };
            points[7] = new Vec4[]
            {
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z)
            };
            Loc.Z += Dist;

            // Up
            Loc.Y += Dist;
            points[8] = new Vec4[]
            {
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size)
            };
            points[9] = new Vec4[]
            {
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size)
            };
            Loc.Y -= Dist;

            // Down
            Loc.Y -= Dist;
            points[10] = new Vec4[]
            {
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size)
            };
            points[11] = new Vec4[]
            {
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size)
            };
            Loc.Y += Dist;

            return points;
        }

        public void Update(Vec4 Loc)
        {
            // Front
                // Temporarily move Loc over, to prevent the need to
                // repeatedly move it over for every point 
                Loc.Z += Dist;
                Mesh.Triangles[0].Points = new Vec4[] 
                {
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z)
                };
                Mesh.Triangles[1].Points = new Vec4[]
                {
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z)
                };
                // Move Loc back
                Loc.Z -= Dist;

            // Right
                Loc.X += Dist;
                Mesh.Triangles[2].Points = new Vec4[]
                {
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size),
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z - Size),
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size)
                };
                Mesh.Triangles[3].Points = new Vec4[]
                {
                        new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size),
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z + Size),
                        new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size)
                };
                Loc.X -= Dist; 

            // Left
                Loc.X -= Dist;
                Mesh.Triangles[4].Points = new Vec4[]
                {
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size),
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z - Size),
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size)
                };
                Mesh.Triangles[5].Points = new Vec4[]
                {
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z - Size),
                    new Vec4(Loc.X, Loc.Y + Size, Loc.Z + Size),
                    new Vec4(Loc.X, Loc.Y - Size, Loc.Z + Size)
                };
                Loc.X += Dist; 

            // Back
                Loc.Z -= Dist;
                Mesh.Triangles[6].Points = new Vec4[]
                {
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z)
                };
                Mesh.Triangles[7].Points = new Vec4[]
                {
                    new Vec4(Loc.X + Size, Loc.Y - Size, Loc.Z),
                    new Vec4(Loc.X + Size, Loc.Y + Size, Loc.Z),
                    new Vec4(Loc.X - Size, Loc.Y + Size, Loc.Z)
                };
                Loc.Z += Dist;  
            
            // Up
                Loc.Y += Dist;
                Mesh.Triangles[8].Points = new Vec4[]
                {
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size)
                };
                Mesh.Triangles[9].Points = new Vec4[]
                {
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size)
                };
                Loc.Y -= Dist; 

            // Down
                Loc.Y -= Dist;
                Mesh.Triangles[10].Points = new Vec4[]
                {
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size)
                };
                Mesh.Triangles[11].Points = new Vec4[]
                {
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z - Size),
                    new Vec4(Loc.X - Size, Loc.Y, Loc.Z + Size),
                    new Vec4(Loc.X + Size, Loc.Y, Loc.Z + Size)
                };
                Loc.Y += Dist; 
        }

        ///// <summary>
        ///// Moves the skybox around the player. 
        ///// </summary>
        //public void Update(WeatherAndTime time, Vec4 cameraLoc)
        //{
        //    Color[][,] tex;
        //    if (time == WeatherAndTime.Day)
        //    {
        //        tex = Textures[0];
        //    }
        //    else
        //    {
        //        tex = Textures[1]; 
        //    }
        //}

        /// <summary>
        /// Draws the sky to the painter's canvas
        /// </summary>
        public void DrawToCanvas(Painter painter, Camera camera, Matrix matView, Matrix matProj, int DrawWidth, int DrawHeight, Hue hue)
        {
            // Decide what skybox we're using
            Color[][,] timeOne = null, timeTwo = null; 
            bool solid; // Whether or not we're using a single skybox texture 
            if (Textures.ContainsKey(hue.Status))     // If it's not a transitory time
            {
                timeOne = Textures[hue.Status];
                solid = true; 
            }
            else
            {
                switch (hue.Status)
                {
                    case WeatherAndTime.Dawn:
                        timeOne = Textures[WeatherAndTime.Night];
                        timeTwo = Textures[WeatherAndTime.Day];
                        break;

                    case WeatherAndTime.Dusk:
                        timeOne = Textures[WeatherAndTime.Day];
                        timeTwo = Textures[WeatherAndTime.Night];
                        break;  
                }
                solid = false; 
            }

            List<SkyboxTriangle> unclippedTriangles = new List<SkyboxTriangle>();

            for (int index = 0; index < Mesh.Triangles.Length; index++)
            {
                Triangle original = Mesh.Triangles[index];

                // Get ray from triangle to camera
                Vec4 cameraRay = original.Points[0] - camera.loc;

                // If ray is aligned with normal, then triangle is visible
                if (Vec4.DotProduct(original.Normal, cameraRay) < 0)
                {
                    // Clone triangle so we can edit and not need to make new
                    // triangle objects.
                    Triangle tri = original.Clone(); 

                    // If it has two images, we'll adjust that later. 
                    tri.Image = timeOne[index / 2];

                    // Convert World Space to View Space
                    tri.TimesEquals(matView);

                    // Clip viewed triangle against near plane (this could
                    // form two additional triangles)
                    Triangle[] clipped = Vec4.TriangleClipAgainstPlane(
                        new Vec4(0, 0, 0.1f),
                        new Vec4(0, 0, 1),
                        tri);

                    foreach (Triangle clippedTri in clipped)
                    {
                        // Project triangle from 3D to 2D
                        clippedTri.TimesEquals(matProj);
                        clippedTri.DivideTexel(0, clippedTri.Points[0].W);
                        clippedTri.DivideTexel(1, clippedTri.Points[1].W);
                        clippedTri.DivideTexel(2, clippedTri.Points[2].W);

                        // Scale into view 
                        clippedTri.Points[0].DivideEquals(clippedTri.Points[0].W);
                        clippedTri.Points[1].DivideEquals(clippedTri.Points[1].W);
                        clippedTri.Points[2].DivideEquals(clippedTri.Points[2].W);

                        // X/Y are inverted so put them back 
                        clippedTri.Points[0].X *= -1;
                        clippedTri.Points[1].X *= -1;
                        clippedTri.Points[2].X *= -1;
                        clippedTri.Points[0].Y *= -1;
                        clippedTri.Points[1].Y *= -1;
                        clippedTri.Points[2].Y *= -1;

                        // Offset verts into visible normalized space 
                        Vec4 offsetView = new Vec4(1, 1, 0);
                        clippedTri.Points[0].PlusEquals(offsetView);
                        clippedTri.Points[1].PlusEquals(offsetView);
                        clippedTri.Points[2].PlusEquals(offsetView);

                        clippedTri.Points[0].X *= 0.5f * DrawWidth; clippedTri.Points[0].Y *= 0.5f * DrawHeight;
                        clippedTri.Points[1].X *= 0.5f * DrawWidth; clippedTri.Points[1].Y *= 0.5f * DrawHeight;
                        clippedTri.Points[2].X *= 0.5f * DrawWidth; clippedTri.Points[2].Y *= 0.5f * DrawHeight;

                        if (!solid)
                        {
                            unclippedTriangles.Add(new SkyboxTriangle(clippedTri, timeTwo[index / 2]));
                        }
                        else
                        {
                            unclippedTriangles.Add(new SkyboxTriangle(clippedTri));
                        }
                    }
                }
            }
            
            foreach (SkyboxTriangle triangle in unclippedTriangles)
            {
                Queue<SkyboxTriangle> listTriangles = new Queue<SkyboxTriangle>();
                int nNewTriangles = 1;

                listTriangles.Enqueue(triangle);
                for (int p = 0; p < 4; p++)
                {
                    while (nNewTriangles > 0)
                    {
                        Triangle[] clipped = null;

                        // Take triangle from front of queue
                        SkyboxTriangle orig = listTriangles.Dequeue();
                        Triangle test = orig; 
                        nNewTriangles--;

                        // Clip it against a plane 
                        switch (p)
                        {
                            case 0:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, 0, 0),
                                    new Vec4(0, 1, 0),
                                    test);
                                break;

                            case 1:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, DrawHeight - 1, 0),
                                    new Vec4(0, -1, 0),
                                    test);
                                break;

                            case 2:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(0, 0, 0),
                                    new Vec4(1, 0, 0),
                                    test);
                                break;

                            case 3:
                                clipped = Vec4.TriangleClipAgainstPlane(
                                    new Vec4(DrawWidth - 1, 0, 0),
                                    new Vec4(-1, 0, 0),
                                    test);
                                break;
                        }

                        // Add clipped triangles to back of queue to 
                        // test again. 
                        for (int w = 0; w < clipped.Length; w++)
                        {
                            SkyboxTriangle clippedCasted = new SkyboxTriangle(clipped[w], orig.SecondImage);
                            listTriangles.Enqueue(clippedCasted);
                        }
                    }
                    nNewTriangles = listTriangles.Count;
                }

                if (solid)
                {
                    foreach (Triangle t in listTriangles)
                    {
                        TexturedTriangle(
                        (int)t.Points[0].X, (int)t.Points[0].Y,
                        (int)t.Points[1].X, (int)t.Points[1].Y,
                        (int)t.Points[2].X, (int)t.Points[2].Y,
                        t.Texels[0].U, t.Texels[0].V, t.Texels[0].W,
                        t.Texels[1].U, t.Texels[1].V, t.Texels[1].W,
                        t.Texels[2].U, t.Texels[2].V, t.Texels[2].W,
                        255,
                        t.Image,
                        painter);
                    }
                }
                else    // We'll have to draw the two skyboxes with different textures 
                {
                    foreach (SkyboxTriangle t in listTriangles)
                    {
                        TexturedTriangle(
                        (int)t.Points[0].X, (int)t.Points[0].Y,
                        (int)t.Points[1].X, (int)t.Points[1].Y,
                        (int)t.Points[2].X, (int)t.Points[2].Y,
                        t.Texels[0].U, t.Texels[0].V, t.Texels[0].W,
                        t.Texels[1].U, t.Texels[1].V, t.Texels[1].W,
                        t.Texels[2].U, t.Texels[2].V, t.Texels[2].W,
                        hue.Amount,
                        new Color[][,] { t.Image, t.SecondImage },
                        painter);

                        //Polygon(painter, (int)t.Points[0].X, (int)t.Points[0].Y, (int)t.Points[1].X, (int)t.Points[1].Y, (int)t.Points[2].X, (int)t.Points[2].Y);
                    }
                }
            }
        }

        private void Polygon(Painter painter, int x1, int y1, int x2, int y2, int x3, int y3)
        {
            painter.DrawLine(x1, y1, x2, y2, Color.White);
            painter.DrawLine(x2, y2, x3, y3, Color.White);
            painter.DrawLine(x3, y3, x1, y1, Color.White);
        }

        private void TexturedTriangle(
            int x1, int y1, int x2, int y2, int x3, int y3,
            float u1, float v1, float w1,
            float u2, float v2, float w2,
            float u3, float v3, float w3,
            byte alpha, Color[,] sprite, Painter painter)
        {
            if (y2 < y1)
            {
                // Swap y1 and y2
                int tempi = y1;
                y1 = y2;
                y2 = tempi;

                // Swap x1 and x2 
                tempi = x1;
                x1 = x2;
                x2 = tempi;

                // Swap u1 and u2
                float tempf = u1;
                u1 = u2;
                u2 = tempf;

                // Swap v1 and v2
                tempf = v1;
                v1 = v2;
                v2 = tempf;

                // Swap w1 and w2
                tempf = w1;
                w1 = w2;
                w2 = tempf;
            }
            if (y3 < y1)
            {
                // Swap y1 and y3
                int tempi = y1;
                y1 = y3;
                y3 = tempi;

                // Swap x1 and x3 
                tempi = x1;
                x1 = x3;
                x3 = tempi;

                // Swap u1 and u3
                float tempf = u1;
                u1 = u3;
                u3 = tempf;

                // Swap v1 and v3
                tempf = v1;
                v1 = v3;
                v3 = tempf;

                // Swap w1 and w3
                tempf = w1;
                w1 = w3;
                w3 = tempf;
            }
            if (y3 < y2)
            {
                // Swap y2 and y3
                int tempi = y2;
                y2 = y3;
                y3 = tempi;

                // Swap x2 and x3 
                tempi = x2;
                x2 = x3;
                x3 = tempi;

                // Swap u2 and u3
                float tempf = u2;
                u2 = u3;
                u3 = tempf;

                // Swap v2 and v3
                tempf = v2;
                v2 = v3;
                v3 = tempf;

                // Swap w2 and w3
                tempf = w2;
                w2 = w3;
                w3 = tempf;
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;
            float du1 = u2 - u1;
            float dv1 = v2 - v1;
            float dw1 = w2 - w1;

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;
            float du2 = u3 - u1;
            float dv2 = v3 - v1;
            float dw2 = w3 - w1;

            float daxStep = 0, dbxStep = 0,
                  du1Step = 0, dv1Step = 0,
                  du2Step = 0, dv2Step = 0,
                  dw1Step = 0, dw2Step = 0;

            if (dy2 != 0)
            {
                float dy2Abs = (float)Math.Abs(dy2);
                dbxStep = dx2 / dy2Abs;
                du2Step = du2 / dy2Abs;
                dv2Step = dv2 / dy2Abs;
                dw2Step = dw2 / dy2Abs;
            }

            if (dy1 != 0)
            {
                float dy1Abs = (float)Math.Abs(dy1);
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y1; i <= y2; i++)
                {
                    float iDiff = (float)(i - y1);

                    int ax = (int)(x1 + iDiff * daxStep);
                    int bx = (int)(x1 + iDiff * dbxStep);

                    float texSu = u1 + iDiff * du1Step;
                    float texSv = v1 + iDiff * dv1Step;
                    float texSw = w1 + iDiff * dw1Step;

                    float texEu = u1 + iDiff * du2Step;
                    float texEv = v1 + iDiff * dv2Step;
                    float texEw = w1 + iDiff * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu 
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv 
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw 
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f;

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;

                        int spriteLocU = (int)(texU / texW * (sprite.GetLength(0) - 1));
                        int spriteLocV = (int)(texV / texW * (sprite.GetLength(1) - 1));

                        Color color = sprite[spriteLocU, spriteLocV];
                        color.A = alpha;

                        painter.SetPixel(j, i, color);

                        t += tStep;
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            du1 = u3 - u2;
            dv1 = v3 - v2;
            dw1 = w3 - w2;

            if (dy2 != 0)
            {
                dbxStep = dx2 / (float)(Math.Abs(dy2));
            }

            du1Step = 0;
            dv1Step = 0;

            if (dy1 != 0)
            {
                float dy1Abs = (float)(Math.Abs(dy1));
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y2; i <= y3; i++)
                {
                    float iDiff1 = (float)(i - y1);
                    float iDiff2 = (float)(i - y2);

                    int ax = (int)(x2 + iDiff2 * daxStep);
                    int bx = (int)(x1 + iDiff1 * dbxStep);

                    float texSu = u2 + iDiff2 * du1Step;
                    float texSv = v2 + iDiff2 * dv1Step;
                    float texSw = w2 + iDiff2 * dw1Step;

                    float texEu = u1 + iDiff1 * du2Step;
                    float texEv = v1 + iDiff1 * dv2Step;
                    float texEw = w1 + iDiff1 * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f;

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;

                        int spriteLocU = (int)(texU / texW * (sprite.GetLength(0) - 1));
                        int spriteLocV = (int)(texV / texW * (sprite.GetLength(1) - 1));

                        Color color = sprite[spriteLocU, spriteLocV];
                        color.A = alpha;

                        painter.SetPixel(j, i, color);

                        t += tStep;
                    }
                }
            }
        }

        private void TexturedTriangle(
            int x1, int y1, int x2, int y2, int x3, int y3,
            float u1, float v1, float w1,
            float u2, float v2, float w2,
            float u3, float v3, float w3,
            float alpha, Color[][,] sprite, Painter painter)
        {
            if (y2 < y1)
            {
                // Swap y1 and y2
                int tempi = y1;
                y1 = y2;
                y2 = tempi;

                // Swap x1 and x2 
                tempi = x1;
                x1 = x2;
                x2 = tempi;

                // Swap u1 and u2
                float tempf = u1;
                u1 = u2;
                u2 = tempf;

                // Swap v1 and v2
                tempf = v1;
                v1 = v2;
                v2 = tempf;

                // Swap w1 and w2
                tempf = w1;
                w1 = w2;
                w2 = tempf;
            }
            if (y3 < y1)
            {
                // Swap y1 and y3
                int tempi = y1;
                y1 = y3;
                y3 = tempi;

                // Swap x1 and x3 
                tempi = x1;
                x1 = x3;
                x3 = tempi;

                // Swap u1 and u3
                float tempf = u1;
                u1 = u3;
                u3 = tempf;

                // Swap v1 and v3
                tempf = v1;
                v1 = v3;
                v3 = tempf;

                // Swap w1 and w3
                tempf = w1;
                w1 = w3;
                w3 = tempf;
            }
            if (y3 < y2)
            {
                // Swap y2 and y3
                int tempi = y2;
                y2 = y3;
                y3 = tempi;

                // Swap x2 and x3 
                tempi = x2;
                x2 = x3;
                x3 = tempi;

                // Swap u2 and u3
                float tempf = u2;
                u2 = u3;
                u3 = tempf;

                // Swap v2 and v3
                tempf = v2;
                v2 = v3;
                v3 = tempf;

                // Swap w2 and w3
                tempf = w2;
                w2 = w3;
                w3 = tempf;
            }

            int dy1 = y2 - y1;
            int dx1 = x2 - x1;
            float du1 = u2 - u1;
            float dv1 = v2 - v1;
            float dw1 = w2 - w1;

            int dy2 = y3 - y1;
            int dx2 = x3 - x1;
            float du2 = u3 - u1;
            float dv2 = v3 - v1;
            float dw2 = w3 - w1;

            float daxStep = 0, dbxStep = 0,
                  du1Step = 0, dv1Step = 0,
                  du2Step = 0, dv2Step = 0,
                  dw1Step = 0, dw2Step = 0;

            if (dy2 != 0)
            {
                float dy2Abs = (float)Math.Abs(dy2);
                dbxStep = dx2 / dy2Abs;
                du2Step = du2 / dy2Abs;
                dv2Step = dv2 / dy2Abs;
                dw2Step = dw2 / dy2Abs;
            }

            if (dy1 != 0)
            {
                float dy1Abs = (float)Math.Abs(dy1);
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y1; i <= y2; i++)
                {
                    float iDiff = (float)(i - y1);

                    int ax = (int)(x1 + iDiff * daxStep);
                    int bx = (int)(x1 + iDiff * dbxStep);

                    float texSu = u1 + iDiff * du1Step;
                    float texSv = v1 + iDiff * dv1Step;
                    float texSw = w1 + iDiff * dw1Step;

                    float texEu = u1 + iDiff * du2Step;
                    float texEv = v1 + iDiff * dv2Step;
                    float texEw = w1 + iDiff * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu 
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv 
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw 
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f;

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;

                        int spriteLocU = (int)(texU / texW * (sprite[0].GetLength(0) - 1));
                        int spriteLocV = (int)(texV / texW * (sprite[0].GetLength(1) - 1));

                        Color color = Color.Lerp(
                            sprite[0][spriteLocU, spriteLocV],
                            sprite[1][spriteLocU, spriteLocV],
                            alpha);
                        color.A = 255; 

                        painter.SetPixel(j, i, color); 

                        t += tStep;
                    }
                }
            }

            dy1 = y3 - y2;
            dx1 = x3 - x2;
            du1 = u3 - u2;
            dv1 = v3 - v2;
            dw1 = w3 - w2;

            if (dy2 != 0)
            {
                dbxStep = dx2 / (float)(Math.Abs(dy2));
            }

            du1Step = 0;
            dv1Step = 0;

            if (dy1 != 0)
            {
                float dy1Abs = (float)(Math.Abs(dy1));
                daxStep = dx1 / dy1Abs;
                du1Step = du1 / dy1Abs;
                dv1Step = dv1 / dy1Abs;
                dw1Step = dw1 / dy1Abs;

                for (int i = y2; i <= y3; i++)
                {
                    float iDiff1 = (float)(i - y1);
                    float iDiff2 = (float)(i - y2);

                    int ax = (int)(x2 + iDiff2 * daxStep);
                    int bx = (int)(x1 + iDiff1 * dbxStep);

                    float texSu = u2 + iDiff2 * du1Step;
                    float texSv = v2 + iDiff2 * dv1Step;
                    float texSw = w2 + iDiff2 * dw1Step;

                    float texEu = u1 + iDiff1 * du2Step;
                    float texEv = v1 + iDiff1 * dv2Step;
                    float texEw = w1 + iDiff1 * dw2Step;

                    if (ax > bx)
                    {
                        // Swap ax and bx
                        int tempi = ax;
                        ax = bx;
                        bx = tempi;

                        // Swap texSu and texEu
                        float tempf = texSu;
                        texSu = texEu;
                        texEu = tempf;

                        // Swap texSv and texEv
                        tempf = texSv;
                        texSv = texEv;
                        texEv = tempf;

                        // Swap texSw and texEw
                        tempf = texSw;
                        texSw = texEw;
                        texEw = tempf;
                    }

                    float texU = texSu;
                    float texV = texSv;
                    float texW = texSw;

                    float tStep = 1f / (float)(bx - ax);
                    float t = 0f;

                    for (int j = ax; j < bx; j++)
                    {
                        texU = (1f - t) * texSu + t * texEu;
                        texV = (1f - t) * texSv + t * texEv;
                        texW = (1f - t) * texSw + t * texEw;

                        int spriteLocU = (int)(texU / texW * (sprite[0].GetLength(0) - 1));
                        int spriteLocV = (int)(texV / texW * (sprite[0].GetLength(1) - 1));

                        Color color = Color.Lerp(
                            sprite[0][spriteLocU, spriteLocV],
                            sprite[1][spriteLocU, spriteLocV],
                            alpha);
                        color.A = 255;

                        painter.SetPixel(j, i, color);

                        t += tStep;
                    }
                }
            }
        }
    }
}
