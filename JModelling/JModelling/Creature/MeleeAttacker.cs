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
        private const float speed = Camera.NormalSpeed * 0.666f, damage = 20, health = 100, noticeDistance = 100;
        private static ChunkGenerator cg;

        public static Mesh mesh; 

        /// <summary>
        /// How tall this creature is. 
        /// </summary>
        public float Height; 

        /// <summary>
        /// The angle this creature makes to the player. 
        /// </summary>
        public float AngleToPlayer;

        public bool tookDamage;

        private Vec4 gravityVelocity;
        public Vec4 TravelVector; 

        public MeleeAttacker(Vec4 Location, Color color, int size) 
            : base(CreateMesh(Location, color, size), Location, speed, (int)damage, (int)health, (int)noticeDistance, new List<Item>(new Item[] { new CubeItem(Vec4.Zero, cg) }))
        {
            Height = (Mesh.bounds.Max.Y - Mesh.bounds.Min.Y) / 2;
            tookDamage = false;
            gravityVelocity = Vec4.Zero;
            TravelVector = Vec4.Zero; 
        }

        private static Mesh CreateMesh(Vec4 location, Color color, int size)
        {
            Mesh mesh = Load.Mesh(meshName, size, location.X, location.Y, location.Z);
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
        public override void Update(Player player, ChunkGenerator cg)
        {
            // Gravity 
            Loc.Y += gravityVelocity.Y;
            gravityVelocity.Y -= Player.Gravity;

            // Move forward towards player
            if (!tookDamage)
            {
                float deltaX = Loc.X - player.Camera.loc.X;
                float deltaZ = Loc.Z - player.Camera.loc.Z;
                AngleToPlayer = (float)Math.Atan2(deltaZ, deltaX) + (float)Math.PI;

                TravelVector = new Vec4((float)Math.Cos(AngleToPlayer), 0, (float)Math.Sin(AngleToPlayer)); 
                Loc.X += TravelVector.X * Speed;
                Loc.Z += TravelVector.Z * Speed;
            }
            else
            {
                float deltaX = Loc.X - player.Camera.loc.X;
                float deltaZ = Loc.Z - player.Camera.loc.Z;
                AngleToPlayer = (float)Math.Atan2(deltaZ, deltaX) + (float)Math.PI;

                TravelVector = new Vec4((float)Math.Cos(AngleToPlayer), 0, (float)Math.Sin(AngleToPlayer));
                Loc.X -= TravelVector.X * Speed;
                Loc.Z -= TravelVector.Z * Speed;
            }

            // Monster should be on floor, not floating
            float floor = cg.GetHeightAt(Loc.X, Loc.Z) + Height;
            if (Loc.Y < floor)
            {
                tookDamage = false;
                gravityVelocity = Vec4.Zero;
                Loc.Y = floor; 
            }

            // If collides with player, let them know
            float dist = MathExtensions.Dist(Loc, player.Camera.loc); 
            if (!player.tookDamage && dist < (Mesh.bounds.Max.X - Mesh.bounds.Min.X))
            {
                player.TookDamage(this);
            }
        }

        public void TookDamage(Player player)
        {
            Health -= player.Damage;             
            tookDamage = true;
            gravityVelocity = new Vec4(0, 1, 0); 
        }
    }
}
