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
        /// The angle this creature makes to the player. 
        /// </summary>
        public float AngleToPlayer; 

        public MeleeAttacker(Mesh mesh, Vec4 Location, float Speed, int Damage, int Health, int NoticeDistance) 
            : base(mesh, Location, Speed, Damage, Health, NoticeDistance)
        { }

        /// <summary>
        /// Will move towards the player, trying to get close enough
        /// to attack. 
        /// </summary>
        public override void Update(Player player, ChunkGenerator cg)
        {
            // Move forward towards player
            float deltaX = Loc.X - player.Camera.loc.X;
            float deltaZ = Loc.Z - player.Camera.loc.Z;
            AngleToPlayer = (float)Math.Atan2(deltaZ, deltaX) + (float)Math.PI;

            Loc.X += (float)Math.Cos(AngleToPlayer) * Speed;
            Loc.Z += (float)Math.Sin(AngleToPlayer) * Speed;

            // Monster should be on floor, not floating
            Loc.Y = cg.GetHeightAt(Loc.X, Loc.Z) + (Mesh.bounds.Max.Y - Mesh.bounds.Min.Y) / 2;

            // If collides with player, let them know
            float dist = MathExtensions.Dist(Loc, player.Camera.loc); 
            if (!player.tookDamage && dist < (Mesh.bounds.Max.X - Mesh.bounds.Min.X))
            {
                player.TookDamage(this);
            }
        }
    }
}
