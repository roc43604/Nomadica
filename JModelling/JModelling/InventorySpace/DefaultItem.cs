using JModelling.InventorySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace JModelling.InventorySpace
{
    public class DefaultItem : Item
    {
        private Texture2D tex; 

        public Texture2D ImageInInventory
        {
            get
            {
                return tex; 
            }

            set
            {
                tex = value; 
            }
        }

        public DefaultItem(Texture2D tex)
        {
            this.tex = tex; 
        }
    }
}
