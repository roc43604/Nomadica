using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    /// <summary>
    /// A basic interface defining what an item should be. When
    /// creating an inventory item, have it implement this class. 
    /// </summary>
    public interface Item
    {
        /// <summary>
        /// How this item should look inside your inventory menu. 
        /// </summary>
        Texture2D ImageInInventory
        {
            get;
            set;
        } 
    }
}
