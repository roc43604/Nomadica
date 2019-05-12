using JModelling.InventorySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;

namespace JModelling.InventorySpace
{
    public class DefaultItem : Item
    {
        private const string FileName = @"Images/Inventory/item";
        private const int ImageWidth = 200, ImageHeight = 200; 

        public DefaultItem(Vec4 Loc, ChunkGenerator cg)
            : base(Load.OneDimImage(FileName), ImageWidth, ImageHeight, Loc.X, Loc.Z, cg, 100, 100)
        { }
    }
}
