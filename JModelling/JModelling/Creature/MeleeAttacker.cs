using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JModelling.JModelling;
using JModelling.JModelling.Chunk;

namespace JModelling.Creature
{
    /// <summary>
    /// An abstract definition for a creature that attacks via
    /// direct contact with the player. A creature that fits this
    /// definition should extend this class. 
    /// </summary>
    public class MeleeAttacker : Creature
    {
        /// <summary>
        /// How tall this creature is. 
        /// </summary>
        public float Height; 

        /// <summary>
        /// The angle this creature makes to the player. 
        /// </summary>
        public float AngleToPlayer;

        private bool tookDamage;

        private Vec4 gravityVelocity; 

        public MeleeAttacker(Mesh mesh, Vec4 Location, float Speed, int Damage, int Health, int NoticeDistance) 
            : base(mesh, Location, Speed, Damage, Health, NoticeDistance)
        {
            Height = (Mesh.bounds.Max.Y - Mesh.bounds.Min.Y) / 2;
            tookDamage = false;
            gravityVelocity = Vec4.Zero; 
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

                Loc.X += (float)Math.Cos(AngleToPlayer) * Speed;
                Loc.Z += (float)Math.Sin(AngleToPlayer) * Speed;
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
            tookDamage = true;
            gravityVelocity = new Vec4(0, 1, 0); 
        }
    }
}
