using JModelling.JModelling;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// The player character that the person playing the game will 
    /// control. 
    /// </summary>
    public class Player
    {
        /// <summary>
        /// How fast the player will fall downwards each frame. 
        /// </summary>
        private const float Gravity = 0.04f;

        /// <summary>
        /// How tall the player model is, from the ground to where
        /// their eyes are. 
        /// </summary>
        private const int Height = 60;

        /// <summary>
        /// The last known location of the mouse. 
        /// </summary>
        private int lastMouseX, lastMouseY;

        /// <summary>
        /// The thing housing this player object. 
        /// </summary>
        private JManager manager; 

        /// <summary>
        /// The camera the player will see through. 
        /// </summary>
        public Camera Camera;

        /// <summary>
        /// How much health the player has before dying. 
        /// </summary>
        public int Health;

        /// <summary>
        /// The last velocity vector the player had. Will update
        /// each frame. 
        /// </summary>
        private Vec4 lastVelocity; 

        public Player(JManager manager, Camera Camera)
        {
            lastMouseX = -1;
            lastMouseY = -1; 

            this.manager = manager; 
            this.Camera = Camera;
            Health = 100;

            lastVelocity = Vec4.Zero; 
        }

        /// <summary>
        /// Updates the location of the player, as well as any 
        /// variables set to show what the player is doing. 
        /// </summary>
        public void Update(KeyboardState kb, MouseState ms)
        {
            // Get where the player should move 
            Vec4 moveDir = GetMoveDirectionFromKeyboard(kb);

            // If the player jumped
            if (moveDir.Y != 0)
            {
                
            }
            // Otherwise, add gravity
            else
            {
                moveDir.Y = lastVelocity.Y - Gravity;
            }
            
            // Move the player. If the sprint key is pressed, move their x/z velocity quicker. 
            float speed = (kb.IsKeyDown(Controls.Sprint)) ? Camera.FastSpeed : Camera.NormalSpeed;

            Camera.Move(speed, moveDir);

            // If player is below ground, set them on the ground. 
            float ground = manager.cg.GetHeightAt(Camera.loc.X, Camera.loc.Z);
            if (Camera.loc.Y < ground + Height)
            {
                Camera.loc.Y = ground + Height;
                moveDir.Y = 0;
            }

            // If the mouse is focused and has moved since the last frame...
            if (manager.isMouseFocused && (ms.X != lastMouseX || ms.Y != lastMouseY))
            {
                // Update last known positions.
                lastMouseX = ms.X;
                lastMouseY = ms.Y;

                // Process the mouse. 
                ProcessMouseLoc(ms.X, ms.Y);
            }
            
            lastVelocity = moveDir; 
        }

        /// <summary>
        /// Gets where the player will be moving, as well as how fast they
        /// will move, from the keyboard. 
        /// </summary>
        private Vec4 GetMoveDirectionFromKeyboard(KeyboardState kb)
        {
            // Will add each movement vector to this base vector if the 
            // associated movement key is pressed. 
            Vec4 direction = new Vec4();

            // Forward/Backwards
            if (kb.IsKeyDown(Controls.Forward))
            {
                direction.Z += 1;
            }
            if (kb.IsKeyDown(Controls.Backwards))
            {
                direction.Z -= 1; 
            }

            // Left/Right
            if (kb.IsKeyDown(Controls.Left))
            {
                direction.X += 1; 
            }
            if (kb.IsKeyDown(Controls.Right))
            {
                direction.X -= 1; 
            }

            // Up/Down
            if (kb.IsKeyDown(Controls.Up))
            {
                direction.Y += 1; 
            }
            if (kb.IsKeyDown(Controls.Down))
            {
                direction.Y -= 1; 
            }

            return direction;
        }

        public void ProcessMouseLoc(int mouseLocX, int mouseLocY)
        {
            float x = mouseLocX,
                  y = mouseLocY; 

            x -= JManager.centerX;
            y -= JManager.centerY;

            x /= Controls.MouseSensitivity;
            y /= Controls.MouseSensitivity;

            Camera.yaw += x;
            Camera.pitch += y;

            if (Camera.yaw > JManager.PITimesTwo)
            {
                Camera.yaw = 0;
            }
            else if (Camera.yaw < 0)
            {
                Camera.yaw = JManager.PITimesTwo;
            }

            Mouse.SetPosition(JManager.centerX, JManager.centerY);
        }
    }
}
