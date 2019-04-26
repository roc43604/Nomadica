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
    public class SkyBox
    {
        /// <summary>
        /// How far away the skybox will be from the player. 
        /// </summary>
        public const int Dist = 10000;

        /// <summary>
        /// How big one part of the box is. 
        /// </summary>
        public const int Size = 10000;

        public Color[][,] Textures; 

        public Vec4 FrontLoc, RightLoc, LeftLoc, BackLoc, UpLoc, DownLoc;

        /// <summary>
        /// The triangles to render. 
        /// </summary>
        public Triangle[] Triangles; 

        /// <summary>
        /// Creates a sky box given the name of the file the image is in, and
        /// the Rectangle for each frame of the box. It will be loaded in the
        /// order of:
        /// 
        /// Front, Right, Left, Back, Up, Down. 
        /// </summary>
        public SkyBox(string fileName, Rectangle[] rectangles)
        {
            Color[,] source = Load.TwoDimImage(fileName);

            Textures = new Color[][,] {
                LoadPart(source, rectangles[0]),
                LoadPart(source, rectangles[1]), 
                LoadPart(source, rectangles[2]),
                LoadPart(source, rectangles[3]),
                LoadPart(source, rectangles[4]),
                LoadPart(source, rectangles[5]) };

            Triangles = new Triangle[12];
            Update(new Vec4(0, 0, 0)); 
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
        /// Moves the skybox around the player. 
        /// </summary>
        public void Update(Vec4 cameraLoc)
        {
            FrontLoc = cameraLoc.Clone();
            FrontLoc.Z += Dist;
            Triangles[0] = new Triangle(
                new Vec4(FrontLoc.X + Size, FrontLoc.Y - Size, FrontLoc.Z),
                new Vec4(FrontLoc.X - Size, FrontLoc.Y - Size, FrontLoc.Z),
                new Vec4(FrontLoc.X - Size, FrontLoc.Y + Size, FrontLoc.Z),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[0].Image = Textures[0];
            CalcTriNormal(Triangles[0]);
            Triangles[1] = new Triangle(
                new Vec4(FrontLoc.X - Size, FrontLoc.Y + Size, FrontLoc.Z),
                new Vec4(FrontLoc.X + Size, FrontLoc.Y + Size, FrontLoc.Z),
                new Vec4(FrontLoc.X + Size, FrontLoc.Y - Size, FrontLoc.Z),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[1].Image = Textures[0];
            CalcTriNormal(Triangles[1]);

            RightLoc = cameraLoc.Clone();
            RightLoc.X += Dist;
            Triangles[2] = new Triangle(
                new Vec4(RightLoc.X, RightLoc.Y + Size, RightLoc.Z - Size),
                new Vec4(RightLoc.X, RightLoc.Y - Size, RightLoc.Z - Size),
                new Vec4(RightLoc.X, RightLoc.Y - Size, RightLoc.Z + Size),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[2].Image = Textures[1];
            CalcTriNormal(Triangles[2]);
            Triangles[3] = new Triangle(
                new Vec4(RightLoc.X, RightLoc.Y - Size, RightLoc.Z + Size),
                new Vec4(RightLoc.X, RightLoc.Y + Size, RightLoc.Z + Size),
                new Vec4(RightLoc.X, RightLoc.Y + Size, RightLoc.Z - Size),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[3].Image = Textures[1];
            CalcTriNormal(Triangles[3]);

            LeftLoc = cameraLoc.Clone();
            LeftLoc.X -= Dist;
            Triangles[4] = new Triangle(
                new Vec4(LeftLoc.X, LeftLoc.Y - Size, LeftLoc.Z + Size),
                new Vec4(LeftLoc.X, LeftLoc.Y - Size, LeftLoc.Z - Size),
                new Vec4(LeftLoc.X, LeftLoc.Y + Size, LeftLoc.Z - Size),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[4].Image = Textures[2];
            CalcTriNormal(Triangles[4]);
            Triangles[5] = new Triangle(
                new Vec4(LeftLoc.X, LeftLoc.Y + Size, LeftLoc.Z - Size),
                new Vec4(LeftLoc.X, LeftLoc.Y + Size, LeftLoc.Z + Size),
                new Vec4(LeftLoc.X, LeftLoc.Y - Size, LeftLoc.Z + Size),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[5].Image = Textures[2];
            CalcTriNormal(Triangles[5]);

            BackLoc = cameraLoc.Clone();
            BackLoc.Z -= Dist;
            Triangles[6] = new Triangle(
                new Vec4(BackLoc.X - Size, BackLoc.Y + Size, BackLoc.Z),
                new Vec4(BackLoc.X - Size, BackLoc.Y - Size, BackLoc.Z),
                new Vec4(BackLoc.X + Size, BackLoc.Y - Size, BackLoc.Z),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[6].Image = Textures[3];
            CalcTriNormal(Triangles[6]);
            Triangles[7] = new Triangle(
                new Vec4(BackLoc.X + Size, BackLoc.Y - Size, BackLoc.Z),
                new Vec4(BackLoc.X + Size, BackLoc.Y + Size, BackLoc.Z),
                new Vec4(BackLoc.X - Size, BackLoc.Y + Size, BackLoc.Z),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[7].Image = Textures[3];
            CalcTriNormal(Triangles[7]);

            UpLoc = cameraLoc.Clone();
            UpLoc.Y += Dist;
            Triangles[8] = new Triangle(
                new Vec4(UpLoc.X - Size, UpLoc.Y, UpLoc.Z - Size),
                new Vec4(UpLoc.X + Size, UpLoc.Y, UpLoc.Z - Size),
                new Vec4(UpLoc.X + Size, UpLoc.Y, UpLoc.Z + Size),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[8].Image = Textures[4];
            CalcTriNormal(Triangles[8]);
            Triangles[9] = new Triangle(
                new Vec4(UpLoc.X + Size, UpLoc.Y, UpLoc.Z + Size),
                new Vec4(UpLoc.X - Size, UpLoc.Y, UpLoc.Z + Size),
                new Vec4(UpLoc.X - Size, UpLoc.Y, UpLoc.Z - Size),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[9].Image = Textures[4];
            CalcTriNormal(Triangles[9]);

            DownLoc = cameraLoc.Clone();
            DownLoc.Y -= Dist;
            Triangles[10] = new Triangle(
                new Vec4(DownLoc.X + Size, DownLoc.Y, DownLoc.Z + Size),
                new Vec4(DownLoc.X + Size, DownLoc.Y, DownLoc.Z - Size),
                new Vec4(DownLoc.X - Size, DownLoc.Y, DownLoc.Z - Size),
                new Vec3(1, 1), new Vec3(0, 1), new Vec3(0, 0));
            Triangles[10].Image = Textures[5];
            CalcTriNormal(Triangles[10]);
            Triangles[11] = new Triangle(
                new Vec4(DownLoc.X - Size, DownLoc.Y, DownLoc.Z - Size),
                new Vec4(DownLoc.X - Size, DownLoc.Y, DownLoc.Z + Size),
                new Vec4(DownLoc.X + Size, DownLoc.Y, DownLoc.Z + Size),
                new Vec3(0, 0), new Vec3(1, 0), new Vec3(1, 1));
            Triangles[11].Image = Textures[5];
            CalcTriNormal(Triangles[11]);
        }

        private void CalcTriNormal(Triangle triangle)
        {
            Vec4 normal =
                Vec4.CrossProduct(
                    triangle.Points[1] - triangle.Points[0],
                    triangle.Points[2] - triangle.Points[0]);
            normal.Normalize();
            triangle.Normal = normal;
        }
    }
}
