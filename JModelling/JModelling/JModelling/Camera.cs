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
    public class Camera
    {
        /// <summary>
        /// How fast the camera will normally move.
        /// </summary>
        public const float NormalSpeed = 10f;

        /// <summary>
        /// How fast the camera will move when the user is
        /// accelerating it. 
        /// </summary>
        public const float FastSpeed = NormalSpeed * 3f;

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
            Vec4 lookDir = GetLookDir(direction);
            lookDir.X *= speed;
            lookDir.Z *= speed;
            lookDir.Y *= NormalSpeed; // Should always be normal speed. Holding sprint won't
                                      // make you fall any quicker. 
            loc = loc + lookDir; 
        }

        public Vec4 GetLookDir(Vec4 direction)
        {
            return Matrix.MakeRotationY(yaw) * direction; 
        }
    }
}
