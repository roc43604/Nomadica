using JModelling.JModelling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.Creature
{
    /// <summary>
    /// The player character that the person playing the game will 
    /// control. 
    /// </summary>
    public class Player
    {
        /// <summary>
        /// The camera the player will see through. 
        /// </summary>
        public Camera Camera; 

        public Player(Camera Camera)
        {
            this.Camera = Camera; 
        }
    }
}
