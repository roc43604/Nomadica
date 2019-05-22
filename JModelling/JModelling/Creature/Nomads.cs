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

namespace JModelling.Creature
{
    class Nomads : Creature
    {
        /// <summary>
        /// How tall this creature is. 
        /// </summary>
        public float Height;

        /// <summary>
        /// The angle this creature makes to the player. 
        /// </summary>
        public bool clicked;
        public Vec4 gravityVelocity;
        public Vec4 TravelVector;
        public Nomads(Mesh mesh, Vec4 Location, float Speed, int Damage, int Health, int NoticeDistance, ChunkGenerator cg)
            : base(mesh, Location, Speed, Damage, Health, NoticeDistance, new List<Item>(new Item[] { new CubeItem(Vec4.Zero, cg) }), MonsterType.None)
        {
            Height = (Mesh.bounds.Max.Y - Mesh.bounds.Min.Y) / 2;
            gravityVelocity = Vec4.Zero;
            TravelVector = Vec4.Zero;
            clicked = false;
            Mesh.SetColor(Color.Blue);
        }
        public void ifclicked()
        {

        }
        public override void Update(Player player)
        {
            // Gravity 
            Loc.Y += gravityVelocity.Y;
            gravityVelocity.Y -= Player.Gravity;
            // nomad should be on floor, not floating
            float floor = cg.GetHeightAt(Loc.X, Loc.Z) + Height;
            if (Loc.Y < floor)
            {
                gravityVelocity = Vec4.Zero;
                Loc.Y = floor;
            }
        }
    }
}