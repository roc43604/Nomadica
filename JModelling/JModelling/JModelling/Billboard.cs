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
    /// A sprite rendered in 3D space that's always facing the camera.
    /// </summary>
    public class Billboard : Entity
    {
        /// <summary>
        /// The texture itself. 
        /// </summary>
        public Color[] Texture;
        public int TextureWidth, TextureHeight; 

        /// <summary>
        /// The size of this texture in world-space.  
        /// </summary>
        public int Width, Height;

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight)
            : this(Texture, TextureWidth, TextureHeight, new Vec4(0, 30, 0))
        { }

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight, Vec4 Loc)
            : this(Texture, TextureWidth, TextureHeight, Loc, 10, 10)
        { }

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight, Vec4 Loc, int Width, int Height)
            : base(Loc)
        { 
            this.Texture = Texture;
            this.TextureWidth = TextureWidth;
            this.TextureHeight = TextureHeight; 
            this.Loc = Loc;
            this.Width = Width;
            this.Height = Height; 
        }

        public void DrawToCanvas(
            Camera camera, Painter painter, float[,] depthBuffer,
            Matrix matView, Matrix matProj,
            int drawWidth, int drawHeight)
        {
            float dist = (float)MathExtensions.Dist(camera.loc, Loc);
            if (dist > 0.1f)
            {
                float viewHeight = Height / dist * 0.1f * 300f;
                float viewWidth = Width / dist * 0.1f * 300f;

                // Have a copy to translate. 
                Vec4 point = Loc.Clone();

                // Convert World Space to View Space
                point.TimesEquals(matView);

                if (Vec4.VecClipAgainstPlane(
                        new Vec4(0, 0, 0.1f),
                        new Vec4(0, 0, 1),
                        point))
                {
                    // Project triangle from 3D to 2D
                    point.TimesEquals(matProj);

                    // Scale into view
                    point.DivideEquals(point.W);

                    // X/Y are inverted so put them back 
                    point.X *= -1;
                    point.Y *= -1;

                    // Offset verts into visible normalized space 
                    Vec4 offsetView = new Vec4(1, 1, 0);
                    point.PlusEquals(offsetView);

                    point.X *= 0.5f * drawWidth;
                    point.Y *= 0.5f * drawHeight;

                    painter.DrawImage(
                        Texture, TextureWidth, TextureHeight,
                        new Rectangle(
                            (int)point.X - (int)viewWidth / 2,
                            (int)point.Y - (int)viewHeight / 2,
                            (int)viewWidth, (int)viewHeight),
                        depthBuffer, 1f / dist);
                }
            }
        }
    }
}
