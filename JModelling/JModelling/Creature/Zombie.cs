using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JModelling.JModelling;
using Microsoft.Xna.Framework;

namespace JModelling.Creature
{
    public class Zombie : MeleeAttacker
    {
        private const int size = 20;
        private static readonly Color color = Color.Green;  

        public Zombie(Vec4 Location) 
            : base(Location, color, size)
        {

        }
    }
}
