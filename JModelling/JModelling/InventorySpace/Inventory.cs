using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    /// <summary>
    /// A collection of the items on the player. 
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// The size of the player's inventory, in the format (x, y).
        /// </summary>
        public static readonly Vector2 Size = new Vector2(9, 3);

        /// <summary>
        /// Each item stored on the player. Accessed via (x, y). 
        /// </summary>
        public Item[,] Items;

        public Inventory()
        {
            Items = new Item[(int)Size.X, (int)Size.Y]; 
        }

        /// <summary>
        /// Adds an item to the inventory, putting it in the next
        /// available space. Returns whether or not the item could
        /// have been added. 
        /// </summary>
        public bool Add(Item item)
        {
            for (int y = 0; y < Items.GetLength(1); y++)
            {
                for (int x = 0; x < Items.GetLength(0); x++)
                {
                    if (Items[x, y] == null)
                    {
                        Items[x, y] = item;
                        return true; 
                    }
                }
            }

            return false; 
        }
    }
}
