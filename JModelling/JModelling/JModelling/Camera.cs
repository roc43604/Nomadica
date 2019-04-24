using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// The thing the player will be viewing the scene from. 
    /// </summary>
    class Camera
    {
        /// <summary>
        /// How fast the camera will normally move.
        /// </summary>
        public const float NormalSpeed = 0.3f;

        /// <summary>
        /// How fast the camera will move when the user is
        /// accelerating it. 
        /// </summary>
        public const float FastSpeed = 0.5f;

        /// <summary>
        /// The current speed this is moving. 
        /// </summary>
        public float speed; 

        /// <summary>
        /// The directions the camera can be looking. Yaw is
        /// left/right, pitch is up/down, and roll is clockwise/
        /// anticlockwise. 
        /// </summary>
        public float yaw, pitch, roll;

        /// <summary>
        /// Where the camera is located at any moment.  
        /// </summary>
        public Vec4 loc;

        /// <summary>
        /// The direction the camera is looking at. 
        /// </summary>
        public Vec4 lookDir; 

        public Camera()
            : this(0, 0, 0)
        { }

        public Camera(float x, float y, float z)
        {
            loc = new Vec4(x, y, z);
        }

        public void Move(float speed, Vec4 direction)
        {
            loc = loc + GetLookDir(direction) * speed; 
        }

        public Vec4 GetLookDir(Vec4 direction)
        {
            return Matrix.MakeRotationY(yaw) * direction; 
        }
    }
}
