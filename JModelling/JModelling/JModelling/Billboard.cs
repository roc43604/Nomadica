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
                
                Vec4 worldView = matView * Loc;

                if (Vec4.VecClipAgainstPlane(
                        new Vec4(0, 0, 0.1f),
                        new Vec4(0, 0, 1),
                        worldView))
                {
                    Vec4 twoD = worldView * matProj;

                    Vec4 scale = twoD / twoD.W;
                    scale.X *= -1;
                    scale.Y *= -1;
                    Vec4 offsetView = new Vec4(1, 1, 0);
                    scale = scale + offsetView;
                    scale.X *= 0.5f * drawWidth; scale.Y *= 0.5f * drawHeight;

                    painter.DrawImage(
                        Texture, TextureWidth, TextureHeight, 
                        new Rectangle(
                            (int)scale.X - (int)viewWidth / 2,
                            (int)scale.Y - (int)viewHeight / 2, 
                            (int)viewWidth, (int)viewHeight), 
                        depthBuffer, 1f / dist);
                }
            }
        }
    }
}
