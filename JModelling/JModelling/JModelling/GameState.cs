using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// What screen is showing
    /// </summary>
    enum GameState
    {
        /// <summary>
        /// Regular game is showing. Mouse is invisible, and key inputs
        /// are locked on game. 
        /// </summary>
        Playing, 

        /// <summary>
        /// Inventory menu is showing. Player can move their mouse around
        /// and change the state of their inventory. 
        /// </summary>
        Inventory,

        /// <summary>
        /// Pause menu is showing. Player can move their mouse around and 
        /// change their settings or quit the game. 
        /// </summary>
        Paused, 

        /// <summary>
        /// Player is talking with someone/something. 
        /// </summary>
        Talking
    }
}
