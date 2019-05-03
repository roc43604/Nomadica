﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// Something that orbits around the center of the world (the player).
    /// This is probably just going to be the sun and the moon. 
    /// </summary>
    class Satellite : Billboard
    {
        /// <summary>
        /// How far away this satellite is from the player. 
        /// </summary>
        public float Dist = 1000;

        /// <summary>
        /// How fast this satellite moves. This is measured in
        /// radians. 
        /// </summary>
        public float Speed;

        /// <summary>
        /// The current angle measurement of this satellite to
        /// the origin point. This is measured in Radians. 
        /// </summary>
        public float Angle; 

        /// <summary>
        /// The image representing this Satellite. 
        /// </summary>
        public Color[] Tex;

        /// <summary>
        /// The dimensions of the texture above.
        /// </summary>
        private int texWidth, texHeight; 

        /// <summary>
        /// Creates a Satellite with variable speed, and a 
        /// defined image. 
        /// </summary>
        public Satellite(float Speed, Color[] Tex, int texWidth, int texHeight)
            : this(Speed, Tex, texWidth, texHeight, 0, 100, 100, 1000)
        { }

        public Satellite(float Speed, Color[] Tex, int texWidth, int texHeight, float Angle, int Width, int Height, float dist)
            : this(Speed, Tex, texWidth, texHeight, CalcLoc(dist, Vec4.Zero, 0), Angle, Width, Height, dist)
        { }

        public Satellite(float Speed, Color[] Tex, int texWidth, int texHeight, Vec4 Loc, float Angle, int Width, int Height, float Dist)
            : base(Tex, texWidth, texHeight, Loc, Width, Height)
        {
            this.Speed = Speed;
            this.Tex = Tex;
            this.texWidth = texWidth;
            this.texHeight = texHeight; 
            this.Loc = Loc;
            this.Angle = Angle;
            this.Dist = Dist; 
        }  

        /// <summary>
        /// Moves this Satellite a step forwards, given the
        /// point it is centered on. 
        /// </summary>
        public void Step(Vec4 centerPoint)
        {
            Angle += Speed;
            if (Angle > JManager.PITimesTwo)
            {
                Angle = 0; 
            }

            Loc = CalcLoc(Dist, centerPoint, Angle); 
        }

        /// <summary>
        /// Calculates the location of the satellite given
        /// the point it's centered on, the distance away it
        /// is, and the angle its measured at. 
        /// </summary>
        private static Vec4 CalcLoc(float dist, Vec4 centerPoint, float angle)
        {
            return new Vec4((float)Math.Cos(angle) * dist + centerPoint.X, (float)Math.Sin(angle) * dist, centerPoint.Z);
        }
    }
}
