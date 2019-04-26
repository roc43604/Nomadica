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
    class Billboard
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

        /// <summary>
        /// Where this texture is located in world-space. 
        /// </summary>
        public Vec4 Loc; 

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight)
            : this(Texture, TextureWidth, TextureHeight, new Vec4(0, 30, 0))
        { }

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight, Vec4 Loc)
            : this(Texture, TextureWidth, TextureHeight, Loc, 10, 10)
        { }

        public Billboard(Color[] Texture, int TextureWidth, int TextureHeight, Vec4 Loc, int Width, int Height)
        {
            this.Texture = Texture;
            this.TextureWidth = TextureWidth;
            this.TextureHeight = TextureHeight; 
            this.Loc = Loc;
            this.Width = Width;
            this.Height = Height; 
        }
    }
}
