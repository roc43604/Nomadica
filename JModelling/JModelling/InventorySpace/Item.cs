using JModelling.Creature;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    /// <summary>
    /// A basic class defining what an item should be. When
    /// creating an inventory item, have it extend this class. 
    /// </summary>
    public abstract class Item : Billboard
    {
        /// <summary>
        /// How far away from an item you need to be before you pick it up. 
        /// </summary>
        private const int PickupRange = 30;

        /// <summary>
        /// How much time has passed since this items creation. Used
        /// for the bobbing effect in Update. 
        /// </summary>
        private int timer;

        /// <summary>
        /// The original location
        /// </summary>
        private float originalLocY; 

        public Item(Color[] tex, int TextureWidth, int TextureHeight, float x, float z, ChunkGenerator cg, int Width, int Height)
            : base(tex, TextureWidth, TextureHeight, new Vec4(x, cg.GetHeightAt(x, z) + 10, z), Width, Height)
        {
            timer = 0; 
            originalLocY = Loc.Y;
        }

        /// <summary>
        /// Puts the item in world-space (i.e. after a creature dies
        /// and they drop the item on death). 
        /// </summary>
        public void SetInWorldSpace(Vec4 Loc)
        {
            this.Loc = Loc;
            originalLocY = Loc.Y; 
        }

        /// <summary>
        /// Bobs this item up and down in world-space, and checks to
        /// see if it's close enough to be picked up by a player. 
        /// 
        /// Returns true if this item was picked up by the player. 
        /// </summary>
        public bool Update(Player player)
        {
            timer++;

            // Using a Sin graph, bobs the item up and down. 
            Loc.Y = originalLocY + 5 * (float)Math.Sin(timer / Math.PI / 10);

            // If the player can actually pick up this item, and if the player
            // is in range to pick it up, then pick it up. 
            if (player.Inventory.NumItems < player.Inventory.Items.Length &&
                MathExtensions.Dist(Loc, player.Camera.loc) < PickupRange)
            {
                player.Inventory.Add(this);
                timer = 0;
                return true; 
            }

            return false; 
        }
    }
}
