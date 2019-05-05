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
            float angle = (float)Math.Atan2(deltaZ, deltaX);

            Loc.X -= (float)Math.Cos(angle) * Speed;
            Loc.Z -= (float)Math.Sin(angle) * Speed;

            // Monster should be on floor, not floating
            Loc.Y = cg.GetHeightAt(Loc.X, Loc.Z) + 60;
        }
    }
}
