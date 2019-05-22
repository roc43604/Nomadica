using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;
using JModelling.InventorySpace;
using Microsoft.Xna.Framework;

namespace JModelling.Creature
{
    /// <summary>
    /// An abstract definition for a creature that attacks via
    /// direct contact with the player. A creature that fits this
    /// definition should extend this class. 
    /// </summary>
    public abstract class MeleeAttacker : Creature
    {
        private const string meshName = @"Content/Models/cube.obj";
        private const float speed = Camera.NormalSpeed * 0.666f, damage = 20, health = 100, noticeDistance = 500;
        private static ChunkGenerator cg;

        public static Mesh mesh; 

        /// <summary>
        /// How tall this creature is. 
        /// </summary>
        public float Height;

        public MeleeAttacker(Vec4 Location, Color color, int size, MonsterType type) 
            : base(CreateMesh(Location, color, size), Location, speed, (int)damage, (int)health, (int)noticeDistance, new List<Item>(new Item[] { new CubeItem(Vec4.Zero, cg) }), type)
        {
            Height = (Mesh.bounds.Max.Y - Mesh.bounds.Min.Y) / 2;
            tookDamage = false;
            gravityVelocity = Vec4.Zero;
            TravelVector = Vec4.Zero; 
        }

        private static Mesh CreateMesh(Vec4 location, Color color, int size)
        {
            Mesh mesh = Load.Mesh(meshName, size, 0, 0, 0);
            mesh.SetColor(color);
            return mesh; 
        }

        public static void Init(ChunkGenerator cg)
        {
            MeleeAttacker.cg = cg; 
        }

        /// <summary>
        /// Will move towards the player, trying to get close enough
        /// to attack. 
        /// </summary>
        public override void Update(Player player)
        {
            // Gravity 
            Loc.Y += gravityVelocity.Y;
            gravityVelocity.Y -= Player.Gravity;

            // Monster should be on floor, not floating
            float floor = cg.GetHeightAt(Loc.X, Loc.Z) + Height;
            if (Loc.Y < floor)
            {
                tookDamage = false;
                gravityVelocity = Vec4.Zero;
                Loc.Y = floor;
            }

            float dist = MathExtensions.Dist(player.Camera.loc, Loc);
            if (dist < noticeDistance)
            {
                // Move forward towards player
                float deltaX = Loc.X - player.Camera.loc.X;
                float deltaZ = Loc.Z - player.Camera.loc.Z;
                AngleToPlayer = (float)Math.Atan2(deltaZ, deltaX) + (float)Math.PI;

                TravelVector = new Vec4((float)Math.Cos(AngleToPlayer), 0, (float)Math.Sin(AngleToPlayer));

                if (tookDamage)
                {
                    Loc.X -= TravelVector.X * Speed;
                    Loc.Z -= TravelVector.Z * Speed;
                }
                else
                {
                    Loc.X += TravelVector.X * Speed;
                    Loc.Z += TravelVector.Z * Speed;
                }

                // If collides with player, let them know
                if (!player.tookDamage && dist < (Mesh.bounds.Max.X - Mesh.bounds.Min.X))
                {
                    player.TookDamage(this);
                }
            }
        }
    }
}
