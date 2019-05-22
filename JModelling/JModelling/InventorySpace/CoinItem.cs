using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    class CoinItem : Item
    {
        private const string FileName = @"Images/Inventory/coin";
        private const int ImageWidth = 234, ImageHeight = 243;

        public CoinItem(Vec4 Loc, ChunkGenerator cg)
            : base(Load.OneDimImage(FileName), ImageWidth, ImageHeight, Loc.X, Loc.Z, cg, 60, 60)
        { }
    }
}
