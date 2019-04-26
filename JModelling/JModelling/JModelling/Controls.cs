using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JModelling.JModelling
{
    /// <summary>
    /// A complete list of all of the controls and control-configurations
    /// used and set by the player. 
    /// </summary>
    class Controls
    {
        /// <summary>
        /// Will exit the program when pressed. 
        /// </summary>
        public const Keys QuitProgram = Keys.Escape;

        /// <summary>
        /// Will tell the program whether or not it should read mouse inputs
        /// and reset mouse position. 
        /// </summary>
        public const Keys FocusOrUnfocusMouse = Keys.Tab; 

        /// <summary>
        /// Movement keys that move the player in a certain direction. 
        /// </summary>
        public const Keys Forward = Keys.W,
                          Backwards = Keys.S,
                          Left = Keys.A,
                          Right = Keys.D, 
                          Up = Keys.E, 
                          Down = Keys.Q;

        /// <summary>
        /// Moves the camera quicker than the default camera speed. 
        /// </summary>
        public const Keys Sprint = Keys.LeftShift; 

        /// <summary>
        /// How fast the camera will look when the mouse is moved. 
        /// </summary>
        public const int MouseSensitivity = 200; 
    }
}
