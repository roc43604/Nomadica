using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// Exactly the same as a regular Triangle with the added
    /// ability to store multiple images. 
    /// </summary>
    public class SkyboxTriangle : Triangle
    {
        /// <summary>
        /// A second image used alongside the first. This is used
        /// when a skybox has two separate images in a transitory
        /// state, like during dawn/dusk. 
        /// </summary>
        public Color[,] SecondImage; 

        /// <summary>
        /// Creates a skybox triangle given what it's supposed to
        /// mimic. 
        /// </summary>
        public SkyboxTriangle(Triangle parent)
            : base(parent.Points, parent.Texels, parent.Color, parent.Image, parent.Normal, parent.NormalLength)
        { }

        /// <summary>
        /// Creates a skybox triangle given what it's supposed to
        /// mimic, as well as the second image it contains
        /// </summary>
        public SkyboxTriangle(Triangle parent, Color[,] SecondImage)
            : base(parent.Points, parent.Texels, parent.Color, parent.Image, parent.Normal, parent.NormalLength)
        {
            this.SecondImage = SecondImage; 
        }
    }
}
