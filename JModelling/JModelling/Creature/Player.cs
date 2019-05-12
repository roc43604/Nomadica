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
        public const float Gravity = 0.07f;

        /// <summary>
        /// How far the player can attack with their weapon. 
        /// </summary>
        private const int WeaponDist = 60; 

        /// <summary>
        /// How tall the player model is, from the ground to where
        /// their eyes are. 
        /// </summary>
        private const int Height = 20;

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
        /// The items the player has on them. 
        /// </summary>
        public InventorySpace.Inventory Inventory;

        /// <summary>
        /// How much health the player has before dying. 
        /// </summary>
        public int Health;

        /// <summary>
        /// How much damage the player will deal when they hit something.
        /// </summary>
        public int Damage; 

        /// <summary>
        /// The last velocity vector the player had. Will update
        /// each frame. 
        /// </summary>
        private Vec4 lastVelocity;

        /// <summary>
        /// Whether or not the player is on the ground. 
        /// </summary>
        private bool isOnGround;

        /// <summary>
        /// Whether or not the player has taken damage in an amount
        /// of time. 
        /// </summary>
        public bool tookDamage;
        
        public Player(JManager manager, Camera Camera)
        {
            lastMouseX = -1;
            lastMouseY = -1; 

            this.manager = manager; 
            this.Camera = Camera;
            Health = 100;
            Damage = 20; 

            lastVelocity = Vec4.Zero;
            isOnGround = false;
            tookDamage = false;

            Inventory = new InventorySpace.Inventory(); 
        }

        /// <summary>
        /// Contains the actions taken after some creature attacks the player. 
        /// </summary>
        public void TookDamage(Creature attacker)
        {
            Health -= attacker.Damage;
            tookDamage = true;
            isOnGround = false;

            lastVelocity = ((MeleeAttacker)attacker).TravelVector;
            lastVelocity.Y = 1; 
        }

        /// <summary>
        /// Contains the actions taken after the player attacks with their weapon. 
        /// </summary>
        private void Attacked()
        {
            // If the monster is off the attack cool-down timer
            if (JManager.monster != null)
            {
                if (!JManager.monster.tookDamage)
                {
                    // If weaponLoc is within the monster's range, they take damage.
                    if (Math.Abs((Camera.yaw + JManager.PITimesTwo) - (MathExtensions.Wrap(JManager.monster.AngleToPlayer + (float)Math.PI / 2f) + JManager.PITimesTwo)) < (Math.PI / 7))
                    {
                        if (MathExtensions.Dist(Camera.loc, JManager.monster.Loc) < WeaponDist)
                        {
                            JManager.monster.TookDamage(this);
                        }
                        //Console.WriteLine("Yes!!!\t\t" + (Camera.yaw - MathExtensions.Wrap(JManager.monster.AngleToPlayer + (float)Math.PI / 2f)));

                    }
                    else
                    {
                        //Console.WriteLine("No!!!\t\t" + ((Camera.yaw + JManager.PITimesTwo) - (MathExtensions.Wrap(JManager.monster.AngleToPlayer + (float)Math.PI / 2f) + JManager.PITimesTwo)));
                    }
                }
            }

            //Console.WriteLine(MathExtensions.Wrap(JManager.monster.AngleToPlayer + (float)Math.PI / 2f));
            //Console.WriteLine(Camera.yaw);
            //Console.WriteLine(); 
        }

        /// <summary>
        /// Updates the location of the player, as well as any 
        /// variables set to show what the player is doing. 
        /// </summary>
        public void Update(KeyboardState kb, MouseState ms)
        {
            // Get where the player should move 
            Vec4 moveDir = lastVelocity.Clone(); 
            if (!tookDamage)
            {
                moveDir = GetMoveDirectionFromKeyboard(kb);
            }

            // If the player is in the air
            if (!isOnGround)
            {
                moveDir.Y = lastVelocity.Y - Gravity;
            }
            
            // Move the player. If the sprint key is pressed, move their x/z velocity quicker. 
            float speed = (kb.IsKeyDown(Controls.Sprint)) ? Camera.FastSpeed : Camera.NormalSpeed;

            if (tookDamage)
            {
                Camera.MoveWorldSpace(Camera.NormalSpeed, lastVelocity); 
            }
            else
            {
                Camera.MoveViewSpace(speed, moveDir);
            }

            // If player is below ground, set them on the ground. 
            float ground = manager.cg.GetHeightAt(Camera.loc.X, Camera.loc.Z);
            if (Camera.loc.Y < ground + Height)
            {
                Camera.loc.Y = ground + Height;
                moveDir.Y = 0;
                isOnGround = true;
                tookDamage = false; 
            }
            else
            {
                isOnGround = false; 
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
            
            // If user clicked, process an attack. 
            if (ms.LeftButton == ButtonState.Pressed)
            {
                Attacked();
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
            if (kb.IsKeyDown(Controls.Up) && isOnGround)
            {
                direction.Y += 1; 
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
