using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using JModelling.InventorySpace;
using GraphicsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using JModelling.Creature;
namespace JModelling.Creature.Nomad
{
    class Chief : Nomads
    {

        public Chief(Mesh mesh, Vec4 Location, float Speed, int Damage, int Health, int NoticeDistance, ChunkGenerator cg)
            : base(mesh, Location, Speed, Damage, Health, NoticeDistance, cg)
        {
            Mesh.SetColor(Color.Orange);
        }
        public override void Update(Player player)
        {
            // Gravity 
            Loc.Y += gravityVelocity.Y;
            gravityVelocity.Y -= Player.Gravity;

            // Monster should be on floor, not floating
            float floor = cg.GetHeightAt(Loc.X, Loc.Z) + Height;
            if (Loc.Y < floor)
            {
                gravityVelocity = Vec4.Zero;
                Loc.Y = floor;
            }
        }
    }
}