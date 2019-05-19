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
        public const float NormalSpeed = 1.5f;

        /// <summary>
        /// How fast the camera will move when the user is
        /// accelerating it. 
        /// </summary>
        public const float FastSpeed = NormalSpeed * 15f;

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

        /// <summary>
        /// Moves the player in view-space, orienting movement around
        /// key inputs. For example, Controls.Back will move player 
        /// backwards from where they're facing. 
        /// </summary>
        public void MoveViewSpace(float speed, Vec4 direction)
        {
            MoveWorldSpace(speed, GetLookDir(direction));
        }

        /// <summary>
        /// Moves the player in world-space, not view space. 
        /// </summary>
        public void MoveWorldSpace(float speed, Vec4 direction)
        {
            direction.X *= speed;
            direction.Z *= speed;
            direction.Y *= NormalSpeed; // Should always be normal speed. Holding sprint won't
                                        // make you fall any quicker. 
            loc = loc + direction; 
        }

        public Vec4 GetLookDir(Vec4 direction)
        {
            return Matrix.MakeRotationY(yaw) * direction; 
        }
    }
}
