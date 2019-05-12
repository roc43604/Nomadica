using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.InventorySpace
{
    public class CubeItem : Item
    {
        private const string FileName = @"Images/Inventory/cube";
        private const int ImageWidth = 152, ImageHeight = 153;

        public CubeItem(Vec4 Loc, ChunkGenerator cg)
            : base(Load.OneDimImage(FileName), ImageWidth, ImageHeight, Loc.X, Loc.Z, cg, 60, 60)
        { }
    }
}
