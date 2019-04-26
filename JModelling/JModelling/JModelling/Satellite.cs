using Microsoft.Xna.Framework;
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
    class Satellite
    {
        /// <summary>
        /// How far away this satellite is from the player. 
        /// </summary>
        public const float Dist = 500;

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
        /// The current location of this Satellite. 
        /// </summary>
        public Vec4 Loc; 

        /// <summary>
        /// Creates a Satellite with variable speed, and a 
        /// defined image. 
        /// </summary>
        public Satellite(float Speed, Color[] Tex)
            : this(Speed, Tex, CalcLoc(Vec4.Zero, 0), 0)
        { }

        public Satellite(float Speed, Color[] Tex, Vec4 Loc, float Angle)
        {
            this.Speed = Speed;
            this.Tex = Tex;
            this.Loc = Loc;
            this.Angle = Angle;
        }

        /// <summary>
        /// Moves this Satellite a step forwards, given the
        /// point it is centered on. 
        /// </summary>
        public void Step(Vec4 centerPoint)
        {
            Angle += Speed;
            Loc = CalcLoc(centerPoint, Angle); 
        }

        /// <summary>
        /// Calculates the location of the satellite given
        /// the point it's centered on, the distance away it
        /// is, and the angle its measured at. 
        /// </summary>
        private static Vec4 CalcLoc(Vec4 centerPoint, float angle)
        {
            return new Vec4((float)Math.Cos(angle) * Dist, (float)Math.Sin(angle) * Dist, centerPoint.Z);
        }
    }
}
